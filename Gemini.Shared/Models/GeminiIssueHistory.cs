using System;

namespace Gemini.Shared.Models
{
    /// <summary>
    /// The issue history item
    /// </summary>
    public class GeminiIssueHistory
    {
        /// <summary>
        /// The id of the history
        /// </summary>
        public decimal HistoryId { get; set; }

        /// <summary>
        /// the issue id
        /// </summary>
        public decimal IssueId { get; set; }

        /// <summary>
        /// the project id
        /// </summary>
        public decimal ProjectId { get; set; }

        /// <summary>
        /// the history data
        /// </summary>
        public string History { get; set; } = string.Empty;

        /// <summary>
        /// the usename
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// the date it was added
        /// </summary>
        public DateTime Created { get; set; }
    }
}
