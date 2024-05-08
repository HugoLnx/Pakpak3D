using System;
using LnxArch;
using UnityEngine;

namespace Pakpak3D
{
    public class Ghost : MonoBehaviour
    {
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
    }
}
