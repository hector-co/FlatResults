using FlatResults.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FlatResults.Model
{
    internal class ResourceDefinition<T> : IResourceDefinition
    {
        private readonly Type _type;
        private Func<object, object> _id;
        private readonly Dictionary<string, (Func<object, object> setter, Type type)> _attributes;
        private readonly Dictionary<string, (Func<object, object> setter, Type type)> _relationships;

        public IReadOnlyDictionary<string, Type> Attributes => _attributes.ToDictionary(a => a.Key, a => a.Value.type);

        public IReadOnlyDictionary<string, Type> Relationships => _relationships.ToDictionary(a => a.Key, a => a.Value.type);

        public ResourceDefinition()
        {
            _type = typeof(T);
            _attributes = new Dictionary<string, (Func<object, object> setter, Type type)>();
            _relationships = new Dictionary<string, (Func<object, object> setter, Type type)>();

            SetId("Id");
            if (_id == null) SetId($"{_type.Name}Id");
        }

        public void SetId(string name)
        {
            var prop = _type.GetCachedProperties().FirstOrDefault(p => p.Name == name);
            if (prop == null) return;
            _id = prop.GetValue;
        }

        public void AddAttribute(string name, Type type)
        {
            var prop = _type.GetCachedProperties().FirstOrDefault(p => p.Name == name);
            if (prop == null) return;
            _attributes.TryAdd(name, (prop.GetValue, type));
        }

        public void RemoveAttribute(string name)
        {
            _attributes.Remove(name);
        }

        public void AddRelationShip(string name, Type type)
        {
            var prop = _type.GetCachedProperties().FirstOrDefault(p => p.Name == name);
            if (prop == null) return;
            _relationships.TryAdd(name, (prop.GetValue, type));
        }

        public void RemoveRelationship(string name)
        {
            _relationships.Remove(name);
        }

        public Document ToDocument(object obj, bool identifierOnly = false)
        {
            if (_id == null) throw new IdPropertyNotFoundException();
            if (!(obj is T tObj)) throw new FlatResultsException("Invalid type");
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
                    resource.Attributes.Add(attrKey, _attributes[attrKey].setter(obj));
                }
            }

            if (_relationships.Any())
            {
                resource.Relationships = new ResourceRelationships();
                foreach (var relKey in _relationships.Keys)
                {
                    var relValue = _relationships[relKey].setter(obj);
                    if (relValue == null) continue;
                    resource.Relationships.Add(relKey, DocumentExtensions.ToDocument(relValue as dynamic, true));
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
                    var includedValue = _relationships[relKey].setter(obj);
                    if (includedValue == null) continue;
                    var relDocument = (Document)DocumentExtensions.ToDocument(includedValue as dynamic);
                    document.AppendIncluded(relDocument.Data as dynamic);
                    if (relDocument.Included != null)
                        document.AppendIncluded(relDocument.Included);
                }
            }
        }
    }
}
