using Gemini.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Gemini.Shared.Contracts
{
    /// <summary>
    /// Gemini Service
    /// </summary>
    public interface IGeminiService
    {
        /// <summary>
        /// Get the issues based on query parameters
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="year"></param>
        /// <param name="sprint"></param>
        /// <param name="excludeDone"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IEnumerable<GeminiIssue>> GetIssuesAsync(int projectId, int year, int? sprint, bool excludeDone, CancellationToken token);

        /// <summary>
        /// Get an issue by id
        /// </summary>
        /// <param name="issueId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<GeminiIssue> GetIssueAsync(int issueId, CancellationToken token);

        /// <summary>
        /// Get a project by code
        /// </summary>
        /// <param name="str"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<GeminiProject> GetProjectAsync(string str, CancellationToken token);

        /// <summary>
        /// Get issues based on a search code
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="searchName"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<GeminiIssue> GetIssueByCodeAsync(int projectId, string searchName, CancellationToken token);
    }
}
