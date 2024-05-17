using LnxArch;
using UnityEngine;

namespace Pakpak3D
{
    public class Ghost : MonoBehaviour
    {
        [SerializeField] private GhostTypeSO _ghostType;
        public bool IsScared => _fsm.Scared.IsActive;
        public bool IsDangerous => !IsScared && !_fsm.Eaten.IsActive;
        public GhostTypeSO GhostType => _ghostType;
        public float Speed => _movement3d.Speed;

        private EnemyFSMMachine _fsm;
        private Grid3DMovement _movement3d;

        [LnxInit]
        private void Init(EnemyFSMMachine fsm, Grid3DMovement movement3d)
        {
            _fsm = fsm;
            _movement3d = movement3d;
        }

        public void SetSpeed(float speed)
        {
            _movement3d.SetSpeed(speed);
        }

        public void GetScared()
        {
            _fsm.SendMessage(_fsm.GetScared);
        }

        public void EndScared()
        {
            _fsm.SendMessage(_fsm.EndScared);
        }

        public void TouchedPlayer()
        {
            _fsm.SendMessage(_fsm.TouchedPlayer);
        }
    }
}
