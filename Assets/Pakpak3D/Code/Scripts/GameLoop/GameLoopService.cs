using System;
using System.Collections;
using System.Linq;
using EasyButtons;
using LnxArch;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pakpak3D
{
    [LnxService]
    public class GameLoopService : MonoBehaviour
    {
        public const int MaxLevel = 3;
        [SerializeField] private WaveConfig[] _waveConfigs;
        [SerializeField] private float _specialCoinChance = 0.35f;
        [field: SerializeField] public int CurrentLevel { get; private set; } = 1;
        [field: SerializeField] public int CurrentWaveInx { get; private set; }
        [SerializeField] private ParticleSystem _gameOverVfx;
        private CollectableSpawnerService _spawner;
        private bool _finishedWave = false;
        private int _amountPointsSpawn = 5;
        private float _pointsSpawnMazeCoverage = 0.25f;
        public event Action OnBeforeWaveStart;

        [LnxInit]
        private void Init(CollectableSpawnerService spawner)
        {
            _spawner = spawner;
            _spawner.OnAllRequiredCollected += OnAllRequiredCollected;
        }

        private void Start()
        {
            StartLoop();
        }

        private void Update()
        {
            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.R))
            {
                Time.timeScale = 1f;
                RestartScene();
            }
        }

        [Button]
        public void StartLoop()
        {
            StartCoroutine(Loop());
        }

        public void SetCoinsSpawnAmount(int amount)
        {
            _amountPointsSpawn = amount;
        }

        public void SetCoinsSpawnMazeCoverage(float coverage)
        {
            _pointsSpawnMazeCoverage = coverage;
        }

        private IEnumerator Loop()
        {
            for (int level = 1; level <= MaxLevel; level++)
            {
                yield return SpawnLevel(level);
            }

            Time.timeScale = 0f;
            for (int i = 0; i < 3; i++)
            {
                _gameOverVfx.Play();
                yield return new WaitForSecondsRealtime(0.75f);
            }
        }

        private IEnumerator SpawnLevel(int level)
        {
            CurrentLevel = level;
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
            CurrentWaveInx = waveConfig.Inx;
            Debug.Log($"Wave {CurrentWaveInx} started");
            OnBeforeWaveStart?.Invoke();
            waveConfig.StartVfx.Play();
            int pointCount = _amountPointsSpawn;
            float mazeCoverage = _pointsSpawnMazeCoverage;
            int specialPointCount = GetSpecialCoinAmount();
            CollectableSpawningBatchConfigSO batchCfg = waveConfig.CreateBatchConfig(
                pointCount: pointCount,
                specialPointCount: specialPointCount,
                mazeCoverage: mazeCoverage
            );
            _spawner.SpawnBatch(batchCfg);
            _finishedWave = false;
            yield return new WaitUntil(() => _finishedWave);
        }

        private int GetSpecialCoinAmount()
        {
            int amount = 0;
            amount += UnityEngine.Random.value < _specialCoinChance ? 1 : 0;
            amount += UnityEngine.Random.value < _specialCoinChance ? 1 : 0;
            return amount;
        }

        private void OnAllRequiredCollected()
        {
            _finishedWave = true;
        }

        private void RestartScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
