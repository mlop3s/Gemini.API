using Gemini.Data.Pagination;
using System;

namespace Gemini.Data.Extensions
{
    public static class PaginationExtensions
    {
        public static bool HasNext(this IssuePagination issuePagination) => !string.IsNullOrEmpty(issuePagination?.NextPage);
        public static bool HasPrevious(this IssuePagination issuePagination) => !string.IsNullOrEmpty(issuePagination?.PreviousPage);
    }
}
