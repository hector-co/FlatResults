using System;
using System.Collections.Generic;
using System.Linq;

namespace FlatResults.Model
{
    internal class ResourceDefinition<T> : IResourceDefinition
    {
        private readonly Type _type;
        private Func<object, object> _id;
        private readonly Dictionary<string, Func<object, object>> _attributes;
        private readonly Dictionary<string, Func<object, object>> _relationships;

        public ResourceDefinition()
        {
            _type = typeof(T);
            _attributes = new Dictionary<string, Func<object, object>>();
            _relationships = new Dictionary<string, Func<object, object>>();

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
            _attributes.Add(name, prop.GetValue);
        }

        public void AddRelationShip(string name)
        {
            var prop = _type.GetProperty(name);
            if (prop == null) return;
            _relationships.Add(name, prop.GetValue);
        }

        public Document ToDocument(object obj, bool identifierOnly = false)
        {
            if (!(obj is T tObj)) throw new Exception("Invalid type");
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
            if (_id == null) throw new Exception("Id not set");

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
