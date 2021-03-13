using AutoMapper;
using Gemini.API.Helpers;
using Gemini.Data.Services;
using Gemini.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Gemini.API.Controllers
{
    /// <summary>
    /// Provides details from gemini
    /// </summary>
    [Produces("application/json", "application/xml")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Route("api/v{version:apiVersion}")]
    [ApiController]
    public class GeminiProjectController : ControllerBase
    {
        private readonly GeminiUrlHelper _geminiUrlHelper;
        private readonly IGeminiRepository _geminiRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="geminiRepository"></param>
        /// <param name="mapper"></param>
        /// <param name="configuration"></param>
        public GeminiProjectController(IGeminiRepository geminiRepository, IMapper mapper, IConfiguration configuration)
        {
            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var geminiViewLink = new Uri(configuration["GeminUri"], UriKind.Absolute);
            _geminiUrlHelper = new GeminiUrlHelper(geminiViewLink);
            _geminiRepository = geminiRepository ?? throw new ArgumentNullException(nameof(geminiRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

            if (int.TryParse(configuration["SprintFieldId"], out int fieldID))
            {
                _geminiRepository.SprintCustomFieldId = fieldID;
            }
        }

        /// <summary>
        /// Returns a project
        /// </summary>
        /// <param name="projectId">The project's id</param>
        /// <returns></returns>
        /// <response code="200">Returns the requested issue </response>
        [HttpGet("projects/{projectId}")]
        [HttpHead("projects/{projectId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<GeminiProject>> GetGeminiProject(long projectId)
        {
            var project = await _geminiRepository.GetProjectByIdAsync(projectId, CancellationToken.None).ConfigureAwait(false);

            if (project == null)
            {
                return NotFound();
            }

            var geminiProject = _mapper.Map<GeminiProject>(project);
            geminiProject.ProjectUri = _geminiUrlHelper.BuildProjectUri(geminiProject);

            return Ok(geminiProject);
        }
    }
}
