using Agora.Api.Models.JsonApi.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Agora.Api.Models.JsonApi.Converters
{
    public class JsonApiRelationshipConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(Dictionary<string, IJsonApiRelationship>).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Dictionary<string, IJsonApiRelationship> res = null;

            // Load JObject from stream

            var jObject = JObject.Load(reader);

            if (jObject.Type == JTokenType.Object)

            {
                res = new Dictionary<string, IJsonApiRelationship>();

                foreach (var jo in jObject)

                {
                    var rd = serializer.Deserialize<RelationshipDto>(jo.Value.CreateReader());

                    res.Add(jo.Key, rd);
                }
            }

            return res;
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}