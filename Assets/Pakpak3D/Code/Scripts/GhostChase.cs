using System;
using System.Collections;
using System.Collections.Generic;
using LnxArch;
using UnityEngine;

namespace Pakpak3D
{
    public class GhostChase : MonoBehaviour
    {
        [SerializeField] private Vector2Int _initialDirection;
        [SerializeField] private int _snapsToSwitchDirection = 10;
        private Vector2Int _direction;
        private int _snapCount = 0;
        private FlyingMover _flying;

        [LnxInit]
        private void Init(FlyingMover flying)
        {
            _flying = flying;
            _flying.OnSnapInCell += SnapInCellCallback;
            TurnTo(_initialDirection);
        }

        private void SnapInCellCallback(Vector3Int cell)
        {
            _snapCount++;
            if (_snapCount > _snapsToSwitchDirection)
            {
                TurnTo(_direction * -1);
            }
        }

        private void TurnTo(Vector2Int direction)
        {
            _direction = direction;
            _flying.TurnTo(direction);
            _snapCount = 0;
        }
    }
}
