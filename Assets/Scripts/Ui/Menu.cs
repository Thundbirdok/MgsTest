using UnityEngine;

namespace Ui
{
    using DG.Tweening;
    using NaughtyAttributes;
    using UnityEngine.UI;

    public class Menu : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup background;

        [SerializeField]
        private PopupMover settingsMover;

        [SerializeField]
        private PopupMover backMover;
        
        [SerializeField]
        private float animationTime = 0.25f;
        
        [SerializeField]
        private Button back;

        private bool _isInitialized;
        
        private bool _isOpen;

        private Sequence _sequence;
        
        private void OnDestroy()
        {
            settingsMover.Dispose();
        }

        private void OnEnable()
        {
            back.onClick.AddListener(Close);
        }

        private void OnDisable()
        {
            back.onClick.RemoveListener(Close);
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

            settingsMover.Initialize();
            backMover.Initialize();

            SetSequence();
            
            _isInitialized = true;
        }

        private void OnSequenceComplete()
        {
            if (_isOpen == false)
            {
                gameObject.SetActive(false);
            }
        }

        private void SetSequence()
        {
            _sequence = DOTween.Sequence();
            
            _sequence
                .Append(background.DOFade(1, animationTime).From(0))
                .Append(settingsMover.Tween)
                .Join(backMover.Tween)
                .OnPause(OnSequenceComplete);
            
            _sequence.SetAutoKill(false);
            _sequence.Pause();
        }
    }
}
