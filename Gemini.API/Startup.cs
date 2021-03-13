using AutoMapper;
using Gemini.API.Profiles;
using Gemini.API.Swagger;
using Gemini.Data.DbContexts;
using Gemini.Data.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.OData.Edm;
using Microsoft.AspNet.OData.Builder;
using Gemini.Shared.Models;
using Gemini.Data.Entities;
using Microsoft.AspNet.OData.Formatter;

namespace Gemini.API
{
    /// <summary>
    /// Star class for asp.net
    /// </summary>
    public class Startup
    {
        private IConfiguration Configuration { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// <paramref name="services"/>
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(
                configure =>
                {
                    configure.ReturnHttpNotAcceptable = true;
                    configure.Filters.Add(
                        new ProducesResponseTypeAttribute(StatusCodes.Status500InternalServerError));
                    configure.Filters.Add(
                        new ProducesResponseTypeAttribute(StatusCodes.Status406NotAcceptable));
                })
                .AddXmlDataContractSerializerFormatters();

            services.AddApiVersioning(
                setupAction =>
                {
                    setupAction.ReportApiVersions = true;
                    setupAction.AssumeDefaultVersionWhenUnspecified = true;
                    setupAction.DefaultApiVersion = new ApiVersion(1, 0);
                });

            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new GeminiProfile(Configuration));
            });

            var mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddScoped<IGeminiRepository, GeminiRepository>();

            services.AddDbContext<GeminiContext>();

            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            services.AddOData().EnableApiVersioning();

            services.AddODataApiExplorer(setupAction =>
            {
                setupAction.GroupNameFormat = "'v'VV";
                setupAction.SubstituteApiVersionInUrl = true;
            });

            services.AddSwaggerGen(
                setupAction =>
                {
                    setupAction.DocInclusionPredicate((documentName, apiDescription) =>
                    {
                        var actionApiVersionModel = apiDescription.ActionDescriptor
                            .GetApiVersionModel();

                        if (actionApiVersionModel == null)
                        {
                            return true;
                        }

                        var librarySpecificationPrefixWithVersion = $"{ConfigureSwaggerOptions.LibrarySpecificationPrefix}v";

                        if (actionApiVersionModel.DeclaredApiVersions.Any())
                        {
                            return actionApiVersionModel.DeclaredApiVersions.Any(v =>
                            $"{librarySpecificationPrefixWithVersion}{v}" == documentName);
                        }

                        return actionApiVersionModel.ImplementedApiVersions.Any(v =>
                            $"{librarySpecificationPrefixWithVersion}{v}" == documentName);
                    });

                    var currentAssembly = Assembly.GetExecutingAssembly();

                    if (currentAssembly?.Location != null)
                    {
                        var path = Path.GetDirectoryName(currentAssembly.Location) ?? string.Empty;

                        var xmlDocs = currentAssembly.GetReferencedAssemblies()
                            .Union(new AssemblyName[] { currentAssembly.GetName() })
                            .Select(a => Path.Combine(path, $"{a.Name}.xml"))
                            .Where(f => File.Exists(f)).ToList();

                        foreach (var xmlDoc in xmlDocs)
                        {
                            setupAction.IncludeXmlComments(xmlDoc);
                        }
                    }

                });

            SetOutputFormatters(services);

            services.AddAuthentication(IISDefaults.AuthenticationScheme);
            services.AddAuthorization();
            services.AddHttpContextAccessor();

        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            IApiVersionDescriptionProvider apiVersionDescriptionProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(appBuilder =>
                {                    
                    appBuilder.Run(async context =>
                    {
                        using var source = new CancellationTokenSource(TimeSpan.FromMinutes(1));
                        var exceptionHandlerPathFeature =
                            context.Features.Get<IExceptionHandlerPathFeature>();

                        context.Response.ContentType = "text/plain";

                        if (exceptionHandlerPathFeature?.Error is ArgumentException ae)
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                            await context.Response
                                .WriteAsync($"{ae.Message} - If you continue getting this error although you are sure the request is correct." +
                                                    $" Please contact the company Team.", source.Token).ConfigureAwait(false);
                        }
                        else
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            await context.Response
                                .WriteAsync("An unexpected fault happened. If you continue getting this error," +
                                            " please contact the company Team.", source.Token).ConfigureAwait(false);
                        }
                    });
                });

                app.UseHttpsRedirection();
            }

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();                
                endpoints.Select().Filter().Expand().MaxTop(20);
                endpoints.MapODataRoute("odata", "odata", GetEdmModel());
            });

            app.UseSwagger();

            app.UseSwaggerUI(
                setupAction =>
                {
                    foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                    {
                        setupAction.SwaggerEndpoint($"{ConfigureSwaggerOptions.LibrarySpecificationPrefix}{description.GroupName}/swagger.json",
                            description.GroupName.ToUpperInvariant());
                    }

                });
        }


        private static IEdmModel GetEdmModel()
        {
            var builder = new ODataConventionModelBuilder();
            builder.EntitySet<GeminiIssueEntity>("projects");
            return builder.GetEdmModel();
        }

        private static void SetOutputFormatters(IServiceCollection services)
        {
            services.AddMvcCore(options =>
            {
                IEnumerable<ODataOutputFormatter> outputFormatters =
                    options.OutputFormatters.OfType<ODataOutputFormatter>()
                        .Where(foramtter => foramtter.SupportedMediaTypes.Count == 0);

                foreach (var outputFormatter in outputFormatters)
                {
                    outputFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/odata"));
                }
            });
        }
    }
}
