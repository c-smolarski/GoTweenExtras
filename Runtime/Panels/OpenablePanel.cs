using UnityEngine;

namespace GoTween.Extras
{
    public class OpenablePanel : MovingPanel, IAnimatable
    {
        [SerializeField] private int _openedIdx;
        [SerializeField] private int _closedIdx = 1;

        protected override void OnValidate()
        {
            base.OnValidate();
            _openedIdx = LoopIndex(_openedIdx, Steps);
            _closedIdx = LoopIndex(_closedIdx, Steps);
            if (_openedIdx == _closedIdx)
                Debug.LogWarning("opened and closed are on the same step : Panel will never move.");
        }

        public void Open()
            => PlayAnimation(true);

        public void Close()
            => PlayAnimation(false);

        public WaitForTween PlayAnimation(bool pOpen)
        {
            return PathToStep(pOpen ? _openedIdx : _closedIdx);
        }
    }
}
