using UnityEngine;

namespace Pakpak3D
{
    public static class Grid3DDirection
    {
        public static bool IsValidDirection(Vector3Int v)
        {
            if (v == Vector3Int.zero) return false;
            if (v.x > 1 || v.x < -1) return false;
            if (v.y > 1 || v.y < -1) return false;
            if (v.z > 1 || v.z < -1) return false;

            int nonZeroCount = 0;
            nonZeroCount += v.x == 0 ? 0 : 1;
            nonZeroCount += v.y == 0 ? 0 : 1;
            nonZeroCount += v.z == 0 ? 0 : 1;
            return nonZeroCount == 1;
        }
    }
}
