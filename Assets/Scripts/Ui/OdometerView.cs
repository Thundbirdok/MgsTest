using UnityEngine;

namespace Ui
{
    using System;
    using System.Globalization;
    using DG.Tweening;
    using DG.Tweening.Core;
    using DG.Tweening.Plugins.Options;
    using ServerInteractions;
    using TMPro;

    [Serializable]
    public class OdometerView
    {
        private float _odometerShowedValue;
        private float OdometerShowedValue
        {
            get
            {
                return _odometerShowedValue;
            }

            set
            {
                _odometerShowedValue = value;
                odometer.text = _odometerShowedValue.ToString(CultureInfo.CurrentCulture);
            }
        }

        [SerializeField]
        private TextMeshProUGUI odometer;

        [SerializeField]
        private float odometerTextTweenDuration = 0.1f;

        private ServerInteractionController _serverInteractionController;
        
        private TweenerCore<float, float, FloatOptions> _odometerTween;

        private bool _isInitialized;
        private bool _isTriedEnable;
        
        public void Construct(ServerInteractionController serverInteractionController)
        {
            _serverInteractionController = serverInteractionController;

            _isInitialized = true;
            
            if (_isTriedEnable)
            {
                Enable();
            }
        }

        public void Enable()
        {
            if (_isInitialized == false)
            {
                _isTriedEnable = true;
                
                return;
            }
            
            OdometerShowedValue = 0;

            _serverInteractionController.OnOdometerValueChanged += SetOdometerValue;

            SetOdometerValue();
        }

        public void Disable()
        {
            _isTriedEnable = false;

            if (_isInitialized == false)
            {
                return;
            }
            
            _serverInteractionController.OnOdometerValueChanged -= SetOdometerValue;
        }

        private void SetOdometerValue()
        {
            _odometerTween?.Kill();
            
            _odometerTween = DOTween.To
            (
                () => OdometerShowedValue, 
                x => OdometerShowedValue = x,
                _serverInteractionController.OdometerValue, 
                odometerTextTweenDuration
            );
        }
    }
}
