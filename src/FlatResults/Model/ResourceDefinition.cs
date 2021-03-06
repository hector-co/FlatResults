﻿using FlatResults.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FlatResults.Model
{
    internal class ResourceDefinition<T> : IResourceDefinition
    {
        private readonly Type _type;
        private string _typeName;
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

        public void SetTypeName(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
                _typeName = typeof(T).Name;
            else
                _typeName = typeName;
        }

        public void AddAttribute(string name, Type type)
        {
            var prop = _type.GetCachedProperties().FirstOrDefault(p => p.Name == name);
            if (prop == null) return;
            _attributes.Add(name, (prop.GetValue, type));
        }

        public void RemoveAttribute(string name)
        {
            _attributes.Remove(name);
        }

        public void AddRelationShip(string name, Type type)
        {
            var prop = _type.GetCachedProperties().FirstOrDefault(p => p.Name == name);
            if (prop == null) return;
            _relationships.Add(name, (prop.GetValue, type));
        }

        public void RemoveRelationship(string name)
        {
            _relationships.Remove(name);
        }

        public Document ToDocument(object obj, bool identifierOnly = false, IEnumerable<string> fields = null)
        {
            if (_id == null) throw new IdPropertyNotFoundException();
            if (!(obj is T tObj)) throw new FlatResultsException($"Invalid type '{typeof(T).FullName}'");
            var document = new Document();
            if (identifierOnly)
            {
                document.Data = new Resource
                {
                    Id = _id(obj).ToString(),
                    Type = _typeName
                };
                return document;
            }

            document.Data = ToResource(tObj, fields);
            AddIncluded(tObj, document, fields);

            return document;
        }

        private Resource ToResource(T obj, IEnumerable<string> fields = null)
        {
            var resource = new Resource
            {
                Id = _id(obj).ToString(),
                Type = _typeName
            };

            if (_attributes.Any())
            {
                resource.Attributes = new ResourceAttributes();

                var keys = fields == null
                    ? _attributes.Keys
                    : _attributes.Keys.Where(k => fields.Contains(k, StringComparer.InvariantCultureIgnoreCase));

                foreach (var attrKey in keys)
                {
                    resource.Attributes.Add(attrKey, _attributes[attrKey].setter(obj));
                }
            }

            if (_relationships.Any())
            {
                resource.Relationships = new ResourceRelationships();
                foreach (var relKey in _relationships.Keys)
                {
                    var selected = fields == null
                        ? true
                        : fields.Any(f => f.StartsWith($"{relKey}.", StringComparison.InvariantCultureIgnoreCase) || f.Equals(relKey, StringComparison.InvariantCultureIgnoreCase));

                    if (!selected) continue;

                    var relValue = _relationships[relKey].setter(obj);
                    if (relValue == null) continue;
                    resource.Relationships.Add(relKey, DocumentExtensions.ToDocument(relValue as dynamic, true));
                }
            }

            return resource;
        }

        private void AddIncluded(T obj, Document document, IEnumerable<string> fields = null)
        {
            if (_relationships.Any())
            {
                foreach (var relKey in _relationships.Keys)
                {
                    var includedValue = _relationships[relKey].setter(obj);
                    if (includedValue == null) continue;

                    IEnumerable<string> relFields = null;

                    if (fields != null)
                    {
                        relFields = fields?.Where(f => f.StartsWith($"{relKey}.", StringComparison.InvariantCultureIgnoreCase)).Select(f => f.Split('.')[1]);

                        if (!relFields.Any() && fields.Any(f => f.Equals(relKey, StringComparison.InvariantCultureIgnoreCase)))
                            relFields = null;
                        else if (!relFields.Any())
                            continue;
                    }

                    var relDocument = (Document)DocumentExtensions.ToDocument(includedValue as dynamic, fields: relFields);
                    document.AppendIncluded(relDocument.Data as dynamic);
                    if (relDocument.Included != null)
                        document.AppendIncluded(relDocument.Included);
                }
            }
        }
    }
}
