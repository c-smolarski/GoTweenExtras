using System;
using System.Collections;
using UnityEngine;

namespace GoTween.Extras
{
    [RequireComponent(typeof(RectTransform))]
    public class MovingPanel : MonoBehaviour
    {
        public event Action FinishedMoving;

        [field: SerializeField] protected AnimationData<Vector2>[] Steps { get; private set; } = new AnimationData<Vector2>[2];

        [SerializeField] private int _currentStep;
        [SerializeField, HideInInspector] private RectTransform _rectTransform;
        private Tween _moveTween;

        protected virtual void OnValidate()
        {
            _rectTransform = GetComponent<RectTransform>();
            _currentStep = LoopIndex(_currentStep, Steps);
            if (_rectTransform.anchoredPosition != Steps[_currentStep].endValue)
            {
                _rectTransform.anchoredPosition = Steps[_currentStep].endValue;
                Debug.LogWarning($"{gameObject.name}: Setting position to match current step's position");
            }
        }

        public void MoveToNextStep()
        {
            MoveToStep(_currentStep + 1);
        }

        public void MoveToPrevStep()
        {
            MoveToStep(_currentStep - 1);
        }

        public WaitForTween PathToStep(int stepId)
        {
            AnimationData<Vector2> step, prevStep = new();
            int newStepIdx, targetStep = LoopIndex(stepId, Steps);

            if (targetStep < _currentStep)
                targetStep += Steps.Length;

            ResetTween();

            prevStep.endValue = _rectTransform.anchoredPosition;
            for (int i = _currentStep + 1; i <= targetStep; i++)
            {
                newStepIdx = LoopIndex(i, Steps);
                step = Steps[newStepIdx];

                _moveTween.TweenCallback(() => _currentStep = newStepIdx);
                _moveTween.TweenProperty(p => _rectTransform.anchoredPosition = p, prevStep.endValue, step);
                prevStep = step;
            }
            _moveTween.TweenCallback(() => FinishedMoving?.Invoke());
            return new WaitForTween(_moveTween);
        }

        private void MoveToStep(int stepId)
        {
            _currentStep = LoopIndex(stepId, Steps);
            MoveToStep(Steps[_currentStep]);
        }

        private void MoveToStep(AnimationData<Vector2> step)
        {
            ResetTween();
            _moveTween.TweenProperty(p => Debug.Log(_rectTransform.anchoredPosition = p), _rectTransform.anchoredPosition, step);
            _moveTween.TweenCallback(() => FinishedMoving?.Invoke());
        }

        private void ResetTween()
        {
            if (_moveTween?.IsPlaying ?? false)
                _moveTween.Kill();
            _moveTween = new(this);
        }

        protected static int LoopIndex(int value, IList array)
        {
            return (int)Mathf.Repeat(value, array.Count);
        }
    }
}
