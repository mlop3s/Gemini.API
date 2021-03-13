using System;

namespace Gemini.Data.Entities
{
    public class GeminiIssueHistoryEntity
    {
        public decimal HistoryId { get; set; }

        public decimal IssueId { get; set; }

        public decimal ProjectId { get; set; }

        public string History { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public DateTime Created { get; set; }
    }
}
