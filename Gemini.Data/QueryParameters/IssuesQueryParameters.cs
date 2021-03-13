using System;
using System.Linq;

namespace Gemini.Data.QueryParameters
{
    public class IssuesQueryParameters : SingleIssueQueryParameter
    {
        /// <summary>
        /// The max page size
        /// </summary>
        public const int MaxPageSize = 100;

        /// <summary>
        /// The min page size
        /// </summary>
        public const int MinPageSize = 1;

        /// <summary>
        /// The selected year
        /// </summary>
        public int? Year { get; set; }

        /// <summary>
        /// The name of the srpint
        /// </summary>
        public string? Sprint { get; set; }

        /// <summary>
        /// The reporter id
        /// </summary>
        public long? ReporterId { get; set; }

        /// <summary>
        /// The assignee id
        /// </summary>
        public long? AssigneeId { get; set; }

        /// <summary>
        /// Fixed in version
        /// </summary>
        public string? Version { get; set; }

        /// <summary>
        /// Exclude items with a closed date
        /// </summary>
        public bool ExcludeClosed { get; set; }

        /// <summary>
        /// Page number
        /// </summary>
        public string? Page { get; set; }

        private int _pageSize = 1000; // ms sql 2008 does not support paging
        /// <summary>
        /// Page size
        /// </summary>
        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (value <= 0)
                {
                    _pageSize = MinPageSize;
                }
                else
                {
                    _pageSize = value > MaxPageSize ? MaxPageSize : value;
                }
            }
        }
    }
}
