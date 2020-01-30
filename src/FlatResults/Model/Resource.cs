using Newtonsoft.Json;
using System.Collections.Generic;

namespace FlatResults.Model
{
    public class Resource : IData
    {
        public string Type { get; internal set; }
        public string Id { get; internal set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ResourceAttributes Attributes { get; internal set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ResourceRelationships Relationships { get; internal set; }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !GetType().Equals(obj.GetType()))
                return false;

            var objRes = (Resource)obj;
            return Type == objRes.Type && Id == objRes.Id;
        }

        public override int GetHashCode()
        {
            var hashCode = -1324594315;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Type);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Id);
            return hashCode;
        }
    }
}
