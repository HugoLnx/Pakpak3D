using System.Collections.Generic;
using EasyButtons;
using LnxArch;
using SensenToolkit;
using UnityEngine;

namespace Pakpak3D
{
    [LnxService]
    public class CollectableSpawnerService : MonoBehaviour
    {
        [SerializeField] private Transform _collectablesParent;
        public Transform CollectablesParent => _collectablesParent != null ? _collectablesParent : transform;
        private MazeService _mazeService;
        private GridBoard _grid;
        private readonly HashSet<Collectable> _allCollectables = new();
        private readonly HashSet<Collectable> _requiredCollectables = new();

        public event System.Action OnAllRequiredCollected;
        public event System.Action<Collectable> OnCollected;

        [LnxInit]
        private void Init(GridBoard grid, MazeService mazeService)
        {
            _mazeService = mazeService;
            _grid = grid;
        }

        [Button]
        public void SpawnBatch(CollectableSpawningBatchConfigSO batchConfig)
        {
            _requiredCollectables.Clear();
            HashSet<Vector3Int> cells = _mazeService
                .GetSurfaceCellsRandomAreaSubset(coverage: batchConfig.MazeCoverage);
            Assertx.IsNotEmpty(cells);
            var cellsStack = cells
                .Shuffle()
                .ToStack();
            foreach (CollectableSpawningConfig config in batchConfig.Configs)
            {
                bool isRequired = config.IsRequiredToCollect;
                for (int i = 0; i < config.Amount; i++)
                {
                    if (cellsStack.Count == 0)
                    {
                        break;
                    }
                    Vector3Int cell = cellsStack.Pop();
                    Vector3 cellPosition = _grid.Cell3DToPosition(cell);
                    Collectable collectable = Instantiate(
                        original: config.Prefab,
                        position: cellPosition,
                        rotation: Quaternion.identity,
                        parent: CollectablesParent
                    );
                    InitService.Instance.InitBehaviour(collectable);
                    _allCollectables.Add(collectable);
                    if (isRequired)
                    {
                        _requiredCollectables.Add(collectable);
                    }
                    collectable.OnCollected += () => OnCollectableCollected(collectable, config);
                }
            }
        }

        private void OnCollectableCollected(Collectable collectable, CollectableSpawningConfig config)
        {
            bool isRequired = config.IsRequiredToCollect;
            if (isRequired)
            {
                _requiredCollectables.Remove(collectable);
            }
            _allCollectables.Remove(collectable);
            OnCollected?.Invoke(collectable);
            if (_requiredCollectables.Count == 0)
            {
                OnAllRequiredCollected?.Invoke();
            }
            Debug.Log($"Collectable {collectable.name} collected. Required: {isRequired}. Remaining required: {_requiredCollectables.Count}");
        }
    }
}
