using FlatResults.Exceptions;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace FlatResults.Model
{
    internal class ResourceDefinition<T> : IResourceDefinition
    {
        private readonly Type _type;
        private Func<object, object> _id;
        private readonly ConcurrentDictionary<string, Func<object, object>> _attributes;
        private readonly ConcurrentDictionary<string, Func<object, object>> _relationships;

        public ResourceDefinition()
        {
            _type = typeof(T);
            _attributes = new ConcurrentDictionary<string, Func<object, object>>();
            _relationships = new ConcurrentDictionary<string, Func<object, object>>();

            SetId("Id");
            if (_id == null) SetId($"{_type.Name}Id");
        }

        public void SetId(string name)
        {
            var prop = _type.GetProperty(name);
            if (prop == null) return;
            _id = prop.GetValue;
        }

        public void AddAttribute(string name)
        {
            var prop = _type.GetProperty(name);
            if (prop == null) return;
            _attributes.TryAdd(name, prop.GetValue);
        }

        public void AddRelationShip(string name)
        {
            var prop = _type.GetProperty(name);
            if (prop == null) return;
            _relationships.TryAdd(name, prop.GetValue);
        }

        public Document ToDocument(object obj, bool identifierOnly = false)
        {
            if (!(obj is T tObj)) throw new FlatResultsException("Invalid type");
            if (_id == null) throw new IdPropertyNotFoundException();
            var document = new Document();
            if (identifierOnly)
            {
                document.Data = new Resource
                {
                    Id = _id(obj).ToString(),
                    Type = typeof(T).Name
                };
                return document;
            }

            document.Data = ToResource(tObj);
            AddIncluded(tObj, document);

            return document;
        }

        private Resource ToResource(T obj)
        {
            var resource = new Resource
            {
                Id = _id(obj).ToString(),
                Type = typeof(T).Name
            };

            if (_attributes.Any())
            {
                resource.Attributes = new ResourceAttributes();
                foreach (var attrKey in _attributes.Keys)
                {
                    resource.Attributes.Add(attrKey, _attributes[attrKey](obj));
                }
            }

            if (_relationships.Any())
            {
                resource.Relationships = new ResourceRelationships();
                foreach (var relKey in _relationships.Keys)
                {
                    resource.Relationships.Add(relKey, DocumentExtensions.ToDocument(_relationships[relKey](obj) as dynamic, true));
                }
            }

            return resource;
        }

        private void AddIncluded(T obj, Document document)
        {
            if (_relationships.Any())
            {
                foreach (var relKey in _relationships.Keys)
                {
                    var relDocument = (Document)DocumentExtensions.ToDocument(_relationships[relKey](obj) as dynamic);
                    document.AppendIncluded(relDocument.Data as dynamic);
                    if (relDocument.Included != null)
                        document.AppendIncluded(relDocument.Included);
                }
            }
        }
    }
}
