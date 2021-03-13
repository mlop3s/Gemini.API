using Gemini.Data.Entities;
using System;
using System.Linq;

namespace Gemini.Data.Extensions
{
    public static class GeminiIssueExtensions
    {

        public static bool IsInSprint(this GeminiIssueEntity @this, int sprint, int fieldID)
        {
            var current = @this.CustomFields.FirstOrDefault(l => l.CustomFieldId == fieldID)?.NumericData;
            if (current is not null && Decimal.ToInt32(current.Value) == sprint)
            {
                return true;
            }

            return false;
        }
    }
}
