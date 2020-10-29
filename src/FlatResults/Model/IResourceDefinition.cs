using System;
using System.Collections.Generic;

namespace FlatResults.Model
{
    public interface IResourceDefinition
    {
        void SetId(string name);
        void SetTypeName(string typeName);
        void AddAttribute(string name, Type type);
        void RemoveAttribute(string name);
        void AddRelationShip(string name, Type type);
        void RemoveRelationship(string name);
        IReadOnlyDictionary<string, Type> Attributes { get; }
        IReadOnlyDictionary<string, Type> Relationships { get; }
        Document ToDocument(object obj, bool identifierOnly = false, IEnumerable<string> fields = null);
    }
}
