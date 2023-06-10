using UnityEngine;

namespace Ui
{
    using System;
    using DG.Tweening;

    [Serializable]
    public class PopupMover
    {
        public Tween Tween;
        
        [SerializeField]
        private RectTransform boxRectTransform;

        [SerializeField]
        private RectTransform popup;
        
        [SerializeField]
        private RectTransform popupTargetPosition;

        [SerializeField]
        private float animationTime = 0.25f;

        [SerializeField]
        private bool fromAbove;
        
        private Vector2 _popupStartPosition;

        public void Initialize()
        {
            SetPopupStartPosition();
            SetTween();
        }

        public void Dispose()
        {
            Tween.Kill();
        }

        private void SetTween()
        {
            Tween = popup.DOAnchorPos
            (
                popupTargetPosition.anchoredPosition,
                animationTime / 2
            )
            .From(_popupStartPosition);

            Tween.SetAutoKill(false);
        }
        
        private void SetPopupStartPosition()
        {
            var y = boxRectTransform.rect.height / 2 + popup.rect.height / 2;

            _popupStartPosition = new Vector2(0, fromAbove ? y : -y);
        }
    }
}
