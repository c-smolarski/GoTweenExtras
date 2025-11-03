using UnityEngine;

namespace GoTween.Extras
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CanvasFader : FloatAnimation, IAnimatable
    {
        [SerializeField, HideInInspector] private CanvasGroup _canvasGroup;

        private void OnValidate()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        protected override void SetValue(float newValue)
        {
            base.SetValue(newValue);
            _canvasGroup.alpha = Mathf.Clamp01(newValue);
        }
    }
}
