using System;
using System.Collections.Generic;
using EasyButtons;
using SensenToolkit.Lerp;
using UnityEngine;

namespace SensenToolkit
{
    [CreateAssetMenu(fileName = "PacingMultiLevelConfig", menuName = "Sensen/Pacing/ConfigMultiLevel")]
    public class PacingMultiLevelConfig : ScriptableObject
    {
        private const float MIN_VALUE = 0f;
        private const float MAX_VALUE = 1f;
        [field: SerializeField]
        public List<PacingAttributeDefinition> Attributes { get; private set; }

        [field: SerializeField]
        [field: Min(0)]
        public int LevelsCount { get; private set; }

        [field: SerializeField]
        [field: Range(0, 1)]
        [Tooltip("The value for the easy part of the last level.")]
        public float EasyMax { get; private set; } = 0.6f;

        [field: SerializeField]
        [Tooltip("How the easy part of the levels will be paced. If null, it'll be linear.")]
        public CurveConfig EasyCurve { get; private set; }

        [field: SerializeField]
        [field: Range(0, 1)]
        [Tooltip("The value for the hard part of the first level.")]
        public float HardMin { get; private set; } = 0.35f;

        [field: SerializeField]
        [Tooltip("How the hard part of the levels will be paced. If null, it'll be linear.")]
        public CurveConfig HardCurve { get; private set; }

        [field: SerializeField]
        [Tooltip("How the inner part of the levels will be paced. If null, it'll be linear.")]
        public CurveConfig LevelInnerCurve { get; private set; }

        private PacingValueSetRange[] _levelValueRanges;

        private void OnEnable()
        {
            _levelValueRanges = new PacingValueSetRange[LevelsCount];
        }

        [Button]
        private void LogPreview(int level)
        {
            OnEnable();
            PacingValueSetRange valuesRange = GetLevelRange(level);
            PacingValueSet easyValueSet = valuesRange.Lerp(0f);
            PacingValueSet mediumValueSet = valuesRange.Lerp(0.5f);
            PacingValueSet hardValueSet = valuesRange.Lerp(1f);
            List<string> lines = new();
            lines.Add($"[PacingMultiLevel] Preview Level {level}");
            foreach (PacingAttributeDefinition definition in Attributes)
            {
                float easyValue = easyValueSet.Get(definition).Value;
                float mediumValue = mediumValueSet.Get(definition).Value;
                float hardValue = hardValueSet.Get(definition).Value;
                lines.Add(
                    $"{definition.Name,-30} Easy:{easyValue,-7:0.00} Medium:{mediumValue,-7:0.00} Hard:{hardValue,-7:0.00}"
                );
            }
            Debug.Log(string.Join("\n", lines));

        }

        public PacingValueSetRange GetLevelRange(int level)
        {
            CheckLevel(level);
            float easyT = GetTFor(
                level: level,
                min: MIN_VALUE,
                max: EasyMax,
                curve: EasyCurve
            );
            float hardT = GetTFor(
                level: level,
                min: HardMin,
                max: MAX_VALUE,
                curve: HardCurve
            );

            PacingValueSetRange range = CreateOrGetBufferRange(level);
            PopulateValueSet(range.Start, easyT);
            PopulateValueSet(range.End, hardT);

            return range;
        }

        private PacingValueSetRange CreateOrGetBufferRange(int level)
        {
            return _levelValueRanges[level - 1]
                ?? new PacingValueSetRange(new PacingValueSet(), new PacingValueSet(), LevelInnerCurve);
        }

        private void PopulateValueSet(PacingValueSet set, float t)
        {
            set.Clear();
            foreach (PacingAttributeDefinition attribute in Attributes)
            {
                float value = attribute.GetLerpedValue(t);
                set.Add(new(attribute, value));
            }
        }

        private float GetTFor(int level, float min, float max, CurveConfig curve)
        {
            float t = (level - 1f) / (LevelsCount - 1f);
            if (curve != null) t = curve.Evaluate(t);
            return Mathf.Lerp(min, max, t);
        }

        private void CheckLevel(int level)
        {
            if (level == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(level), level, "Level must start on 1");
            }
            else if (level < 0 || level > LevelsCount)
            {
                throw new ArgumentOutOfRangeException(nameof(level), level, $"Level must be between 1 and {LevelsCount}");
            }
        }
    }
}
