using SmolTags;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GoTween.Extras
{
    [ExecuteAlways]
    public class AnimationGroup : MonoBehaviour, IAnimatable
    {
        public bool InAnimation => _animationCoroutine != null;

        #region Serialized Fields
        [Header("AutoStart")]
        [SerializeField] private bool _autoStartOnEnable = true;
        [SerializeField, ShowIf(nameof(_autoStartOnEnable), false, resetValue = false)] private bool _autoStartOnStart;

        [Header("Animatables")]
        [SerializeField] private bool _disableOnOut;
        [SerializeField] private bool _autoFetchChildren = true;
        [SerializeField, ShowIf(nameof(_autoFetchChildren), true, resetIfDisabled = false)] private bool _onlyDirectChildren = true;
        [SerializeField, ShowIf(nameof(_autoFetchChildren), true, resetIfDisabled = false)] private bool _fetchInactives = true;
        [SerializeField, ShowIf(nameof(_autoFetchChildren), false, hide = false)] private Object[] _animatables;

        [Header("Selectables")]
        [SerializeField] private bool _disableSelectablesDuringAnim = true;
        [SerializeField, HideInInspector] private Selectable[] _selectables;
        #endregion

        private Coroutine _animationCoroutine;

        //----EDITOR METHODS----//

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_autoFetchChildren)
                _animatables = AutoFetchAnimatables();
            else if (_animatables != null)
            {
                for (int i = 0; i < _animatables.Length; i++)
                    if (_animatables[i] && _animatables[i] is not IAnimatable)
                    {
                        Debug.LogWarning($"{_animatables[i].name} is not implementing {nameof(IAnimatable)}.");
                        _animatables[i] = null;
                    }
            }
            _selectables = GetComponentsInChildren<Selectable>();
        }

        private void Reset()
            => OnValidate();

        private void Update()
        {
            if (!Application.isPlaying)
                OnValidate();
        }

        private Object[] AutoFetchAnimatables()
        {
            List<Object> animatableList = new();
            foreach (IAnimatable animatable in GetComponentsInChildren<IAnimatable>(_fetchInactives))
                if (animatable is MonoBehaviour && (MonoBehaviour)animatable != this && (!_onlyDirectChildren || ((MonoBehaviour)animatable).transform.parent == transform))
                    animatableList.Add((Object)animatable);
            return animatableList.ToArray();
        }
#endif

        private void Start()
        {
            if (!_autoStartOnEnable && _autoStartOnStart)
                PlayAnimation(true);
        }

        private void OnEnable()
        {
            if (_autoStartOnEnable)
                PlayAnimation(true);
        }

        /// <summary>
        /// Used for UnityEvents.
        /// </summary>
        /// <param name="pIn"></param>
        public void Play(bool pIn)
            => PlayAnimation(pIn);

        public WaitForTween PlayAnimation(bool pIn)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return null;
#endif
            gameObject.SetActive(true);

            if (_disableSelectablesDuringAnim)
                foreach (Selectable selectable in _selectables)
                    selectable.interactable = pIn;

            List<WaitForTween> lAwaiters = new();
            foreach (IAnimatable lAnimatable in _animatables)
                lAwaiters.Add(lAnimatable.PlayAnimation(pIn));

            if (_animationCoroutine != null)
                StopCoroutine(_animationCoroutine);
            _animationCoroutine = StartCoroutine(VisibilityCoroutine(lAwaiters, _disableOnOut && !pIn));

            Tween lTween = new(this);
            lTween.TweenAction(() => TweenTimer(lTween), Mathf.Infinity);
            return new(lTween);
        }

        private IEnumerator VisibilityCoroutine(List<WaitForTween> pAwaiters, bool pDisableOnEnd)
        {
            foreach (WaitForTween Awaiter in pAwaiters)
                yield return Awaiter;

            _animationCoroutine = null;
            if (pDisableOnEnd)
                gameObject.SetActive(false);
        }

        private void TweenTimer(Tween tween)
        {
            if (!InAnimation)
                tween.Kill();
        }
    }
}
