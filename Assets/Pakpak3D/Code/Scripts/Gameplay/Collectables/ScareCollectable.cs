using System.Collections;
using System.Collections.Generic;
using LnxArch;
using UnityEngine;

namespace Pakpak3D
{
    public class ScareCollectable : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _explosionVfx;
        private Collectable _collectable;
        private ScaredService _scaredService;

        [LnxInit]
        private void Init(Collectable collectable, ScaredService scaredService)
        {
            _collectable = collectable;
            _collectable.OnCollected += Collect;
            _scaredService = scaredService;
        }

        private void Collect()
        {
            _scaredService?.ScareGhosts();
            if (_explosionVfx != null)
            {
                // TODO: Change this to not generate garbage
                _explosionVfx.Play();
                _explosionVfx.transform.SetParent(null);
            }
        }
    }
}
