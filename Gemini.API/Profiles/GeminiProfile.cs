using AutoMapper;
using Gemini.Data.Entities;
using Gemini.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using System;
using System.Globalization;
using System.Linq;

namespace Gemini.API.Profiles
{
    /// <summary>
    /// A mapping profile for entity => dto mapping
    /// </summary>
    public class GeminiProfile : Profile
    {
        private int _customId = 256;

        /// <summary>
        /// ctor that sets the mapping
        /// </summary>
        public GeminiProfile(IConfiguration configuration)
        {
            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (int.TryParse(configuration["SprintFieldId"], out int id))
            {
                _customId = id;
            }

            CreateMap<GeminiIssueEntity, GeminiIssue>().ForMember(i => i.Sprint, act => act.MapFrom(o => GetSprint(o, _customId)));
            CreateMap<GeminiIssueHistoryEntity, GeminiIssueHistory>();
            CreateMap<GeminiCustomFieldEntity, GeminiCustomField>();
            CreateMap<GeminiProjectEnitity, GeminiProject>();
        }

        private static string GetSprint(GeminiIssueEntity o, int customId)
        {
            var custom = o.CustomFields.FirstOrDefault(x => x.CustomFieldId == customId);
            if (custom?.NumericData is null)
            {
                return string.Empty;
            }

            var i = decimal.ToInt32(custom.NumericData.Value);
            return i.ToString(CultureInfo.InvariantCulture);
        }
            
        
    }
}
