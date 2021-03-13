using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Gemini.Data.Entities
{
    public class GeminiIssueEntity
    {
        [Key]
        public decimal IssueId { get; set; }

        public decimal IssueTypeId { get; set; }

        public decimal ReportedById { get; set; }

        public string ReportedByName { get; set; } = string.Empty;

        public DateTime Created { get; set; }

        public DateTime? ResolvedDate { get; set; }

        public DateTime? ClosedDate { get; set; }

        public string ProjectCode { get; set; } = string.Empty;

        public decimal ProjectId { get; set; }

        public string ProjectName { get; set; } = string.Empty;

        public string Summary { get; set; } = string.Empty;

        public string? Description { get; set; }

        public decimal? IssueResolutionId { get; set; }

        public decimal IssueStatusId { get; set; }

        public string IssueKey { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public string? Version { get; set; }

        public ICollection<GeminiIssueHistoryEntity> HistoryItems { get; set; } = new List<GeminiIssueHistoryEntity>();

        public ICollection<GeminiCustomFieldEntity> CustomFields { get; set; } = new List<GeminiCustomFieldEntity>();

    }
}
