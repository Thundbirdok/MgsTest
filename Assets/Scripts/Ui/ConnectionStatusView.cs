using UnityEngine;

namespace Ui
{
    using System;
    using DG.Tweening;
    using DG.Tweening.Core;
    using DG.Tweening.Plugins.Options;
    using ServerInteractions;
    using TMPro;
    using UnityEngine.UI;

    [Serializable]
    public class ConnectionStatusView
    {
        [SerializeField]
        private Toggle connection;

        [SerializeField]
        private TextMeshProUGUI connectionText;

        [SerializeField]
        private float tweenDuration = 0.25f;
        
        private ServerInteractionController _serverInteractionController;

        private bool _isInitialized;
        private bool _isTriedEnable;
        
        private TweenerCore<string, string, StringOptions> _tween;

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
            
            _serverInteractionController.OnConnectionStatusChanged += ShowConnectionStatus;
            
            ShowConnectionStatus();
        }

        public void Disable()
        {
            _isTriedEnable = false;

            if (_isInitialized == false)
            {
                return;
            }
            
            _serverInteractionController.OnConnectionStatusChanged -= ShowConnectionStatus;
        }
        
        private void ShowConnectionStatus()
        {
            var connectionStatus = _serverInteractionController.ConnectionStatus;
            
            connection.isOn = connectionStatus == ConnectionStatus.Connected;

            var text = connectionStatus switch
            {
                ConnectionStatus.Connected     => "Connected",
                ConnectionStatus.Disconnected  => "Disconnected",
                ConnectionStatus.Connecting    => "Connecting",
                ConnectionStatus.Disconnecting => "Closing",
                ConnectionStatus.Reconnecting  
                    => "Reconnecting (" + _serverInteractionController.ReconnectionIteration + ")",
                _ => "..."
            };
            
            _tween?.Kill();
            
            _tween = DOTween.To
            (
                () => connectionText.text, 
                x => connectionText.text = x,
                text, 
                tweenDuration
            );
        }
    }
}
