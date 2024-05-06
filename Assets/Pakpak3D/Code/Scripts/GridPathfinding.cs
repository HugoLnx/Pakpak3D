using LnxArch;
using SensenToolkit;
using UnityEngine;
using UnityEngine.AI;

namespace Pakpak3D
{
    public class GridPathfinding : MonoBehaviour
    {
        private GridBoard _grid;
        private NavMeshPath _navmeshPath;
        private Vector3[] _waypointsBuffer = new Vector3[3];
        [LnxInit]
        private void Init(GridBoard grid)
        {
            _grid = grid;
            _navmeshPath = new();
        }

        public Vector2Int? GetNextStepDirection(Vector2Int start, Vector2Int target)
        {
            Vector3Int? startCell = _grid.GetCellAboveFloor(start);
            if (!startCell.HasValue)
            {
                throw new System.Exception($"Invalid start position: {start}");
            }

            Vector3Int? targetCell = _grid.GetCellAboveFloor(target);
            if (!targetCell.HasValue)
            {
                return null;
            }

            Vector3 startPosition = _grid.Cell3DToPosition(startCell.Value);
            Vector3 targetPosition = _grid.Cell3DToPosition(targetCell.Value);

            bool foundPath = NavMesh.CalculatePath(startPosition, targetPosition, NavMesh.AllAreas, _navmeshPath);
            if (!foundPath)
            {
                return null;
            }

            int count = _navmeshPath.GetCornersNonAlloc(_waypointsBuffer);
            // Debug.Log($"Found path with {count} waypoints from {startPosition}.");
            for (int i = 1; i < count; i++)
            {
                Vector3 waypoint = _waypointsBuffer[i];
                Vector2Int waypointCell = _grid.GetClosestCell(waypoint).XZ();
                // Debug.Log($"Waypoint {i}: {waypoint}");
                if (waypointCell != start)
                {
                    Vector2Int direction = waypointCell - start;
                    // Debug.Log($"Chosen direction: {direction}");
                    return direction;
                }
            }

            return null;
        }
    }
}
