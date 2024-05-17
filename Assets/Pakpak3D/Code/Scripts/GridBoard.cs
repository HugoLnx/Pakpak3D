using System;
using System.Collections;
using System.Collections.Generic;
using LnxArch;
using SensenToolkit;
using SensenToolkit.Gizmosx;
using UnityEngine;

namespace Pakpak3D
{
    [LnxService]
    public class GridBoard : MonoBehaviour
    {
        [SerializeField] private float _cellSize = 1f;
        [SerializeField] private int _cellCountX = 28;
        [SerializeField] private int _cellCountZ = 36;
        [SerializeField] private int _cellCountHeight = 5;

        public float CellSize => _cellSize;

        private Vector3? _origin;
        private Vector3 Origin => _origin ??= transform.position
                + Vector3.up * (transform.localScale.y * 0.5f + _cellSize * 0.5f)
                + Vector3.right * -(_cellSize * 0.5f * ((_cellCountX + 1) % 2))
                + Vector3.forward * -(_cellSize * 0.5f * ((_cellCountZ + 1) % 2));

        private int WallLayerMask => _wallLayerMask ??= LayerMask.GetMask("Wall");
        private int? _wallLayerMask;
        private int GroundLayerMask => _groundLayerMask ??= LayerMask.GetMask("Ground");
        private int? _groundLayerMask;
        private int AnyObstacleLayerMask => WallLayerMask | GroundLayerMask;


        private void Start()
        {
            _origin = null;
        }

        public Vector3Int GetClosestCell(Vector3 position)
        {
            Vector3 originToCell = position - Origin;
            return new Vector3Int(
                x: Mathf.RoundToInt(originToCell.x / _cellSize),
                y: Mathf.RoundToInt(originToCell.y / _cellSize),
                z: Mathf.RoundToInt(originToCell.z / _cellSize)
            );
        }

        public Vector3 Cell3DToPosition(Vector3Int cell3d)
        {
            return Origin + cell3d.AsVector3Float() * _cellSize;
        }

        public Vector2 Cell2DToPosition(Vector2Int cell2d)
        {
            return Cell3DToPosition(cell2d.X0Y()).XZ();
        }

        public bool CanMoveTowards2D(Vector3 position, Vector2Int direction)
        {
            return CanMoveTowards(position, direction.X0Y());
        }

        public Vector3Int? GetCellAboveGround(Vector2Int cell2d)
        {
            float maxGroundHeight = _cellSize * 5f;
            Vector3 cellSkyPosition = Cell2DToPosition(cell2d).X0Y() + Vector3.up * maxGroundHeight;

            bool hitObstacle = Physics.Raycast(
                origin: cellSkyPosition,
                direction: Vector3.down,
                hitInfo: out RaycastHit hit,
                maxDistance: maxGroundHeight + _cellSize,
                layerMask: GroundLayerMask
            );
            if (!hitObstacle) return null;
            return GetClosestCell(hit.point + Vector3.up * (_cellSize * 0.5f));
        }

        public bool HasWallOn(Vector2Int cell2d)
        {
            float wallMaxHeight = _cellSize * 20f;
            Vector3 cellSkyPosition = Cell2DToPosition(cell2d).X0Y() + Vector3.up * wallMaxHeight;
            return Physics.Raycast(
                origin: cellSkyPosition,
                direction: Vector3.down,
                maxDistance: wallMaxHeight + _cellSize,
                layerMask: WallLayerMask
            );
        }

        // TODO: Remove this logic from GridBoard because each object must have it's own buffer
        private Vector3[] _rayMoveOriginsBuffer = new Vector3[4];
        public bool CanMoveTowards(Vector3 position, Vector3Int direction, bool ignoreGround = false)
        {
            if (!Grid3DDirection.IsValidDirection(direction))
            {
                throw new ArgumentException($"Invalid direction: {direction}. It should be a normalized vector with only one non-zero component.");
            }
            LayerMask layerMask = ignoreGround ? WallLayerMask : AnyObstacleLayerMask;
            Vector3Int originCell = GetClosestCell(position);
            Vector3 originPosition = Cell3DToPosition(originCell);
            Vector3 moveDirection = direction.AsVector3Float();
            GetTransversals(direction, out Vector3Int transversalInt, out Vector3Int transversal2Int);
            Vector3 transversal = transversalInt.AsVector3Float();
            Vector3 transversal2 = transversal2Int.AsVector3Float();

            _rayMoveOriginsBuffer[0] = originPosition + ((_cellSize * 0.5f - 0.001f) * transversal);
            _rayMoveOriginsBuffer[1] = originPosition + ((_cellSize * 0.5f - 0.001f) * transversal);
            _rayMoveOriginsBuffer[2] = originPosition + ((_cellSize * 0.5f - 0.001f) * transversal2);
            _rayMoveOriginsBuffer[3] = originPosition + ((_cellSize * 0.5f - 0.001f) * -transversal2);

            for (int i = 0; i < _rayMoveOriginsBuffer.Length; i++)
            {
                Vector3 origin = _rayMoveOriginsBuffer[i];
                bool hitObstacle = Physics.Raycast(
                    origin: origin,
                    direction: moveDirection,
                    hitInfo: out RaycastHit hit,
                    maxDistance: _cellSize * 0.75f,
                    layerMask: layerMask
                );
                if (hitObstacle) return false;
            }

            return true;
        }

        public bool CanFall(Vector3 position)
        {
            return CanMoveTowards(position, Vector3Int.down);
        }

        public IEnumerable<Vector2Int> EnumerateCells2D()
        {
            int minCellX = -Mathf.CeilToInt(_cellCountX / 2f - 1);
            int maxCellX = _cellCountX / 2;
            int minCellZ = -Mathf.CeilToInt(_cellCountZ / 2f - 1);
            int maxCellZ = _cellCountZ / 2;
            for (int cellX = minCellX; cellX <= maxCellX; cellX++)
            {
                for (int cellZ = minCellZ; cellZ <= maxCellZ; cellZ++)
                {
                    yield return new Vector2Int(cellX, cellZ);
                }
            }
        }

        private void GetTransversals(Vector3Int direction, out Vector3Int transversal1, out Vector3Int transversal2)
        {
            Vector3Int candidate1 = new(direction.z, 0, direction.x);
            Vector3Int candidate2 = new(0, direction.z, direction.y);
            Vector3Int candidate3 = new(direction.y, direction.x, 0);
            transversal1 = candidate1 != Vector3.zero ? candidate1 : candidate2;
            transversal2 = candidate3 != Vector3.zero ? candidate3 : candidate2;
        }

        private void OnDrawGizmos()
        {
            var gizmosUnselectedColor = new Color(0.5f, 0.25f, 0.25f, 0.02f);
            DrawGridGizmos(gizmosUnselectedColor);
        }

        private void OnDrawGizmosSelected()
        {
            var gizmosSelectedColor = new Color(0.5f, 0f, 0f, 0.5f);
            DrawGridGizmos(gizmosSelectedColor);
        }

        private void DrawGridGizmos(Color gridGizmosColor)
        {
            _origin = null;
            using (Gizmosx.Color(gridGizmosColor))
            {
                if (_cellCountHeight == 0)
                {
                    DrawFloorGizmos();
                }
                else
                {
                    DrawLayeredCubeGizmos();
                }
            }
        }

        private void DrawFloorGizmos()
        {
            foreach (Vector2Int cell2d in EnumerateCells2D())
            {
                Vector3Int cell3d = cell2d.X0Y();
                Vector3 center = Cell3DToPosition(cell3d)
                    + Vector3.down * (_cellSize * 0.5f);
                Gizmos.DrawWireCube(center, new Vector3(_cellSize, 0.1f, _cellSize));
            }
        }

        private void DrawLayeredCubeGizmos()
        {
            for (int cellY = 0; cellY < _cellCountHeight; cellY++)
            {
                foreach (Vector2Int cell2d in EnumerateCells2D())
                {
                    int cellX = cell2d.x;
                    int cellZ = cell2d.y;
                    Vector3Int cell3d = new(cellX, cellY, cellZ);
                    Vector3 center = Cell3DToPosition(cell3d);
                    Gizmos.DrawWireCube(center, Vector3.one * _cellSize);
                };
            }
        }
    }
}
