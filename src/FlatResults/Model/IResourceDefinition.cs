namespace FlatResults.Model
{
    public interface IResourceDefinition
    {
        void SetId(string name);
        void AddAttribute(string name);
        void AddRelationShip(string name);
        Document ToDocument(object obj, bool identifierOnly = false);
    }
}
