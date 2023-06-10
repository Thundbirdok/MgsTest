using UnityEngine;

namespace Ui
{
    using DG.Tweening;
    using NaughtyAttributes;
    using UnityEngine.UI;

    public class NoConnectionPopup : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup background;

        [SerializeField]
        private PopupMover popupMover;

        [SerializeField]
        private float animationTime = 0.25f;
        
        [SerializeField]
        private Button ok;

        private bool _isInitialized;
        
        private bool _isOpen;

        private Sequence _sequence;

        private void OnDestroy()
        {
            _sequence.Kill();
        }

        private void OnEnable()
        {
            ok.onClick.AddListener(Close);
        }

        private void OnDisable()
        {
            ok.onClick.RemoveListener(Close);
        }

        [Button("Open")]
        public void Open()
        {
            if (_isOpen)
            {
                return;
            }

            Initialize();
            
            background.alpha = 0;

            gameObject.SetActive(true);
            
            _isOpen = true;
            
            _sequence.PlayForward();
        }

        [Button("Close")]
        public void Close()
        {
            if (_isOpen == false)
            {
                return;
            }

            Initialize();
            
            _isOpen = false;

            _sequence.PlayBackwards();
        }

        private void Initialize()
        {
            if (_isInitialized)
            {
                return;   
            }
            
            popupMover.Initialize();
            
            SetOpenSequence();

            _isInitialized = true;
        }

        private void OnCloseSequenceComplete()
        {
            if (_isOpen == false)
            {
                gameObject.SetActive(false);
            }
        }

        private void SetOpenSequence()
        {
            _sequence = DOTween.Sequence();
            
            _sequence
                .Append(background.DOFade(1, animationTime).From(0))
                .Append(popupMover.Tween)
                .OnPause(OnCloseSequenceComplete);
            
            _sequence.SetAutoKill(false);
            _sequence.Pause();
        }
    }
}
