using Gemini.Shared.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Gemini.API.Helpers
{
    /// <summary>
    /// Helps buiding url within this service
    /// </summary>
    public class GeminiUrlHelper
    {
        private readonly Uri _baseUri;

        /// <summary>
        /// New helper
        /// </summary>
        /// <param name="baseUri"></param>
        public GeminiUrlHelper(Uri baseUri)
        {
            _baseUri = baseUri;
        }

        /// <summary>
        /// builds a issue url
        /// </summary>
        /// <param name="geminiIssue"></param>
        /// <returns></returns>
        public Uri BuilIssuedUri(GeminiIssue geminiIssue)
        {
            var uriBuilder = new UriBuilder(_baseUri);
            uriBuilder.Path += "/issue/ViewIssue.aspx";
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["id"] = geminiIssue.IssueId.ToString(CultureInfo.InvariantCulture);
            query["PROJID"] = geminiIssue.ProjectId.ToString(CultureInfo.InvariantCulture);
            uriBuilder.Query = query.ToString();
            return uriBuilder.Uri;
        }

        /// <summary>
        /// Builds a project url
        /// </summary>
        /// <param name="geminiProject"></param>
        /// <returns></returns>
        public Uri BuildProjectUri(GeminiProject geminiProject)
        {
            var uriBuilder = new UriBuilder(_baseUri);
            uriBuilder.Path += "/project/Project.aspx";
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["PROJID"] = geminiProject.ProjectId.ToString(CultureInfo.InvariantCulture);
            uriBuilder.Query = query.ToString();
            return uriBuilder.Uri;
        }
    }
}
