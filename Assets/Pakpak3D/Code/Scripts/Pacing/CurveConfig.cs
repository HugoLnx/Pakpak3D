using System;
using SensenToolkit.Lerp;
using UnityEngine;

namespace SensenToolkit
{
    [Serializable]
    public class CurveConfig
    {
        [SerializeField] private DG.Tweening.Ease _ease;
        [SerializeField] private bool _useAnimationCurve;
        [SerializeField] private AnimationCurve _animationCurve;
        [SerializeField] private bool _inverseT = false;
        [SerializeField] private float _scaleT = 1f;
        [SerializeField] private float _offsetT = 0f;

        private EaseLerper Lerper => _lerper ??= new();
        private EaseLerper _lerper;

        public float Evaluate(float t)
        {
            t = (t / _scaleT) - _offsetT;
            t = _inverseT ? 1 - t : t;
            if (_useAnimationCurve)
            {
                return _animationCurve.Evaluate(t);
            }
            else
            {
                return Lerper.Lerp(0f, 1f, t, _ease);
            }
        }
    }
}
