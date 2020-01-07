using Newtonsoft.Json;
using System.Collections.Generic;

namespace FlatResults.Model
{
    public class Document
    {
        public IData Data { get; internal set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public HashSet<Resource> Included { get; internal set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Meta { get; internal set; }

        public void Append(Document document)
        {
            AppendData(document.Data as dynamic);
            AppendIncluded(document.Included);

        }

        private void AppendData(Resource resource)
        {
            if (Data != null && !(Data is ResourceCollection)) return;

            if (Data == null) Data = new ResourceCollection();

            ((ResourceCollection)Data).Add(resource);
        }

        private void AppendData(ResourceCollection resources)
        {
            if (Data != null && !(Data is ResourceCollection)) return;

            if (Data == null) Data = new ResourceCollection();

            foreach (var resource in resources)
                ((ResourceCollection)Data).Add(resource);
        }

        internal void AppendIncluded(Resource resource)
        {
            if (resource == null) return;
            if (Included == null) Included = new HashSet<Resource>();
            Included.Add(resource);
        }

        internal void AppendIncluded(ResourceCollection resources)
        {
            if (resources == null) return;
            if (Included == null) Included = new HashSet<Resource>();
            foreach (var res in resources)
            {
                Included.Add(res as dynamic);
            }
        }

        internal void AppendIncluded(HashSet<Resource> included)
        {
            if (included == null) return;
            if (Included == null)
            {
                Included = included;
                return;
            }
            foreach (var inc in included)
            {
                Included.Add(inc);
            }
        }

        public void AddMeta(string key, object value)
        {
            if (Meta == null) Meta = new Dictionary<string, object>();

            if (Meta.ContainsKey(key))
                Meta[key] = value;
            else
                Meta.Add(key, value);
        }
    }
}
