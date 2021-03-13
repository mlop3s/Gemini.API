using Gemini.Data.QueryParameters;
using System;
using System.Text;
using System.Text.Json;

namespace Gemini.Data.Pagination
{
    class MetaIssuePagination
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int CheckSum { get; set; }

        public static MetaIssuePagination? ParsePagination(IssuesQueryParameters issuesQueryParameters)
        {
            if (!string.IsNullOrEmpty(issuesQueryParameters.Page))
            {
                try
                {
                    var decoded = Base64Decode(issuesQueryParameters.Page);
                    var meta = JsonSerializer.Deserialize<MetaIssuePagination>(decoded);
                    return meta;
                }
                catch (JsonException)
                {
                    throw new ArgumentException("Invalid page parameter", nameof(issuesQueryParameters));
                }
                catch (FormatException)
                {
                    throw new ArgumentException("Invalid page parameter", nameof(issuesQueryParameters));
                }
            }

            return null;
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string Create(MetaIssuePagination metaIssuePagination, int totalPages, PageDirection pageDirection)
        {

            var pageNumber = metaIssuePagination.PageNumber;

            if (pageDirection == PageDirection.Next)
            {
                if (metaIssuePagination.PageNumber >= totalPages)
                {
                    return string.Empty;
                }

                pageNumber++;
            }
            else if (pageDirection == PageDirection.Previous)
            {
                if (metaIssuePagination.PageNumber < 1)
                {
                    return string.Empty;
                }

                pageNumber--;
            }

            var meta = new MetaIssuePagination
            {
                CheckSum = metaIssuePagination.CheckSum,
                PageNumber = pageNumber,
                PageSize = metaIssuePagination.PageSize
            };

            var json = JsonSerializer.Serialize(meta);

            return Base64Encode(json);
        }
    }
}
