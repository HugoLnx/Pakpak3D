using System.Collections.Generic;
using LnxArch;
using SensenToolkit.Gizmosx;
using UnityEngine;

namespace Pakpak3D
{
    // public class GraphNode<T> : IGraphNode
    // {
    //     private HashSet<IGraphNode> _neighbors = new();
    //     public IEnumerable<IGraphNode> Neighbors => _neighbors;
    //     public bool Visited { get; set; }
    //     public T Value { get; set; }
    //     public void AddNeighbor(IGraphNode neighbor)
    //     {
    //         _neighbors.Add(neighbor);
    //     }
    // }

    [LnxService]
    public class MazeService : MonoBehaviour
    {
        private GridBoard _grid;
        // private Dictionary<Vector2Int,

        [LnxInit]
        private void Init(GridBoard grid)
        {
            _grid = grid;
        }

        public HashSet<Vector3Int> GetSurfaceCellsRandomAreaSubset(float coverage)
        {
            // TODO: Implement coverage
            return GetAllSurfaceCells();
        }

        public HashSet<Vector3Int> GetAllSurfaceCells()
        {
            HashSet<Vector3Int> cells = new();

            foreach (Vector2Int cell2d in _grid.EnumerateCells2D())
            {
                Vector3Int? cellAboveGround = _grid.GetCellAboveGround(cell2d);
                if (!cellAboveGround.HasValue || _grid.HasWallOn(cell2d)) continue;
                cells.Add(cellAboveGround.Value);
            }

            return cells;
        }

        private void OnDrawGizmosSelected()
        {
            Init(GetComponent<GridBoard>());
            using (Gizmosx.Color(Color.green))
            {
                foreach (Vector3Int cell in GetAllSurfaceCells())
                {
                    Gizmos.DrawSphere(_grid.Cell3DToPosition(cell), 0.1f);
                }
            }
        }
    }
}
