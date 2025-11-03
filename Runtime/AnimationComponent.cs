using UnityEngine;
using UnityEngine.Events;

namespace GoTween.Extras
{
    public class AnimationComponent : FloatAnimation, IAnimatable
    {
        [field: SerializeField] public UnityEvent<float> OnAnimationUpdate { get; private set; } = new();

        protected override void SetValue(float newValue)
        {
            base.SetValue(newValue);
            OnAnimationUpdate?.Invoke(newValue);
        }
    }
}
