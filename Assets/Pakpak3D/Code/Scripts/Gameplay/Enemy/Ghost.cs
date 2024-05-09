using LnxArch;
using UnityEngine;

namespace Pakpak3D
{
    public class Ghost : MonoBehaviour
    {
        [SerializeField] private GhostTypeSO _ghostType;
        public bool IsScared => _fsm.Scared.IsActive;
        public GhostTypeSO GhostType => _ghostType;

        private EnemyFSMMachine _fsm;

        [LnxInit]
        private void Init(EnemyFSMMachine fsm)
        {
            _fsm = fsm;
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
