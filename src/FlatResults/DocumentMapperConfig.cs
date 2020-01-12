using FlatResults.Exceptions;
using FlatResults.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FlatResults
{
    public static class DocumentMapperConfig
    {
        private static ConcurrentDictionary<Type, IResourceDefinition> _definitions = new ConcurrentDictionary<Type, IResourceDefinition>();
        private static ConcurrentDictionary<Type, HashSet<string>> _ignoredFields = new ConcurrentDictionary<Type, HashSet<string>>();

        public static ResourceDefinitionMapperConfig<TType> ForType<TType>()
        {
            AdjustRelationships<TType>();
            if (!_definitions.ContainsKey(typeof(TType)))
            {
                _definitions.TryAdd(typeof(TType), new ResourceDefinition<TType>());
                _ignoredFields.TryAdd(typeof(TType), new HashSet<string>());
            }
            return new ResourceDefinitionMapperConfig<TType>(_definitions, _ignoredFields);
        }

        public static ResourceDefinitionMapperConfig<TType> NewConfig<TType>()
        {
            AdjustRelationships<TType>();
            if (_definitions.ContainsKey(typeof(TType)))
            {
                _definitions.TryRemove(typeof(TType), out var _);
                _ignoredFields.TryRemove(typeof(TType), out var _);
            }
            _definitions.TryAdd(typeof(TType), new ResourceDefinition<TType>());
            _ignoredFields.TryAdd(typeof(TType), new HashSet<string>());
            return new ResourceDefinitionMapperConfig<TType>(_definitions, _ignoredFields);
        }

        public static void ClearConfigs()
        {
            _definitions = new ConcurrentDictionary<Type, IResourceDefinition>();
        }

        internal static IResourceDefinition GetDefinition<TType>()
        {
            var key = typeof(TType);
            if (!_definitions.ContainsKey(key))
                throw new ConfigNotFoundException();
            return _definitions[key];
        }

        private static void AdjustRelationships<TType>()
        {
            foreach (var definition in _definitions)
            {
                foreach (var attribute in definition.Value.Attributes)
                {
                    if (attribute.Value == typeof(TType) || attribute.Value.IsGenericOf<TType>())
                    {
                        definition.Value.RemoveAttribute(attribute.Key);
                        definition.Value.AddRelationShip(attribute.Key, attribute.Value);
                    }
                }
            }
        }
    }

    public class ResourceDefinitionMapperConfig<TType>
    {
        private readonly ConcurrentDictionary<Type, IResourceDefinition> _definitions;
        private readonly ConcurrentDictionary<Type, HashSet<string>> _ignoredFields;

        public ResourceDefinitionMapperConfig(ConcurrentDictionary<Type, IResourceDefinition> definitions, ConcurrentDictionary<Type, HashSet<string>> ignoredFields)
        {
            _definitions = definitions;
            _ignoredFields = ignoredFields;
        }

        public ResourceDefinitionMapperConfig<TType> WithId(string name)
        {
            _definitions[typeof(TType)].SetId(name);
            return this;
        }

        public ResourceDefinitionMapperConfig<TType> WithId<TProperty>(Expression<Func<TType, TProperty>> property)
        {
            if (!property.TryGetPropertyInfo(out var propInfo)) return this;
            return WithId(propInfo.Name);
        }

        public ResourceDefinitionMapperConfig<TType> WithAttribute(string name, Type type)
        {
            _definitions[typeof(TType)].AddAttribute(name, type);
            return this;
        }

        public ResourceDefinitionMapperConfig<TType> WithAttribute<TProperty>(Expression<Func<TType, TProperty>> property)
        {
            if (!property.TryGetPropertyInfo(out var propInfo)) return this;
            return WithAttribute(propInfo.Name, propInfo.PropertyType);
        }

        public ResourceDefinitionMapperConfig<TType> WithRelationship(string name, Type type)
        {
            _definitions[typeof(TType)].AddRelationShip(name, type);
            return this;
        }

        public ResourceDefinitionMapperConfig<TType> WithRelationship<TProperty>(Expression<Func<TType, TProperty>> property)
        {
            if (!property.TryGetPropertyInfo(out var propInfo)) return this;
            return WithRelationship(propInfo.Name, propInfo.PropertyType);
        }

        public ResourceDefinitionMapperConfig<TType> MapWithDetaults()
        {
            foreach (var propInfo in typeof(TType).GetCachedProperties())
            {
                if (_ignoredFields[typeof(TType)].Contains(propInfo.Name)) continue;
                if (_definitions.ContainsKey(propInfo.PropertyType))
                    _definitions[typeof(TType)].AddRelationShip(propInfo.Name, propInfo.PropertyType);
                else
                    _definitions[typeof(TType)].AddAttribute(propInfo.Name, propInfo.PropertyType);
            }
            return this;
        }

        public ResourceDefinitionMapperConfig<TType> Ignore<TProperty>(Expression<Func<TType, TProperty>> property)
        {
            if (!property.TryGetPropertyInfo(out var propInfo)) return this;
            _ignoredFields[typeof(TType)].Add(propInfo.Name);
            var ignoredFields = _ignoredFields[typeof(TType)];
            foreach (var attribute in _definitions[typeof(TType)].Attributes)
            {
                if (ignoredFields.Contains(attribute.Key))
                    _definitions[typeof(TType)].RemoveAttribute(attribute.Key);
            }
            foreach (var relationship in _definitions[typeof(TType)].Relationships)
            {
                if (ignoredFields.Contains(relationship.Key))
                    _definitions[typeof(TType)].RemoveRelationship(relationship.Key);
            }
            return this;
        }
    }
}
