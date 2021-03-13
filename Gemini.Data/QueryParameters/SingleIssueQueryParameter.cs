using System;

namespace Gemini.Data.QueryParameters
{
    public class SingleIssueQueryParameter
    {
        /// <summary>
        /// If the issue should include all custom fields
        /// </summary>
        public bool? IncludeFields { get; set; }

        /// <summary>
        /// |If the issue should include all history items
        /// </summary>
        public bool? IncludeHistory { get; set; }
    }
}
