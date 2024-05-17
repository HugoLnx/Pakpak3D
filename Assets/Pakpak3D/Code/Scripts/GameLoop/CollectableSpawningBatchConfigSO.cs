using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pakpak3D
{
    [CreateAssetMenu(fileName = "SpawnBatch", menuName = "Pakpak3D/Spawning/Batch")]
    public class CollectableSpawningBatchConfigSO : ScriptableObject
    {
        [field: SerializeField]
        public CollectableSpawningConfig[] Configs { get; private set; }

        [field: SerializeField]
        [field: Range(0, 1)]
        public float MazeCoverage { get; private set; }

        public static CollectableSpawningBatchConfigSO Create(
            IEnumerable<CollectableSpawningConfig> configs,
            float mazeCoverage
        )
        {
            CollectableSpawningBatchConfigSO config = CreateInstance<CollectableSpawningBatchConfigSO>();
            config.Configs = configs.ToArray();
            config.MazeCoverage = mazeCoverage;
            return config;
        }

        public CollectableSpawningBatchConfigSO Clone()
        {
            var configs = new CollectableSpawningConfig[Configs.Length];
            for (int i = 0; i < Configs.Length; i++)
            {
                configs[i] = Configs[i].Clone();
            }
            return Create(configs, MazeCoverage);
        }
    }
}
