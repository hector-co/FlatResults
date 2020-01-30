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
        private static ConcurrentDictionary<Type, (Func<dynamic, dynamic> data, Func<dynamic, Dictionary<string, object>> meta)> _wrapperTypes
            = new ConcurrentDictionary<Type, (Func<dynamic, dynamic> data, Func<dynamic, Dictionary<string, object>> meta)>();

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
            _ignoredFields = new ConcurrentDictionary<Type, HashSet<string>>();
            _wrapperTypes = new ConcurrentDictionary<Type, (Func<dynamic, dynamic> data, Func<dynamic, Dictionary<string, object>> meta)>();
        }

        public static bool IsValidType(Type type)
        {
            if (type == typeof(Document))
                if (_definitions.ContainsKey(type)) return true;
            if (_wrapperTypes.ContainsKey(type)) return true;
            if (!type.IsGenericType) return false;

            if (_definitions.ContainsKey(type.GetGenericArguments()[0])) return true;
            return _wrapperTypes.ContainsKey(type.GetGenericTypeDefinition());
        }

        internal static IResourceDefinition GetDefinition<TType>()
        {
            var key = typeof(TType);
            if (!_definitions.ContainsKey(key))
                throw new ConfigNotFoundException();
            return _definitions[key];
        }

        public static void AddWrapperType(Type type, Func<dynamic, dynamic> data, Func<dynamic, Dictionary<string, object>> meta = null)
        {
            if (_wrapperTypes.ContainsKey(type)) _wrapperTypes.TryRemove(type, out var _);
            _wrapperTypes.TryAdd(type, (data, meta));
        }

        public static (Func<dynamic, dynamic> data, Func<dynamic, Dictionary<string, object>> meta) GetWrapperTypeDefinition(Type type)
        {
            if (_wrapperTypes.ContainsKey(type)) return _wrapperTypes[type];
            return _wrapperTypes[type.GetGenericTypeDefinition()];
        }

        internal static bool IsWrapperType(Type type)
        {
            if (_wrapperTypes.ContainsKey(type)) return true;
            if (!type.IsGenericType) return false;
            return _wrapperTypes.ContainsKey(type.GetGenericTypeDefinition());
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

        public ResourceDefinitionMapperConfig<TType> WithTypeName(string typeName)
        {
            _definitions[typeof(TType)].SetTypeName(typeName);
            return this;
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
                if (_definitions.ContainsKey(propInfo.PropertyType) ||
                    (propInfo.PropertyType.IsGenericType && _definitions.ContainsKey(propInfo.PropertyType.GetGenericArguments()[0])))
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
