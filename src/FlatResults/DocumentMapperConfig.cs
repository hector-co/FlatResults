using FlatResults.Exceptions;
using FlatResults.Model;
using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace FlatResults
{
    public static class DocumentMapperConfig
    {
        private static ConcurrentDictionary<Type, IResourceDefinition> _definitions = new ConcurrentDictionary<Type, IResourceDefinition>();

        public static ResourceDefinitionMapperConfig<TType> ForType<TType>()
        {
            if (!_definitions.ContainsKey(typeof(TType))) _definitions.TryAdd(typeof(TType), new ResourceDefinition<TType>());
            return new ResourceDefinitionMapperConfig<TType>(_definitions[typeof(TType)]);
        }

        public static ResourceDefinitionMapperConfig<TType> NewConfig<TType>()
        {
            if (_definitions.ContainsKey(typeof(TType)))
                _definitions.TryRemove(typeof(TType), out var _);
            _definitions.TryAdd(typeof(TType), new ResourceDefinition<TType>());
            return new ResourceDefinitionMapperConfig<TType>(_definitions[typeof(TType)]);
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
    }

    public class ResourceDefinitionMapperConfig<TType>
    {
        private readonly IResourceDefinition _definition;

        public ResourceDefinitionMapperConfig(IResourceDefinition definition)
        {
            _definition = definition;
        }

        public ResourceDefinitionMapperConfig<TType> WithId(string name)
        {
            _definition.SetId(name);
            return this;
        }

        public ResourceDefinitionMapperConfig<TType> WithId<TProperty>(Expression<Func<TType, TProperty>> property)
        {
            if (!property.TryGetPropertyInfo(out var propInfo)) return this;
            return WithId(propInfo.Name);
        }

        public ResourceDefinitionMapperConfig<TType> WithAttribute(string name)
        {
            _definition.AddAttribute(name);
            return this;
        }

        public ResourceDefinitionMapperConfig<TType> WithAttribute<TProperty>(Expression<Func<TType, TProperty>> property)
        {
            if (!property.TryGetPropertyInfo(out var propInfo)) return this;
            return WithAttribute(propInfo.Name);
        }

        public ResourceDefinitionMapperConfig<TType> WithRelationship(string name)
        {
            _definition.AddRelationShip(name);
            return this;
        }

        public ResourceDefinitionMapperConfig<TType> WithRelationship<TProperty>(Expression<Func<TType, TProperty>> property)
        {
            if (!property.TryGetPropertyInfo(out var propInfo)) return this;
            return WithRelationship(propInfo.Name);
        }

        public ResourceDefinitionMapperConfig<TType> MapWithDetaults()
        {
            return this;
        }

        public ResourceDefinitionMapperConfig<TType> Ignore<TProperty>(Expression<Func<TType, TProperty>> property)
        {
            return this;
        }
    }
}
