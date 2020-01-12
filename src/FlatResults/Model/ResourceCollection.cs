using System.Collections;
using System.Collections.Generic;

namespace FlatResults.Model
{
    public class ResourceCollection : IEnumerable<IData>, IData
    {
        private readonly HashSet<IData> _resources;

        public ResourceCollection()
        {
            _resources = new HashSet<IData>();
        }

        public IEnumerator<IData> GetEnumerator()
        {
            return _resources.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _resources.GetEnumerator();
        }

        public void Add(IData data)
        {
            _resources.Add(data);
        }
    }
}
