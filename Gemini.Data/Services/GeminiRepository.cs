using Gemini.Data.DbContexts;
using Gemini.Data.Entities;
using Gemini.Data.Extensions;
using Gemini.Data.Pagination;
using Gemini.Data.QueryParameters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Gemini.Data.Services
{
    public sealed class GeminiRepository : IDisposable, IGeminiRepository
    {
        private readonly GeminiContext _context;
        private bool _disposedValue;

        public int SprintCustomFieldId { get; set; } = 256;

        public GeminiRepository(GeminiContext geminiContext)
        {
            _context = geminiContext ?? throw new ArgumentNullException(nameof(geminiContext));
        }

        public Task<GeminiIssueEntity?> GetIssueAsync(
            decimal issueId,
            SingleIssueQueryParameter singleIssueQueryParameter, CancellationToken token)
        {
            if (issueId <= 0)
            {
                throw new ArgumentNullException(nameof(issueId));
            }

            if (singleIssueQueryParameter is null)
            {
                throw new ArgumentNullException(nameof(singleIssueQueryParameter));
            }

            return InternalGetIssueAsync(issueId, singleIssueQueryParameter, token); // we make sure we are not creating a state machine here
        }

        private async Task<GeminiIssueEntity?> InternalGetIssueAsync(
            decimal issueId,
            SingleIssueQueryParameter singleIssueQueryParameter, CancellationToken token)
        {
            var issue = await _context.Issues
              .Where(c => c.IssueId == issueId).FirstOrDefaultAsync(token).ConfigureAwait(false);

            if (issue == null)
            {
                return issue;
            }

            if (singleIssueQueryParameter.IncludeFields == true)
            {
                issue.CustomFields = await _context.CustomFields.Where(s => s.IssueId == issueId).ToListAsync(token).ConfigureAwait(false);
            }

            if (singleIssueQueryParameter.IncludeHistory == true)
            {
                issue.HistoryItems = await _context.IssueHistory.Where(s => s.IssueId == issueId).ToListAsync(token).ConfigureAwait(false);
            }

            return issue;
        }

        public Task<GeminiIssueEntity?> GetIssueByCodeAsync(
            decimal projectId,
            string issueCode,
            SingleIssueQueryParameter singleIssueQueryParameter, CancellationToken token)
        {
            if (projectId <= 0)
            {
                throw new ArgumentNullException(nameof(projectId));
            }

            if (string.IsNullOrWhiteSpace(issueCode))
            {
                throw new ArgumentNullException(nameof(issueCode));
            }

            if (singleIssueQueryParameter is null)
            {
                throw new ArgumentNullException(nameof(singleIssueQueryParameter));
            }

            return InternalGetIssueByCodeAsync(projectId, issueCode, singleIssueQueryParameter, token);
        }

        private async Task<GeminiIssueEntity?> InternalGetIssueByCodeAsync(
            decimal projectId,
            string issueCode,
            SingleIssueQueryParameter singleIssueQueryParameter, CancellationToken token)
        {
            var issue = await _context.Issues
              .Where(c => c.ProjectId == projectId && c.IssueKey == issueCode)
              .FirstOrDefaultAsync(token)
              .ConfigureAwait(false);

            if (issue == null)
            {
                return issue;
            }

            var issueId = issue.IssueId;

            if (singleIssueQueryParameter.IncludeFields == true)
            {
                issue.CustomFields = await _context.CustomFields.Where(h => h.IssueId == issueId).ToListAsync(token).ConfigureAwait(false);
            }

            if (singleIssueQueryParameter.IncludeHistory == true)
            {
                issue.HistoryItems = await _context.IssueHistory.Where(s => s.IssueId == issueId).ToListAsync(token).ConfigureAwait(false);
            }

            return issue;
        }

        public Task<IssuesPagedList> GetIssuesAsync(decimal projectId, IssuesQueryParameters issueQueryParameters, CancellationToken token)
        {
            if (projectId <= 0)
            {
                throw new ArgumentNullException(nameof(projectId));
            }

            if (issueQueryParameters is null)
            {
                throw new ArgumentNullException(nameof(issueQueryParameters));
            }

            var createdYear = issueQueryParameters.Year ?? DateTime.Now.Year;
            if (createdYear < 2010 || createdYear > DateTime.Now.Year)
            {
                throw new ArgumentNullException(nameof(issueQueryParameters));
            }

            return InternalGetIssuesAsync(projectId, issueQueryParameters, createdYear, token);
        }

        private async Task<IssuesPagedList> InternalGetIssuesAsync(
            decimal projectId,
            IssuesQueryParameters issueQueryParameters,
            int createdYear,
            CancellationToken token)
        {
            var query = _context.Issues.Where(c => c.ProjectId == projectId && c.Created.Year == createdYear);

            if (issueQueryParameters.ExcludeClosed)
            {
                query.Where(x => x.ClosedDate == null);
            }

            var list = await IssuesPagedList.CreateAsync(query, issueQueryParameters, token).ConfigureAwait(false);
            var ids = list.Select(x => x.IssueId).ToList();
            if (ids.Any())
            {                
                await IncludeCustomFieldsAsync(ids, list, token).ConfigureAwait(false);
                if (issueQueryParameters.HasIncludes())
                {
                    await IncludeHistoryAsync(ids, list, token).ConfigureAwait(false);
                }
            }

            return list;
        }

        private async Task IncludeCustomFieldsAsync(
            List<decimal> ids,
            List<GeminiIssueEntity> list,
            CancellationToken token)
        {
            var dictCustom = await GetCustomDetailsAsync(ids, token).ConfigureAwait(false);

            foreach (var issue in list)
            {
                if (dictCustom.ContainsKey(issue.IssueId))
                {
                    issue.CustomFields = dictCustom[issue.IssueId];
                }
            }
    }

        private async Task IncludeHistoryAsync(
            List<decimal> ids,
            List<GeminiIssueEntity> list,
            CancellationToken token)
        {
            var dictHistory = await GetIssuesHistoriesAsync(ids, token).ConfigureAwait(false);

            foreach (var issue in list)
            {
                if (dictHistory.ContainsKey(issue.IssueId))
                {
                    issue.HistoryItems = dictHistory[issue.IssueId];
                }
            }
        }

        public Task<IEnumerable<GeminiIssueEntity>> GetAcceptedIssuesAsync(
            decimal projectId,
            IssuesQueryParameters issueQueryParameters,
            CancellationToken token)
        {
            const int sprintField = 256;

            if (projectId <= 0)
            {
                throw new ArgumentNullException(nameof(projectId));
            }

            if (issueQueryParameters is null)
            {
                throw new ArgumentNullException(nameof(issueQueryParameters));
            }

            var createdYear = issueQueryParameters.Year ?? DateTime.Now.Year;
            if (createdYear < 2010 || createdYear > DateTime.Now.Year)
            {
                throw new ArgumentException("Invalid data", nameof(issueQueryParameters));
            }

            var dateBegin = new DateTime(createdYear, 1, 1);
            var dateEnd = new DateTime(createdYear + 1, 1, 1).AddSeconds(-1);

            return InternalGetAcceptedIssues(projectId, sprintField, issueQueryParameters, dateBegin, dateEnd, token);

        }

        private async Task<IEnumerable<GeminiIssueEntity>> InternalGetAcceptedIssues(
            decimal projectId,
            int sprintField,
            IssuesQueryParameters issueQueryParameters,
            DateTime dateBegin,
            DateTime dateEnd,
            CancellationToken token)
        {
            var sprint = issueQueryParameters.Sprint is null ? (int?)null : int.Parse(issueQueryParameters.Sprint);
            var sprintBegin = sprint ?? 0;
            var sprintEnd = sprint ?? 99999;            

            var list = _context.Issues.FromSqlInterpolated($@"
select g.issueid, g.created, g.resolveddate, g.issuetypeid,
       g.reporteddesc, g.reportedby, g.projectcode, g.summary, g.closeddate, g.issuestatusid,
       g.issueresolutionid, g.issuekey, g.projectid, g.projectname, g.versiondesc, g.versionname, g.versionreleased 
from gemini_issuesview g
join gemini_issuehistory a on a.IssueId = g.issueid
join gemini_customfielddata d on d.IssueId = g.issueid
where g.projectid = {projectId} and a.history = 'Issue status changed to angenommen' and a.created  between {dateBegin} and {dateEnd}
and (d.customfieldid = {sprintField} and (d.fielddata is null or (CONVERT(INT, d.fielddata) >= {sprintBegin} and CONVERT(INT, d.fielddata) <= {sprintEnd}))")
                .ToList();

            var ids = list.Select(x => x.IssueId).ToList();

            if (ids.Any())
            {
                await IncludeHistoryAsync(ids, list, token).ConfigureAwait(false);
            }

            return list;
        }

        public Task<IDictionary<decimal, List<GeminiCustomFieldEntity>>> GetCustomDetailsAsync(List<decimal> list, CancellationToken token)
        {
            if (list is null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            if (!list.Any())
            {
                return Task.FromResult<IDictionary<decimal, List<GeminiCustomFieldEntity>>>(
                    new Dictionary<decimal, List<GeminiCustomFieldEntity>>());
            }

            return InternalGetCustomDetailsAsync(list, token);
        }

        private async Task<IDictionary<decimal, List<GeminiCustomFieldEntity>>> InternalGetCustomDetailsAsync(List<decimal> list, CancellationToken token)
        {
            var items = await _context.CustomFields.Where(h => list.Contains(h.IssueId)).ToListAsync(token).ConfigureAwait(false);
            return items.GroupBy(k => k.IssueId).ToDictionary(d => d.Key, d => d.ToList());
        }

        public Task<GeminiCustomFieldEntity?> GetCustomDetailAsync(long issueId, long fieldId, CancellationToken token)
        {
            if (issueId <= 0)
            {
                throw new ArgumentNullException(nameof(issueId));
            }

            if (fieldId <= 0)
            {
                throw new ArgumentNullException(nameof(fieldId));
            }

            return _context.CustomFields
                .FirstOrDefaultAsync(h => h.CustomFieldDataId == fieldId && h.IssueId == issueId, token); // https://github.com/dotnet/efcore/issues/19443
        }

        public Task<IDictionary<decimal, List<GeminiIssueHistoryEntity>>> GetIssuesHistoriesAsync(List<decimal> list, CancellationToken token)
        {
            if (list is null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            if (!list.Any())
            {
                return Task.FromResult<IDictionary<decimal, List<GeminiIssueHistoryEntity>>>(
                    new Dictionary<decimal, List<GeminiIssueHistoryEntity>>());
            }

            return InternalGetIssuesHistoriesAsync(list, token);
        }

        private async Task<IDictionary<decimal, List<GeminiIssueHistoryEntity>>> InternalGetIssuesHistoriesAsync(List<decimal> list, CancellationToken token)
        {
            var items = await _context.IssueHistory.Where(h => list.Contains(h.IssueId)).ToListAsync(token).ConfigureAwait(false);
            return items.GroupBy(k => k.IssueId).ToDictionary(d => d.Key, d => d.ToList());
        }

        public Task<GeminiIssueHistoryEntity?> GetIssuesHistoryAsync(long issueId, long historyId, CancellationToken token)
        {
            if (issueId <= 0)
            {
                throw new ArgumentNullException(nameof(issueId));
            }

            if (historyId <= 0)
            {
                throw new ArgumentNullException(nameof(historyId));
            }

            return _context.IssueHistory
                .FirstOrDefaultAsync(h => h.HistoryId == historyId && h.IssueId == issueId); // https://github.com/dotnet/efcore/issues/19443
        }

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _context.Dispose();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public Task<GeminiProjectEnitity> GetProjectByIdAsync(decimal id, CancellationToken token)
        {
            if (id <= 0)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return _context.Projects
                .FirstOrDefaultAsync(h => h.ProjectId == id, token); // https://github.com/dotnet/efcore/issues/19443
        }

        public IQueryable<GeminiIssueEntity> GetQueryableIssues(long projectId)
        {
            return _context.Issues.Where(x => x.ProjectId == projectId).AsQueryable();
        }
    }
}
