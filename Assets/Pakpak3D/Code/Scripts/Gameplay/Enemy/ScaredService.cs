using System.Collections;
using System.Collections.Generic;
using LnxArch;
using UnityEngine;

namespace Pakpak3D
{
    [LnxService]
    public class ScaredService : MonoBehaviour
    {
        [SerializeField] private float _scareDuration = 10f;
        private List<Ghost> _ghosts;

        private List<Ghost> Ghosts => _ghosts ??= FindGhosts();

        public void ScareGhosts()
        {
            foreach (Ghost ghost in Ghosts)
            {
                ghost.GetScared();
            }

            StopAllCoroutines();
            StartCoroutine(WaitToStopScaringGhosts());
        }

        private void StopScareGhosts()
        {
            foreach (Ghost ghost in Ghosts)
            {
                ghost.EndScared();
            }
        }

        private IEnumerator WaitToStopScaringGhosts()
        {
            yield return new WaitForSeconds(_scareDuration);
            StopScareGhosts();
        }

        private List<Ghost> FindGhosts()
        {
            if (_ghosts == null)
            {
                _ghosts = new List<Ghost>();
                Ghost[] ghosts = GameObject.FindObjectsOfType<Ghost>();
                foreach (Ghost ghost in ghosts)
                {
                    _ghosts.Add(ghost);
                }
            }
            return _ghosts;
        }
    }
}
