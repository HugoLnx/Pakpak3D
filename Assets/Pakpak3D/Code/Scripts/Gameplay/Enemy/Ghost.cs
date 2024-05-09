using System;
using LnxArch;
using UnityEngine;

namespace Pakpak3D
{
    public class Ghost : MonoBehaviour
    {
        public bool IsScared => _fsm.Scared.IsActive;

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
