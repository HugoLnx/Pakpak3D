using System.Collections.Generic;

namespace Pakpak3D
{
    public class SimpleGraphNode<T> : IGraphNode
    {
        public bool Visited { get; set; }
        public T Value { get; set; }
        public HashSet<IGraphNode> _neighbors = new();
        public IEnumerable<IGraphNode> Neighbors => _neighbors;

        public SimpleGraphNode(T value)
        {
            Value = value;
        }

        public void AddNeighbor(IGraphNode neighbor)
        {
            _neighbors.Add(neighbor);
        }
    }
}
