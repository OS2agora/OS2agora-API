using System;
using System.Collections.Generic;
using System.Linq;
using BallerupKommune.Api.Models.JsonApi.Interfaces;
using Newtonsoft.Json;

namespace BallerupKommune.Api.Models.Common
{
    public class BaseAttributeDto : IJsonApiAttributes
    {
        public DateTime Created { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? LastModified { get; set; }

        public string LastModifiedBy { get; set; }

        [JsonIgnore] public List<string> PropertiesUpdated = new List<string>();

        public void PropertyUpdated([System.Runtime.CompilerServices.CallerMemberName]
            string propertyName = null)
        {
            if (PropertiesUpdated.All(p => p != propertyName))
            {
                PropertiesUpdated.Add(propertyName);
            }
        }
    }
}