using System;
using System.Collections;
using System.Linq;
using EasyButtons;
using LnxArch;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.TestTools;

namespace Pakpak3D
{
    [LnxService]
    public class GameLoopService : MonoBehaviour
    {
        [SerializeField] private WaveConfig[] _waveConfigs;
        [SerializeField] private int _coinCountToSpawnSpecialCoin = 2;
        private CollectableSpawnerService _spawner;
        private bool _finishedWave = false;

        [LnxInit]
        private void Init(CollectableSpawnerService spawner)
        {
            _spawner = spawner;
            _spawner.OnAllRequiredCollected += OnAllRequiredCollected;
        }

        [Button]
        public void StartLoop()
        {
            StartCoroutine(Loop());
        }

        private IEnumerator Loop()
        {
            int maxLevel = 3;
            for (int level = 1; level <= maxLevel; level++)
            {
                yield return SpawnLevel(level);
            }
        }

        private IEnumerator SpawnLevel(int level)
        {
            Debug.Log($"Level {level} started");
            int waveInx = 0;
            foreach (WaveConfig waveConfig in _waveConfigs)
            {
                waveConfig.Inx = waveInx;
                yield return SpawnWave(waveConfig);
                waveInx++;
            }
        }

        private IEnumerator SpawnWave(WaveConfig waveConfig)
        {
            Debug.Log($"Wave {waveConfig.Inx} started");
            int pointCount = 5;
            int specialPointCount = pointCount / _coinCountToSpawnSpecialCoin;
            float mazeCoverage = 0.5f;
            CollectableSpawningBatchConfigSO batchCfg = waveConfig.CreateBatchConfig(
                pointCount: pointCount,
                specialPointCount: specialPointCount,
                mazeCoverage: mazeCoverage
            );
            _spawner.SpawnBatch(batchCfg);
            _finishedWave = false;
            yield return new WaitUntil(() => _finishedWave);
        }

        private void OnAllRequiredCollected()
        {
            _finishedWave = true;
        }
    }
}
