using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pakpak3D
{
    [Serializable]
    public class WaveConfig
    {
        [field: SerializeField] public Collectable PointPrefab { get; private set; }
        [field: SerializeField] public Collectable SpecialPointPrefab { get; private set; }
        [field: SerializeField] public float ScoreMultiplier { get; private set; } = 1.0f;
        public int Inx { get; set; }

        public CollectableSpawningBatchConfigSO CreateBatchConfig(int pointCount, int specialPointCount, float mazeCoverage)
        {
            List<CollectableSpawningConfig> configs = new();
            PointPrefab.GetComponentInChildren<PointCollectable>().SetScoreMultiplier(ScoreMultiplier);
            configs.Add(new CollectableSpawningConfig(PointPrefab, pointCount, isOptionalToCollect: false));
            if (specialPointCount > 0)
            {
                SpecialPointPrefab.GetComponentInChildren<PointCollectable>().SetScoreMultiplier(ScoreMultiplier);
                configs.Add(new CollectableSpawningConfig(SpecialPointPrefab, specialPointCount, isOptionalToCollect: true));
            }
            return CollectableSpawningBatchConfigSO.Create(configs, mazeCoverage);
        }
    }
}
