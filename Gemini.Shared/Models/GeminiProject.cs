using System;

namespace Gemini.Shared.Models
{
    /// <summary>
    /// Representa the details of the gemini project
    /// </summary>
    public class GeminiProject
    {
        /// <summary>
        /// The project id
        /// </summary>
        public decimal ProjectId { get; set; }

        /// <summary>
        /// The name of the project
        /// </summary>
        public string ProjectName { get; set; } = string.Empty;

        /// <summary>
        /// The code of the project like NXMED
        /// </summary>
        public string ProjectCode { get; set; } = string.Empty;

        /// <summary>
        /// Project's description
        /// </summary>
        public string ProjectDescription { get; set; } = string.Empty;

        /// <summary>
        /// The date when the project was created
        /// </summary>
        public DateTime Created { get; set; } = new DateTime(1900, 1, 1);

        /// <summary>
        /// The project url
        /// </summary>
        public Uri? ProjectUri { get; set; }
    }
}
