using Gemini.Data.Entities;
using Gemini.Data.Pagination;
using Gemini.Data.QueryParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Gemini.Data.Services
{
    public interface IGeminiRepository
    {
        int SprintCustomFieldId { get; set; }
        Task<GeminiIssueEntity?> GetIssueAsync(decimal issueId, SingleIssueQueryParameter singleIssueQueryParameter, CancellationToken token);
        Task<GeminiIssueEntity?> GetIssueByCodeAsync(decimal projectId, string issueCode, SingleIssueQueryParameter singleIssueQueryParameter, CancellationToken token);
        Task<IssuesPagedList> GetIssuesAsync(decimal projectId, IssuesQueryParameters issueQueryParameters, CancellationToken token);
        Task<IEnumerable<GeminiIssueEntity>> GetAcceptedIssuesAsync(decimal projectId, IssuesQueryParameters issueQueryParameters, CancellationToken token);
        Task<IDictionary<decimal, List<GeminiIssueHistoryEntity>>> GetIssuesHistoriesAsync(List<decimal> list, CancellationToken token);
        Task<IDictionary<decimal, List<GeminiCustomFieldEntity>>> GetCustomDetailsAsync(List<decimal> list, CancellationToken token);
        Task<GeminiIssueHistoryEntity?> GetIssuesHistoryAsync(long issueId, long historyId, CancellationToken token);
        Task<GeminiCustomFieldEntity?> GetCustomDetailAsync(long issueId, long fieldId, CancellationToken token);
        Task<GeminiProjectEnitity> GetProjectByIdAsync(decimal id, CancellationToken token);
        IQueryable<GeminiIssueEntity> GetQueryableIssues(long projectId);
    }
}
