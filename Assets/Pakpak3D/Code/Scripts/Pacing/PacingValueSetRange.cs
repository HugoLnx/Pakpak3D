using SensenToolkit;
using UnityEngine;

namespace SensenToolkit
{
    public class PacingValueSetRange
    {
        public PacingValueSet Start { get; private set; }
        public PacingValueSet End { get; private set; }

        private CurveConfig _defaultCurve;

        public PacingValueSetRange(PacingValueSet start, PacingValueSet end, CurveConfig defaultCurve = null)
        {
            Start = start;
            End = end;
            _defaultCurve = defaultCurve;
        }

        public PacingValueSet Lerp(
            float t,
            CurveConfig curve = null,
            PacingValueSet setBuffer = null
        )
        {
            setBuffer ??= new PacingValueSet();
            setBuffer.Clear();
            curve ??= _defaultCurve;
            if (curve != null)
            {
                t = curve.Evaluate(t);
            }
            foreach (PacingAttributeValue startValue in Start.All)
            {
                PacingAttributeDefinition definition = startValue.Definition;
                PacingAttributeValue endValue = End.Get(definition);
                float lerpedRawValue = Mathf.Lerp(startValue.Value, endValue.Value, t);
                // Debug.Log($"Lerp {startValue.Definition.Name} from {startValue.Value} to {End.Get(startValue.Definition).Value} at {t}. Result: {lerpedRawValue}");
                PacingAttributeValue lerpedValue = new(definition, lerpedRawValue);
                setBuffer.Add(lerpedValue);
            }
            return setBuffer;
        }
    }
}
