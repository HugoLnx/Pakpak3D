using System.Collections.Generic;

namespace Pakpak3D
{
    public class SimpleGraph<TNode>
    where TNode : IGraphNode
    {
        private readonly HashSet<TNode> _nodes = new();
        public IReadOnlyCollection<TNode> Nodes => _nodes;

        public void AddNode(TNode node)
        {
            _nodes.Add(node);
        }

        public void LinkBothway(TNode node1, TNode node2)
        {
            AddNode(node1);
            AddNode(node2);
            node1.AddNeighbor(node2);
            node2.AddNeighbor(node1);
        }

        public IEnumerable<TNode> EnumerateBreadthTraverse(TNode start)
        {
            ResetVisited();
            Queue<TNode> queue = new();
            queue.Enqueue(start);
            start.Visited = true;

            while (queue.Count > 0)
            {
                TNode node = queue.Dequeue();
                yield return node;

                foreach (TNode neighbor in node.Neighbors)
                {
                    if (!neighbor.Visited)
                    {
                        neighbor.Visited = true;
                        queue.Enqueue(neighbor);
                    }
                }
            }
        }

        public void ResetVisited()
        {
            foreach (TNode node in _nodes)
            {
                node.Visited = false;
            }
        }
    }
}
