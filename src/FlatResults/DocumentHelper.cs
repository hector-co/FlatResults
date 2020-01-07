using FlatResults.Model;
using System.Collections.Generic;
using System.Linq;

namespace FlatResults
{
    public static class DocumentExtensions
    {
        public static Document ToDocument<T>(this T obj, bool identifierOnly = false)
        {
            var definition = DocumentMapperConfig.GetDefinition<T>();
            return definition.ToDocument(obj, identifierOnly);
        }

        public static Document ToDocument<T>(this IEnumerable<T> objs, bool identifiersOnly = false)
        {
            var document = new Document
            {
                Data = new ResourceCollection()
            };
            foreach (var obj in objs)
            {
                document.Append(ToDocument(obj, identifiersOnly));
            }
            return document;
        }

        public static Document ToDocument<T>(this T[] objs, bool identifiersOnly = false)
        {
            return ToDocument(objs.AsEnumerable(), identifiersOnly);
        }

        public static Document ToDocument<T>(this List<T> objs, bool identifiersOnly = false)
        {
            return ToDocument(objs.AsEnumerable(), identifiersOnly);
        }
    }
}
