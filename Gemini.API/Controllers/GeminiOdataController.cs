using Gemini.Data.Entities;
using Gemini.Data.Services;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using static Microsoft.AspNetCore.Http.StatusCodes;
using static Microsoft.AspNet.OData.Query.AllowedQueryOptions;
using Microsoft.OData;

namespace Gemini.API.Controllers
{
    /// <summary>
    /// The gemini issue controller
    /// </summary>
    [Route("odata")]
    [ApiController]
    public class GeminiOdataController : ODataController
    {
        private readonly IGeminiRepository _geminiRepository;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="geminiRepository"></param>
        public GeminiOdataController(IGeminiRepository geminiRepository)
        {
            _geminiRepository = geminiRepository ?? throw new ArgumentNullException(nameof(geminiRepository));
        }

        /// <summary>
        /// Gets a list of issues for a certain project and year (query).
        /// </summary>
        /// <param name="projectId">The project's id</param>
        /// <param name="options">The current OData query options.</param>
        /// <returns></returns>
        /// <response code="200">Returns a list of issues </response>
        [HttpGet("projects/{projectId}/issues", Name = "GetOdataGeminiIssues")]
        [HttpHead("projects/{projectId}/issues", Name = "GetOdataGeminiIssues")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ODataValue<IQueryable<GeminiIssueEntity>>), Status200OK)]
        [ProducesResponseType(Status400BadRequest)]
        public IActionResult GetGeminiIssues(long projectId, ODataQueryOptions<GeminiIssueEntity> options)
        {
            var validationSettings = new ODataValidationSettings()
            {
                AllowedQueryOptions = Select | OrderBy | Top | Skip | Count | Filter | Expand,
                AllowedArithmeticOperators = AllowedArithmeticOperators.None,
                AllowedFunctions = AllowedFunctions.None,
                AllowedLogicalOperators = AllowedLogicalOperators.All,
                MaxTop = 20,                
            };

            validationSettings.AllowedOrderByProperties.Add(nameof(GeminiIssueEntity.IssueId));

            try
            {
                options.Validate(validationSettings);
            }
            catch (ODataException d)
            {
                return BadRequest(d.Message);
            }

            var issues = _geminiRepository.GetQueryableIssues(projectId);
            var result = options.ApplyTo(issues);
            return Ok(result);
        }
    }
}
