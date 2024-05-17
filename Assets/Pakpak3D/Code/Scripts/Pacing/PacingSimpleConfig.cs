using System.Collections.Generic;
using System.Linq;
using EasyButtons;
using UnityEngine;

namespace SensenToolkit
{
    [CreateAssetMenu(fileName = "PacingSimpleConfig", menuName = "Sensen/Pacing/ConfigSimple")]
    public class PacingSimpleConfig : ScriptableObject
    {
        [field: SerializeField]
        public List<PacingAttributeDefinition> Attributes { get; private set; }

        [field: SerializeField]
        public CurveConfig Curve { get; private set; }

        [SerializeField]
        [Tooltip("0 = TurnOff. Turn this off if you won't call GetValueSet a lot (like every frame), otherwise it'll generate pressure on Unity's Garbage Collector.")]
        private int _amountOfReusedValueSetReferences = 20;
        private MostRecentlyUsedPooledCache<float, PacingValueSet> _valueSetCache;

        private void OnEnable()
        {
            _valueSetCache = new MostRecentlyUsedPooledCache<float, PacingValueSet>(
                factory: () => new PacingValueSet(),
                minSize: 0,
                maxCreations: _amountOfReusedValueSetReferences,
                prefill: false
            );
        }

        [Button]
        private void LogPreview()
        {
            OnEnable();
            List<float> tList = new() { 0f, 0.25f, 0.5f, 0.75f, 1f };
            var valueSetList = tList
                .Select(t => GetValueSet(t))
                .ToList();
            List<string> lines = new();
            lines.Add($"[PacingSimple] Preview");
            lines.Add($"{"t",-30}: {string.Join(", ", tList.Select(t => $"{t,-7:0.00}"))}");
            lines.Add("---");
            foreach (PacingAttributeValue value in valueSetList.First().All)
            {
                lines.Add($"{value.Definition.Name,-30}: {string.Join(", ", valueSetList.Select(v => $"{v.Get(value.Definition).Value,-7:0.00}"))}");
            }
            Debug.Log(string.Join("\n", lines));
        }

        public PacingValueSet GetValueSet(float inputT)
        {
            bool isNew = true;
            PacingValueSet valueSet = _amountOfReusedValueSetReferences > 0
                ? _valueSetCache.Get(inputT, out isNew)
                : new PacingValueSet();
            if (isNew)
            {
                float t = Curve.Evaluate(inputT);
                foreach (PacingAttributeDefinition definition in Attributes)
                {
                    float lerpedValue = definition.GetLerpedValue(t);
                    valueSet.Add(new PacingAttributeValue(definition, lerpedValue));
                }
            }
            return valueSet;
        }
    }
}
