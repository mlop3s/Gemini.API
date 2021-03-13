using AutoMapper;
using Gemini.API.Helpers;
using Gemini.Data.Extensions;
using Gemini.Data.QueryParameters;
using Gemini.Data.Services;
using Gemini.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Gemini.API.Controllers
{
    /// <summary>
    /// The gemini issue controller
    /// </summary>
    [Produces("application/json", "application/xml")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Route("api/v{version:apiVersion}")]
    [ApiController]
    public class GeminiIssuesController : ControllerBase
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
        public GeminiIssuesController(IGeminiRepository geminiRepository, IMapper mapper, IConfiguration configuration)
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
        /// Gets a list of issues for a certain project and year (query). Default year is current year.
        /// </summary>
        /// <param name="projectId">The project's id</param>
        /// <param name="issueQueryParameters">The query parameters</param>
        /// <returns></returns>
        /// <response code="200">Returns a list of issues </response>
        [HttpGet("projects/{projectId}/issues", Name = "GetGeminiIssues")]
        [HttpHead("projects/{projectId}/issues")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<GeminiIssue>>> GetGeminiIssues(long projectId,
            [FromQuery] IssuesQueryParameters issueQueryParameters)
        {
            var issues = await _geminiRepository.GetIssuesAsync(projectId, issueQueryParameters, CancellationToken.None).ConfigureAwait(false);

            var previousPageLink = issues.IssuePagination.HasPrevious() ?
                CreateResourceUri(issues.IssuePagination.PreviousPage, issueQueryParameters) : null;

            var nextPageLink = issues.IssuePagination.HasNext() ?
                CreateResourceUri(issues.IssuePagination.NextPage, issueQueryParameters) : null;

            var paginationMetadata = new
            {
                totalCount = issues.IssuePagination.Count,
                pageSize = issueQueryParameters.PageSize,
                previousPageLink,
                nextPageLink
            };

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetadata));

            // TODO: This destoys the pagination - but this SQL SEVER does not support it anyway
            if (!string.IsNullOrWhiteSpace(issueQueryParameters.Sprint) && int.TryParse(issueQueryParameters.Sprint, out int sprint))
            {
                var sprintIssues = issues.Where(x => x.IsInSprint(sprint, _geminiRepository.SprintCustomFieldId));
                return Ok(_mapper.Map<IEnumerable<GeminiIssue>>(sprintIssues));
            }

            var geminiIssues = _mapper.Map<IEnumerable<GeminiIssue>>(issues);
            foreach (var item in geminiIssues)
            {
                item.IssueUri = _geminiUrlHelper.BuilIssuedUri(item);
            }

            return Ok(geminiIssues);
        }

        /// <summary>
        /// Gets a list of accepted issues for a certain project and year (query). Default year is current year.
        /// </summary>
        /// <param name="projectId">The project's id</param>
        /// <param name="issueQueryParameters">The query parameter</param>
        /// <returns></returns>
        /// <response code="200">Returns a list of accpeted issues</response>
        [HttpGet("projects/{projectId}/acceptedIssues")]
        [HttpHead("projects/{projectId}/acceptedIssues")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<GeminiIssue>>> GetGeminiAcceptedIssues(long projectId,
            [FromQuery] IssuesQueryParameters issueQueryParameters)
        {
            var accepted = await _geminiRepository.GetAcceptedIssuesAsync(projectId, issueQueryParameters, CancellationToken.None).ConfigureAwait(false);
            var issues = accepted.ToList();

            if (!issues.Any())
            {
                return Ok(_mapper.Map<IEnumerable<GeminiIssue>>(issues));
            }

            var ids = issues.Select(s => s.IssueId).ToList();
            var history = await _geminiRepository.GetIssuesHistoriesAsync(ids, CancellationToken.None).ConfigureAwait(false);

            var geminiIssues = _mapper.Map<IEnumerable<GeminiIssue>>(issues);

            foreach (var issue in geminiIssues)
            {
                if (history.ContainsKey(issue.IssueId))
                {
                    var acceptedItem = history.Single(x => x.Key == issue.IssueId).Value
                        .FirstOrDefault(
                            c => c.History.IndexOf("angenommen", StringComparison.InvariantCultureIgnoreCase) >= 0);

                    issue.IssueUri = _geminiUrlHelper.BuilIssuedUri(issue);
                    issue.AcceptedDate = acceptedItem?.Created;
                }
            }

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize("Not Supported"));

            return Ok(geminiIssues);
        }

        /// <summary>
        /// Returns an issue by code for a certain project
        /// </summary>
        /// <param name="projectId">The project's id</param>
        /// <param name="code">The issue code</param>
        /// <param name="singleIssueQueryParameter">The query parameters</param>
        /// <returns></returns>
        /// <response code="200">Returns the requested issue </response>
        /// 
        [HttpGet("projects/{projectId}/issues/{code}")]
        [HttpHead("projects/{projectId}/issues/{code}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<GeminiIssue>> GetGeminiIssueByCode(
            long projectId,
            string code,
            [FromQuery] SingleIssueQueryParameter singleIssueQueryParameter)
        {
            var issue = await _geminiRepository.GetIssueByCodeAsync(projectId, code, singleIssueQueryParameter, CancellationToken.None).ConfigureAwait(false);

            if (issue == null)
            {
                return NotFound();
            }

            var item = _mapper.Map<GeminiIssue>(issue);
            item.IssueUri = _geminiUrlHelper.BuilIssuedUri(item);

            return Ok(item);
        }

        /// <summary>
        /// Returns an issue by id for a certain project
        /// </summary>
        /// <param name="issueId">The issue's id</param>
        /// <param name="singleIssueQueryParameter">The query parameters</param>
        /// <returns></returns>
        /// <response code="200">Returns the requested issue </response>
        [HttpGet("issues/{issueId}")]
        [HttpHead("issues/{issueId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<GeminiIssue>> GetGeminiIssueById(
            long issueId,
            [FromQuery] SingleIssueQueryParameter singleIssueQueryParameter)
        {

            var issue = await _geminiRepository.GetIssueAsync(issueId, singleIssueQueryParameter, CancellationToken.None).ConfigureAwait(false);

            if (issue == null)
            {
                return NotFound();
            }

            var item = _mapper.Map<GeminiIssue>(issue);
            item.IssueUri = _geminiUrlHelper.BuilIssuedUri(item);

            if (singleIssueQueryParameter.IncludeFields != true)
            {
                var fields = await _geminiRepository.GetCustomDetailsAsync(new List<decimal> { issueId }, CancellationToken.None).ConfigureAwait(false);
                var custom = fields.FirstOrDefault();
                if (fields.Any() && custom.Value.Any(x => x.CustomFieldId == _geminiRepository.SprintCustomFieldId))
                {
                    var sprint = custom.Value.First(c => c.CustomFieldId == _geminiRepository.SprintCustomFieldId).NumericData;
                    item.Sprint = sprint is not null
                        ? decimal.ToInt32(sprint.Value).ToString(CultureInfo.InvariantCulture)
                        : string.Empty;
                }
            }

            return Ok(item);
        }

        private string CreateResourceUri(string? page, IssuesQueryParameters issuesQueryParameters)
        {
            return Url.Link("GetGeminiIssues", new IssuesQueryParameters
            {
                AssigneeId = issuesQueryParameters.AssigneeId,
                IncludeFields = issuesQueryParameters.IncludeFields,
                IncludeHistory = issuesQueryParameters.IncludeHistory,
                Page = page,
                PageSize = issuesQueryParameters.PageSize,
                ReporterId = issuesQueryParameters.ReporterId,
                Sprint = issuesQueryParameters.Sprint,
                Version = issuesQueryParameters.Version,
                Year = issuesQueryParameters.Year
            });
        }
    }
}
