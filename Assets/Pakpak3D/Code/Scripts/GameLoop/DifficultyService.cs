using System;
using LnxArch;
using SensenToolkit;
using UnityEngine;

namespace Pakpak3D
{
    // public class GraphNode<T> : IGraphNode
    // {
    //     private HashSet<IGraphNode> _neighbors = new();
    //     public IEnumerable<IGraphNode> Neighbors => _neighbors;
    //     public bool Visited { get; set; }
    //     public T Value { get; set; }
    //     public void AddNeighbor(IGraphNode neighbor)
    //     {
    //         _neighbors.Add(neighbor);
    //     }
    // }

    [LnxService]
    public class DifficultyService : MonoBehaviour
    {
        [SerializeField] private float _pakpakReferenceSpeed = 5f;
        [SerializeField] private PacingMultiLevelConfig _pacingConfig;
        [SerializeField] private PacingAttributeDefinition _gameSpeedAttr;
        [SerializeField] private PacingAttributeDefinition _ghostSpeedAttr;
        [SerializeField] private PacingAttributeDefinition _huntCoverageAttr;
        [SerializeField] private PacingAttributeDefinition _scatterCoverageAttr;
        [SerializeField] private PacingAttributeDefinition _coinsCountAttr;
        [SerializeField] private PacingAttributeDefinition _coinsMazeCoverageAttr;
        private GameLoopService _loopService;
        private HuntInstructorService _huntService;
        private Pakpak _pakpak;
        private Ghost[] _enemies;

        private int LevelInx => _loopService.CurrentLevel - 1;
        private int WaveInx => _loopService.CurrentWaveInx;

        [LnxInit]
        private void Init(GameLoopService loopService, HuntInstructorService huntService)
        {
            _loopService = loopService;
            _huntService = huntService;
            _loopService.OnBeforeWaveStart += BeforeWaveStart;
            _pakpak = FindObjectOfType<Pakpak>();
            _enemies = FindObjectsOfType<Ghost>();
        }

        private void BeforeWaveStart()
        {
            float wavePercent = (float)WaveInx / GameLoopService.MaxLevel;
            PacingValueSetRange levelRange = _pacingConfig.GetLevelRange(LevelInx + 1);
            PacingValueSet values = levelRange.Lerp(wavePercent);
            SetupSpeeds(values);
            SetupHuntScatterDurations(values);
            SetupSpawningSettings(values);
        }

        private void SetupSpeeds(PacingValueSet values)
        {
            float gameSpeedPacing = values.Get(_gameSpeedAttr).Value;
            float ghostSpeedPacing = values.Get(_ghostSpeedAttr).Value;

            float pakpakSpeed = _pakpakReferenceSpeed * gameSpeedPacing;
            float ghostSpeed = pakpakSpeed * ghostSpeedPacing;

            _pakpak.SetSpeed(pakpakSpeed);
            foreach (Ghost enemy in _enemies)
            {
                enemy.SetSpeed(ghostSpeed);
            }
        }

        private void SetupHuntScatterDurations(PacingValueSet values)
        {
            float estimatedFullCoverageDistance = 25f;
            float huntCoverage = values.Get(_huntCoverageAttr).Value;
            float scatterCoverage = values.Get(_scatterCoverageAttr).Value;

            Ghost ghost = _enemies[0];
            float huntDuration = (estimatedFullCoverageDistance * huntCoverage) / ghost.Speed;
            float scatterDuration = (estimatedFullCoverageDistance * scatterCoverage) / ghost.Speed;

            _huntService.SetAttackDuration(huntDuration);
            _huntService.SetScatterDuration(scatterDuration);
        }

        private void SetupSpawningSettings(PacingValueSet values)
        {
            int coinsCount = Mathf.RoundToInt(values.Get(_coinsCountAttr).Value);
            float coinsMazeCoverage = values.Get(_coinsMazeCoverageAttr).Value;

            _loopService.SetCoinsSpawnAmount(coinsCount);
            _loopService.SetCoinsSpawnMazeCoverage(coinsMazeCoverage);
        }
    }
}
