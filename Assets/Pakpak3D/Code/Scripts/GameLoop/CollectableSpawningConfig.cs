using System;
using UnityEngine;

namespace Pakpak3D
{
    [Serializable]
    public class CollectableSpawningConfig
    {
        [field: SerializeField]
        public Collectable Prefab { get; private set; }

        [field: SerializeField]
        public int Amount { get; private set; }

        [field: SerializeField]
        public bool IsOptionalToCollect { get; private set; }

        public bool IsRequiredToCollect => !IsOptionalToCollect;

        public CollectableSpawningConfig(Collectable prefab, int amount, bool isOptionalToCollect)
        {
            Prefab = prefab;
            Amount = amount;
            IsOptionalToCollect = isOptionalToCollect;
        }

        public CollectableSpawningConfig Clone()
        {
            return new CollectableSpawningConfig(
                prefab: Prefab,
                amount: Amount,
                isOptionalToCollect: IsOptionalToCollect
            );
        }
    }
}
