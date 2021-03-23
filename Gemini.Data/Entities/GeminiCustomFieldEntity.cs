using System;
using System.ComponentModel.DataAnnotations;

namespace Gemini.Data.Entities
{
    public class GeminiCustomFieldEntity
    {
        [Key]
        public decimal CustomFieldDataId { get; set; }

        public decimal CustomFieldId { get; set; }

        public decimal UserId { get; set; }

        public decimal ProjectId { get; set; }

        public decimal IssueId { get; set; }

        public string? FieldData { get; set; } = string.Empty;

        public decimal? NumericData { get; set; }

        public DateTime Created { get; set; }
    }
}
