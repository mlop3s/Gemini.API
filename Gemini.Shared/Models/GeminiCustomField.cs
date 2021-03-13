using System;

namespace Gemini.Shared.Models
{
    /// <summary>
    /// The Gemini custom field, with issue id, user id and data
    /// </summary>
    public class GeminiCustomField
    {
        /// <summary>
        /// The id of this custom data
        /// </summary>
        public decimal CustomFieldDataId { get; set; }

        /// <summary>
        /// The if of this cusom id type
        /// </summary>
        public decimal CustomFieldId { get; set; }

        /// <summary>
        /// The user id
        /// </summary>
        public decimal UserId { get; set; }

        /// <summary>
        /// The project id
        /// </summary>
        public decimal ProjectId { get; set; }

        /// <summary>
        /// The issue id
        /// </summary>
        public decimal IssueId { get; set; }

        /// <summary>
        /// The data of this field
        /// </summary>
        public string? FieldData { get; set; } = string.Empty;

        /// <summary>
        /// The date the field was created
        /// </summary>
        public DateTime Created { get; set; }
    }
}
