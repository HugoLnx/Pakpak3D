using System.Collections.Generic;
using System.Linq;
using EasyButtons;
using UnityEngine;

namespace SensenToolkit
{
    [CreateAssetMenu(fileName = "PacingAttribute", menuName = "Sensen/Pacing/Attribute")]
    public class PacingAttributeDefinition : ScriptableObject
    {
        public string Name => this.name;
        [field: SerializeField]
        [field: Multiline]
        public string Description { get; private set; }

        [field: SerializeField]
        public float BeginValue { get; private set; }

        [field: SerializeField]
        public float EndValue { get; private set; }

        [field: SerializeField]
        public CurveConfig Curve { get; private set; }

        public float GetLerpedValue(float t)
        {
            t = Curve.Evaluate(t);
            return Mathf.Lerp(BeginValue, EndValue, t);
        }

        // public float GetLerpedValueUnclamped(float t)
        // {
        //     t = Curve.Evaluate(t);
        //     return Mathf.LerpUnclamped(BeginValue, EndValue, t);
        // }

        [Button]
        private void LogPreview()
        {
            List<float> inTList = new() { 0f, 0.25f, 0.5f, 0.75f, 1f };
            var tList = inTList
                .Select(t => Curve.Evaluate(t))
                .ToList();
            var lerpedList = inTList
                .Select(t => GetLerpedValue(t))
                .ToList();
            // var lerpedUnclampedList = inTList
            //     .Select(t => GetLerpedValueUnclamped(t))
            //     .ToList();
            List<string> lines = new();
            lines.Add($"[PacingSimple] Preview");
            lines.Add($"{"input t",-30}: {string.Join(", ", inTList.Select(v => $"{v,-7:0.00}"))}");
            lines.Add("---");
            lines.Add($"{"evaluated t",-30}: {string.Join(", ", tList.Select(v => $"{v,-7:0.00}"))}");
            lines.Add($"{"lerped value",-30}: {string.Join(", ", lerpedList.Select(v => $"{v,-7:0.00}"))}");
            // lines.Add($"{"lerped UNCLAMPED value",-30}: {string.Join(", ", lerpedUnclampedList.Select(v => $"{v,-7:0.00}"))}");
            Debug.Log(string.Join("\n", lines));
        }
    }
}
