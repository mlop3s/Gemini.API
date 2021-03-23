using Gemini.Data.Entities;
using Gemini.Data.Pagination;
using Gemini.Data.QueryParameters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Gemini.Data.Services
{
    public class GeminiInMemoryRepository : IGeminiRepository
    {
        private readonly List<GeminiIssueEntity> _geminiIssueEntities = new List<GeminiIssueEntity>();
        private readonly List<GeminiCustomFieldEntity> _custom01;
        private readonly List<GeminiIssueHistoryEntity> _history01;

        public GeminiInMemoryRepository()
        {
            _custom01 = new List<GeminiCustomFieldEntity>
            {
                new GeminiCustomFieldEntity
                {
                    IssueId = 5,
                    FieldData = "Some Data",
                    ProjectId = 99,
                    UserId = 5,
                    CustomFieldId = 555,
                    CustomFieldDataId = 666,
                    Created = new DateTime(2020,07,01)
                }

            };

            _history01 = new List<GeminiIssueHistoryEntity>
            {
                new GeminiIssueHistoryEntity
                {
                    Created =  new DateTime(2020,09,01),
                    HistoryId = 777,
                    ProjectId = 99,
                    History= "Some History",
                    IssueId = 5,
                    UserName = "John Robert Doe"
                }
            };

            _geminiIssueEntities.Add(new GeminiIssueEntity
            {
                ClosedDate = new DateTime(2020, 5, 10),
                CreatedDate = new DateTime(2020, 5, 1),
                Description = "Some Description",
                IssueId = 5,
                IssueKey = "GEM-5",
                IssueResolutionId = 3,
                IssueStatusId = 6,
                IssueTypeId = 2,
                ProjectCode = "GEM",
                ProjectId = 99,
                ProjectName = "Gemini",
                ReportedById = 6,
                ReportedByName = "Joh Doe",
                ResolvedDate= new DateTime(2020, 06, 04),
                Status = "Resolved",
                Summary = "Fix odata on this project",
                Version = "Some version"
            });
            _geminiIssueEntities.Add(new GeminiIssueEntity
            {
                CreatedDate = new DateTime(2020, 2, 1),
                Description = "Some Description 2",
                IssueId = 6,
                IssueKey = "GEM-6",
                IssueResolutionId = 3,
                IssueStatusId = 6,
                IssueTypeId = 2,
                ProjectCode = "GEM",
                ProjectId = 99,
                ProjectName = "Gemini",
                ReportedById = 7,
                ReportedByName = "Jane Doe",
                Status = "Open",
                Summary = "Fix documentation",
                Version = "Undocumented version"
            });
        }

        public int SprintCustomFieldId { get; set; }

        public Task<IEnumerable<GeminiIssueEntity>> GetAcceptedIssuesAsync(decimal projectId, IssuesQueryParameters issueQueryParameters, CancellationToken token)
        {
            var json = JsonConvert.SerializeObject(_geminiIssueEntities);
            var list = JsonConvert.DeserializeObject<IEnumerable<GeminiIssueEntity>>(json);

            if (issueQueryParameters.IncludeFields == true)
            {
                foreach (var item in list)
                {
                    item.CustomFields = _custom01;
                }
            }

            if (issueQueryParameters.IncludeHistory == true)
            {
                foreach (var item in list)
                {
                    item.HistoryItems = _history01;
                }
            }

            return Task.FromResult(list);
        }

        public Task<GeminiCustomFieldEntity?> GetCustomDetailAsync(long issueId, long fieldId, CancellationToken token) 
            => Task.FromResult(_custom01.FirstOrDefault(x => x.IssueId == issueId));

        public Task<IDictionary<decimal, List<GeminiCustomFieldEntity>>> GetCustomDetailsAsync(List<decimal> list, CancellationToken token)
            => Task.FromResult<IDictionary<decimal, List<GeminiCustomFieldEntity>>>(
                new Dictionary<decimal, List<GeminiCustomFieldEntity>> { { _custom01.First().ProjectId, _custom01 } });

        public Task<GeminiIssueEntity?> GetIssueAsync(decimal issueId, SingleIssueQueryParameter singleIssueQueryParameter, CancellationToken token)
        {
            var json = JsonConvert.SerializeObject(_geminiIssueEntities.First());
            var issue = JsonConvert.DeserializeObject<GeminiIssueEntity>(json);

            if (singleIssueQueryParameter.IncludeFields == true)
            {
                issue.CustomFields = _custom01;                
            }

            if (singleIssueQueryParameter.IncludeHistory == true)
            {
                issue.HistoryItems = _history01;                
            }

            return Task.FromResult(issue);
        }

        public Task<GeminiIssueEntity?> GetIssueByCodeAsync(
            decimal projectId,
            string issueCode,
            SingleIssueQueryParameter singleIssueQueryParameter,
            CancellationToken token) => GetIssueAsync(projectId, singleIssueQueryParameter, token);

        public Task<IssuesPagedList> GetIssuesAsync(decimal projectId, IssuesQueryParameters issueQueryParameters, CancellationToken token)
        {
            return null;
        }

        public Task<IDictionary<decimal, List<GeminiIssueHistoryEntity>>> GetIssuesHistoriesAsync(
            List<decimal> list,
            CancellationToken token) => Task.FromResult<IDictionary<decimal, List<GeminiIssueHistoryEntity>>>(
            new Dictionary<decimal, List<GeminiIssueHistoryEntity>> { { _history01.First().ProjectId, _history01 } });

        public Task<GeminiIssueHistoryEntity?> GetIssuesHistoryAsync(
            long issueId,
            long historyId,
            CancellationToken token) => Task.FromResult(_history01.FirstOrDefault(x => x.HistoryId == historyId));

        public Task<GeminiProjectEnitity> GetProjectByIdAsync(decimal id, CancellationToken token)
            => Task.FromResult(new GeminiProjectEnitity { ProjectId = 99, ProjectName = "Gemini" });
        

        public IQueryable<GeminiIssueEntity> GetQueryableIssues(long projectId)
        {
            var issues = GetAcceptedIssuesAsync(
                projectId,
                new IssuesQueryParameters { IncludeFields = true, IncludeHistory = true },
                CancellationToken.None)
                    .GetAwaiter()
                    .GetResult();

            return issues.AsQueryable();
        }
        
    }
}
