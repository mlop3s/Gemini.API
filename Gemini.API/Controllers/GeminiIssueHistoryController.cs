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
    /// The issue history controller
    /// </summary>
    [Produces("application/json", "application/xml")]
    [Route("api/v{version:apiVersion}/issues/{issueId}/histories")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiController]
    public class GeminiIssueHistoryController : ControllerBase
    {
        private readonly IGeminiRepository _geminiRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="geminiRepository"></param>
        /// <param name="mapper"></param>
        public GeminiIssueHistoryController(IGeminiRepository geminiRepository, IMapper mapper)
        {
            _geminiRepository = geminiRepository ?? throw new ArgumentNullException(nameof(geminiRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Returns all history items for a specifice issue
        /// </summary>
        /// <param name="issueId">The id of the field</param>
        /// <returns></returns>
        /// <response code="200">Returns a list of history items </response>
        [HttpGet]
        [HttpHead]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<GeminiIssueHistory>>> GetGeminiHistories(long issueId)
        {
            var issuesHistoryItems = await _geminiRepository.GetIssuesHistoriesAsync(new List<decimal> { issueId }, CancellationToken.None).ConfigureAwait(false);
            if (!issuesHistoryItems.Any())
            {
                return NotFound();
            }

            var items = issuesHistoryItems.Single().Value;
            return Ok(_mapper.Map<IEnumerable<GeminiIssueHistory>>(items));
        }

        /// <summary>
        /// Returns a specific history id for an issue
        /// </summary>
        /// <param name="issueId">The id of the issue</param>
        /// <param name="historyId">The id of the history item</param>
        /// <returns></returns>
        /// <response code="200">Returns the history item</response>
        [HttpGet("{historyId}")]
        [HttpHead("{historyId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<GeminiIssueHistory>> GetGeminiHistory(long issueId, long historyId)
        {
            var issuesHistoryItem = await _geminiRepository.GetIssuesHistoryAsync(issueId, historyId, CancellationToken.None).ConfigureAwait(false);
            if (issuesHistoryItem == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<GeminiIssueHistory>(issuesHistoryItem));
        }
    }
}
