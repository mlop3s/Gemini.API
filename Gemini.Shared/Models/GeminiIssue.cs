using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Gemini.Shared.Models
{
    /// <summary>
    /// The gemini issue with all details
    /// </summary>
    public class GeminiIssue
    {
        /// <summary>
        /// Issue's Id
        /// </summary>
        [Key]
        public decimal IssueId { get; set; }

        /// <summary>
        /// Issue's id type
        /// </summary>
        public decimal IssueTypeId { get; set; }

        /// <summary>
        /// Reported by id
        /// </summary>
        public decimal ReportedById { get; set; }

        /// <summary>
        /// Reported by name
        /// </summary>
        public string ReportedByName { get; set; } = string.Empty;

        /// <summary>
        /// The sprint this issue is related to
        /// </summary>
        public string? Sprint { get; set; }

        /// <summary>
        /// Target version of this issue
        /// </summary>
        public string? TargetVersion { get; set; }

        /// <summary>
        /// When it was created
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// When it was accepted
        /// </summary>
        public DateTime? AcceptedDate { get; set; }

        /// <summary>
        /// When it was resolved
        /// </summary>
        public DateTime? ResolvedDate { get; set; }

        /// <summary>
        /// When it was closed
        /// </summary>
        public DateTime? ClosedDate { get; set; }

        /// <summary>
        /// The project code
        /// </summary>
        public string ProjectCode { get; set; } = string.Empty;

        /// <summary>
        /// the project id
        /// </summary>
        public decimal ProjectId { get; set; }

        /// <summary>
        /// The name of the project
        /// </summary>
        public string ProjectName { get; set; } = string.Empty;

        /// <summary>
        /// The summary
        /// </summary>
        public string Summary { get; set; } = string.Empty;

        /// <summary>
        /// The item resolution id
        /// </summary>
        public decimal? IssueResolutionId { get; set; }

        /// <summary>
        /// Ths status id
        /// </summary>
        public decimal IssueStatusId { get; set; }

        /// <summary>
        /// The issue key
        /// </summary>
        public string IssueKey { get; set; } = string.Empty;

        /// <summary>
        /// The fixed in version
        /// </summary>
        public string? Version { get; set; }

        /// <summary>
        /// The gemini issue url
        /// </summary>
        public Uri? IssueUri { get; set; }

        /// <summary>
        /// Description from gemini
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Status from gemini
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// The history items if requested
        /// </summary>
        public ICollection<GeminiIssueHistory>? HistoryItems { get; set; }

        /// <summary>
        /// The custom fields if requested
        /// </summary>
        public ICollection<GeminiCustomField>? CustomFields { get; set; }

    }
}
