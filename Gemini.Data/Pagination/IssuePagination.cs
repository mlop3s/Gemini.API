using System;

namespace Gemini.Data.Pagination
{
    public class IssuePagination
    {
        public string? NextPage { get; set; }
        public string? PreviousPage { get; set; }
        public string? CurrentPage { get; set; }
        public int Count { get; set; }
        public int TotalPages { get; set; }
    }
}
