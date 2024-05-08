using System.Collections;
using EasyButtons;
using LnxArch;
using UnityEngine;

namespace Pakpak3D
{
    public class GhostChaseTrack : MonoBehaviour
    {
        [SerializeField] private Transform _track;
        [SerializeField] private float _distanceToReachTarget = 0.65f;
        private GhostChase _chase;
        private bool _isChasing;

        [LnxInit]
        private void Init(GhostChase chase)
        {
            _chase = chase;
        }

        [Button]
        public void BeginChase()
        {
            if (_isChasing) return;
            StartCoroutine(ChaseCoroutine());
        }

        [Button]
        public void StopChase()
        {
            if (!_isChasing) return;
            StopAllCoroutines();
            _isChasing = false;
        }

        private IEnumerator ChaseCoroutine()
        {
            _isChasing = true;
            int childCount = _track.childCount;
            var waypoints = new Transform[childCount];
            for (int i = 0; i < childCount; i++)
            {
                waypoints[i] = _track.GetChild(i);
            }

            int inx = 0;
            while (_isChasing)
            {
                _chase.SetChasing(waypoints[inx], ignoreNavmesh: true);
                yield return new WaitWhile(() => _isChasing && _chase.GetDistanceToTarget() > _distanceToReachTarget);
                inx = (inx + 1) % childCount;
            }

        }
    }
}
