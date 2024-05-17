using System.Collections.Generic;

namespace Pakpak3D
{
    public interface IGraphNode
    {
        bool Visited { get; set; }
        IEnumerable<IGraphNode> Neighbors { get; }
        void AddNeighbor(IGraphNode neighbor);
    }
}
