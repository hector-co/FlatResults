using FlatResults.Model;
using System.Collections.Generic;
using System.Linq;

namespace FlatResults
{
    public static class DocumentExtensions
    {
        public static Document ToDocument<T>(this T obj, bool identifierOnly = false, IEnumerable<string> fields = null)
        {
            if (DocumentMapperConfig.IsWrapperType(typeof(T)))
            {
                var (data, meta) = DocumentMapperConfig.GetWrapperTypeDefinition(typeof(T).GetGenericTypeDefinition());
                var document = (Document)DocumentExtensions.ToDocument(data(obj), fields: fields);
                if (meta != null)
                {
                    var metaInfos = meta(obj);
                    foreach (var info in metaInfos)
                    {
                        document.AddMeta(info.Key, info.Value);
                    }
                }
                return document;
            }
            else
            {
                var definition = DocumentMapperConfig.GetDefinition<T>();
                return definition.ToDocument(obj, identifierOnly, fields);
            }
        }

        public static Document ToDocument<T>(this IEnumerable<T> objs, bool identifiersOnly = false, IEnumerable<string> fields = null)
        {
            var document = new Document
            {
                Data = new ResourceCollection()
            };
            foreach (var obj in objs)
            {
                document.Append(ToDocument(obj, identifiersOnly, fields));
            }
            return document;
        }

        public static Document ToDocument<T>(this T[] objs, bool identifiersOnly = false, IEnumerable<string> fields = null)
        {
            return ToDocument(objs.AsEnumerable(), identifiersOnly, fields);
        }

        public static Document ToDocument<T>(this List<T> objs, bool identifiersOnly = false, IEnumerable<string> fields = null)
        {
            return ToDocument(objs.AsEnumerable(), identifiersOnly, fields);
        }
    }
}
