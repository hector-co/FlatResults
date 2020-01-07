using FlatResults.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FlatResults
{
    public static class DocumentMapperConfig
    {
        private static Dictionary<Type, IResourceDefinition> _definitions = new Dictionary<Type, IResourceDefinition>();

        public static ResourceDefinitionMapperConfig<TType> ForType<TType>()
        {
            if (!_definitions.ContainsKey(typeof(TType))) _definitions.Add(typeof(TType), new ResourceDefinition<TType>());
            return new ResourceDefinitionMapperConfig<TType>(_definitions[typeof(TType)]);
        }

        internal static IResourceDefinition GetDefinition<TType>()
        {
            return _definitions[typeof(TType)];
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

        public ResourceDefinitionMapperConfig<TType> ExcludeNonMapped()
        {
            return this;
        }
    }
}
