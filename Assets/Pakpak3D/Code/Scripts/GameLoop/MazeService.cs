using System;
using System.Collections.Generic;
using LnxArch;
using SensenToolkit;
using SensenToolkit.Gizmosx;
using SensenToolkit.Utils;
using UnityEngine;

namespace Pakpak3D
{
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
            HashSet<Vector3Int> allCells = GetAllSurfaceCells();
            SimpleGraph<SimpleGraphNode<Vector3Int>> graph = BuildCellsGraph(allCells);

            int targetCount = Mathf.CeilToInt(allCells.Count * coverage);
            HashSet<Vector3Int> cellsSubset = new();
            SimpleGraphNode<Vector3Int> startNode = graph.Nodes.GetRandomElement();
            foreach (SimpleGraphNode<Vector3Int> node in graph.EnumerateBreadthTraverse(startNode))
            {
                cellsSubset.Add(node.Value);
                if (cellsSubset.Count >= targetCount) break;
            }
            return cellsSubset;
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

        private SimpleGraph<SimpleGraphNode<Vector3Int>> BuildCellsGraph(HashSet<Vector3Int> allCells)
        {
            Dictionary<Vector2Int, SimpleGraphNode<Vector3Int>> nodesDict = new();
            foreach (Vector3Int cell3d in allCells)
            {
                Vector2Int cell2d = cell3d.XZ();
                if (!nodesDict.ContainsKey(cell2d))
                {
                    nodesDict[cell2d] = new SimpleGraphNode<Vector3Int>(cell3d);
                }
            }

            SimpleGraph<SimpleGraphNode<Vector3Int>> graph = new();
            foreach ((Vector2Int cell2d, SimpleGraphNode<Vector3Int> node) in nodesDict)
            {
                foreach (Direction4 direction in Direction4.All)
                {
                    Vector2Int neighbor2d = cell2d + direction.VectorInt;
                    if (nodesDict.TryGetValue(neighbor2d, out SimpleGraphNode<Vector3Int> neighborNode))
                    {
                        graph.LinkBothway(node, neighborNode);
                    }
                }
            }
            return graph;
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
