using UnityEngine;

namespace Ui
{
    using DG.Tweening;
    using System.Globalization;
    using DG.Tweening.Core;
    using DG.Tweening.Plugins.Options;
    using ServerInteractions;
    using TMPro;
    using UnityEngine.UI;

    public class MainScreen : MonoBehaviour
    {
        [SerializeField]
        private ServerInteractionController serverInteractionController;

        [SerializeField]
        private Toggle connection;
        
        [SerializeField]
        private Toggle randomStatus;

        [SerializeField]
        private TextMeshProUGUI odometer;

        [SerializeField]
        private NoConnectionPopup noConnectionPopup;

        [SerializeField]
        private Button menuButton;

        [SerializeField]
        private Menu menu;
        
        [SerializeField]
        private float odometerTextTweenDuration = 0.1f;

        private TweenerCore<float, float, FloatOptions> _odometerTween;
        
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
        
        private void OnEnable()
        {
            connection.isOn = serverInteractionController.IsConnected;
            randomStatus.isOn = serverInteractionController.RandomStatus;
            OdometerShowedValue = serverInteractionController.OdometerValue;
            
            serverInteractionController.OnConnectionStatusChanged += ConnectionStatusChanged;
            serverInteractionController.OnRandomStatusChanged += RandomStatusChanged;
            serverInteractionController.OnOdometerValueChanged += OdometerValueChanged;
            
            menuButton.onClick.AddListener(menu.Open);
            menu.Close();
        }

        private void OnDisable()
        {
            serverInteractionController.OnConnectionStatusChanged -= ConnectionStatusChanged;
            serverInteractionController.OnRandomStatusChanged -= RandomStatusChanged;
            serverInteractionController.OnOdometerValueChanged -= OdometerValueChanged;
            
            menuButton.onClick.RemoveListener(menu.Open);
        }

        private void ConnectionStatusChanged()
        {
            connection.isOn = serverInteractionController.IsConnected;

            if (serverInteractionController.IsConnected == false)
            {
                noConnectionPopup.Open();
            }
        }

        private void OdometerValueChanged()
        {
            _odometerTween?.Kill();
            
            _odometerTween = DOTween.To(() => OdometerShowedValue, x => OdometerShowedValue = x, serverInteractionController.OdometerValue, odometerTextTweenDuration);
        }

        private void RandomStatusChanged()
        {
            randomStatus.isOn = serverInteractionController.RandomStatus;
        }
    }
}
