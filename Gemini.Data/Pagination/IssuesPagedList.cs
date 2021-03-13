using Gemini.Data.Entities;
using Gemini.Data.QueryParameters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Gemini.Data.Pagination
{
    public class IssuesPagedList : List<GeminiIssueEntity>
    {
        public IssuePagination IssuePagination { get; }

        IssuesPagedList(List<GeminiIssueEntity> items, int count, MetaIssuePagination metaIssuePagination)
        {
            IssuePagination = CreateIssuePagination(count, metaIssuePagination);
            AddRange(items);
        }

        public static async Task<IssuesPagedList> CreateAsync(
            IQueryable<GeminiIssueEntity> source,
            IssuesQueryParameters issuesQueryParameters,
            CancellationToken token)
        {

            var count = source.Count();
            var requestedIssuePagination = MetaIssuePagination.ParsePagination(issuesQueryParameters);
            int pageNumber = 0;

            List<GeminiIssueEntity> items;
            if (requestedIssuePagination != null)
            {
                pageNumber = Math.Max(1, requestedIssuePagination.PageNumber);
                items = await GetPageAsync(source, issuesQueryParameters.PageSize, pageNumber, token).ConfigureAwait(false);
            }
            else
            {
                items = await source.Take(issuesQueryParameters.PageSize).ToListAsync(token).ConfigureAwait(false);
            }

            var meta = new MetaIssuePagination
            {
                PageNumber = pageNumber,
                PageSize = issuesQueryParameters.PageSize,
                CheckSum = CreateCheckSum(issuesQueryParameters)
            };

            return new IssuesPagedList(items, count, meta);
        }

        private static Task<List<GeminiIssueEntity>> GetPageAsync(
            IQueryable<GeminiIssueEntity> source,
            int pageSize,
            int pageNumber,
            CancellationToken token)
        {
            if (pageNumber > 0)
            {
                return source.Skip(pageNumber * pageSize).Take(pageSize).ToListAsync(token);
            }
            else
            {
                return source.Take(pageSize).ToListAsync(token);
            }
        }


        private static IssuePagination CreateIssuePagination(int count, MetaIssuePagination metaIssuePagination)
        {
            var totalPages = (int)Math.Ceiling(count / (double)metaIssuePagination.PageSize);

            var issuePagination = new IssuePagination
            {
                Count = count,
                CurrentPage = MetaIssuePagination.Create(metaIssuePagination, totalPages, PageDirection.Current),
                NextPage = MetaIssuePagination.Create(metaIssuePagination, totalPages, PageDirection.Next),
                PreviousPage = MetaIssuePagination.Create(metaIssuePagination, totalPages, PageDirection.Previous),
                TotalPages = totalPages
            };

            return issuePagination;
        }

        private static int CreateCheckSum(IssuesQueryParameters issuesQueryParameters)
        {
            return (issuesQueryParameters.Version?.GetHashCode() ?? 71) ^
                (issuesQueryParameters.AssigneeId?.GetHashCode() ?? 173) ^
                (issuesQueryParameters.ReporterId?.GetHashCode() ?? 1069) ^
                (issuesQueryParameters.Sprint?.GetHashCode() ?? 4231) ^
                (issuesQueryParameters.Year?.GetHashCode() ?? 6997);
        }
    }
}
