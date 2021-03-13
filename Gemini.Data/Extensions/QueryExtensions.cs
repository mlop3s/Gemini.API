using Gemini.Data.QueryParameters;
using System;

namespace Gemini.Data.Extensions
{
    public static class QueryExtensions
    {
        public static bool HasIncludes(this SingleIssueQueryParameter singleIssueQueryParameter)
            => singleIssueQueryParameter.IncludeFields == true || singleIssueQueryParameter.IncludeHistory == true;
    }
}
