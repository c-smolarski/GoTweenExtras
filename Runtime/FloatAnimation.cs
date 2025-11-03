using UnityEngine;

namespace GoTween.Extras
{
    public class FloatAnimation : MonoBehaviour, IAnimatable
    {
        [SerializeField] private float _startValue;
        [SerializeField] private AnimationData<float> _animationIn;
        [SerializeField] private AnimationData<float> _animationOut;

        private Tween _tween;
        private float _value;

        private void Awake()
        {
            _value = _startValue;
        }

        protected virtual void SetValue(float newValue)
        {
            _value = newValue;
        }

        public WaitForTween PlayAnimation(bool pIn)
        {
            if (_tween?.IsPlaying ?? false)
                _tween.Kill();
            _tween = new(this);
            _tween.TweenProperty(f => SetValue(f), _value, pIn ? _animationIn : _animationOut);
            _tween.Start();
            return new(_tween);
        }
    }
}
