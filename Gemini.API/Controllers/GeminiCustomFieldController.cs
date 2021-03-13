using AutoMapper;
using Gemini.Data.Services;
using Gemini.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Gemini.API.Controllers
{
    /// <summary>
    /// Custom field controller
    /// </summary>
    [Route("api/v{version:apiVersion}/issues/{issueId}/fields")]
    [ApiController]
    [Produces("application/json", "application/xml")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class GeminiCustomFieldController : ControllerBase
    {
        private readonly IGeminiRepository _geminiRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="geminiRepository"></param>
        /// <param name="mapper"></param>
        public GeminiCustomFieldController(IGeminiRepository geminiRepository, IMapper mapper)
        {
            _geminiRepository = geminiRepository ?? throw new ArgumentNullException(nameof(geminiRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Get all history fields connected to an issue
        /// </summary>
        /// <param name="issueId">The Gemini id of the field</param>
        /// <param name="fieldTypeId">A filter that defines the type of the field to retrieve</param>
        /// <returns>An ActionResult containning the custom fields - Async</returns>
        /// <response code="200">Returns the list of custome fields if any</response>
        [HttpGet]
        [HttpHead]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<GeminiCustomField>>> GetGeminiCustomFields(long issueId, [FromQuery] long? fieldTypeId)
        {
            var issueFields = await _geminiRepository.GetCustomDetailsAsync(new List<decimal> { issueId }, CancellationToken.None).ConfigureAwait(false); 
            if (!issueFields.Any())
            {
                return NotFound();
            }

            var items = fieldTypeId == null
                ? issueFields.Single().Value
                : issueFields.Single().Value.Where(s => s.CustomFieldId == fieldTypeId);

            return Ok(_mapper.Map<IEnumerable<GeminiCustomField>>(items));
        }

        /// <summary>
        /// Get a history fields connected to an issue
        /// </summary>
        /// <param name="issueId">The issue's id</param>
        /// <param name="fieldId">The id for the field</param>
        /// <returns></returns>
        /// <response code="200">Returns the custom field</response>
        [HttpGet("{fieldId}")]
        [HttpHead("{fieldId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<GeminiCustomField>> GetGeminiCustomField(long issueId, long fieldId)
        {
            var detail = await _geminiRepository.GetCustomDetailAsync(issueId, fieldId, CancellationToken.None).ConfigureAwait(false);
            if (detail == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<GeminiCustomField>(detail));
        }
    }
}
