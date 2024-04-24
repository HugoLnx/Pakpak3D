using System.Collections;
using System.Collections.Generic;
using SensenToolkit;
using SensenToolkit.Gizmosx;
using UnityEngine;

namespace Pakpak3D
{
    public class GridBoard : MonoBehaviour
    {
        [SerializeField] private float _cellSize = 1f;
        [SerializeField] private int _cellCountX = 28;
        [SerializeField] private int _cellCountY = 36;
        [SerializeField] private int _cellCountHeight = 5;
        private Vector3? _origin;
        private Vector3 Origin => _origin ??= transform.position
                + Vector3.up * (transform.localScale.y * 0.5f + _cellSize * 0.5f)
                + Vector3.right * -(_cellSize * 0.5f * ((_cellCountX + 1) % 2))
                + Vector3.forward * -(_cellSize * 0.5f * ((_cellCountY + 1) % 2));

        private void Start()
        {
            _origin = null;
        }

        public Vector3Int GetClosestCell(Vector3 position)
        {
            Vector3 originToCell = position - Origin;
            return new Vector3Int(
                x: Mathf.RoundToInt(originToCell.x / _cellSize),
                y: Mathf.RoundToInt(originToCell.z / _cellSize),
                z: Mathf.RoundToInt(originToCell.y / _cellSize)
            );
        }

        public Vector3 Cell3DToPosition(Vector3Int cell3d)
        {
            return Origin + cell3d.XZY().AsVector3Float() * _cellSize;
        }

        public Vector2 Cell2DToPosition(Vector2Int cell2d)
        {
            return Cell3DToPosition(cell2d.XY0()).XZ();
        }

        private void OnDrawGizmos()
        {
            _origin = null;
            using (Gizmosx.Color(Color.red))
            {
                int minCellX = -Mathf.CeilToInt(_cellCountX / 2f - 1);
                int maxCellX = _cellCountX / 2;
                int minCellY = -Mathf.CeilToInt(_cellCountY / 2f - 1);
                int maxCellY = _cellCountY / 2;
                for (int cellX = minCellX; cellX <= maxCellX; cellX++)
                {
                    for (int cellY = minCellY; cellY <= maxCellY; cellY++)
                    {
                        for (int cellZ = 0; cellZ < _cellCountHeight; cellZ++)
                        {
                            Vector3Int cell3d = new(cellX, cellY, cellZ);
                            Vector3 center = Cell3DToPosition(cell3d);
                            Gizmos.DrawWireCube(center, Vector3.one * _cellSize);
                        }
                    }
                }
            }
        }
    }
}
