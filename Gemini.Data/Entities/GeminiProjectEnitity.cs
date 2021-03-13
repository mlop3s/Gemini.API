using System;

namespace Gemini.Data.Entities
{
    public class GeminiProjectEnitity
    {
        public string ProjectCode { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public string? ProjectLeader { get; set; }
        public decimal ProjectId { get; set; }
        public string ProjectDescription { get; set; } = string.Empty;
        public string ProjectReadonly { get; set; } = string.Empty;
        public string ProjectArchived { get; set; } = string.Empty;
        public decimal ResourceMode { get; set; }
        public decimal ComponentMode { get; set; }
        public decimal GlobalSchemeId { get; set; }
        public decimal IssueTypeSchemeId { get; set; }
        public decimal IssuePrioritySchemeId { get; set; }
        public decimal IssueSeveritySchemeId { get; set; }
        public decimal IssueWorkflowId { get; set; }
        public decimal FieldVisibilitySchemeId { get; set; }
        public decimal UserId { get; set; }
        public DateTime Created { get; set; }
        public decimal Projectlabelid { get; set; }
        public bool Settingdescription { get; set; }
        public byte[] TimeStamp { get; set; } = new byte[1];

    }
}
