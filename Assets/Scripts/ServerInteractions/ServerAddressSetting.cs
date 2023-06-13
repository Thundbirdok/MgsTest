using UnityEngine;

namespace ServerInteractions
{
    using System;
    using Settings;

    [CreateAssetMenu(fileName = "ServerAddressSetting", menuName = "Settings/ServerAddressSetting")]
    public class ServerAddressSetting : Setting<ServerAddressSettingJson>
    {
        public event Action OnInitialized;
        public event Action OnUrlUpdate;
        
        private bool _isInitialized;
        public bool IsInitialized
        {
            get
            {
                return _isInitialized;
            }

            private set
            {
                _isInitialized = value;

                if (_isInitialized)
                {
                    OnInitialized?.Invoke();
                }
            }
        }
        
        public string Url { get; private set; }

        private string _address;

        public string Address
        {
            get
            {
                return _address;
            }

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    return;
                }

                if (_address == value)
                {
                    return;   
                }
                
                _address = value;

                Url = GetUrl();
                
                OnUrlUpdate?.Invoke();
            }
        }

        private string _port;

        public string Port
        {
            get
            {
                return _port;
            }

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    return;
                }

                if (_port == value)
                {
                    return;
                }
                
                _port = value;
                
                Url = GetUrl();
                
                OnUrlUpdate?.Invoke();
            }
        }

        [SerializeField]
        private string format = "ws://{0}:{1}/ws";
        
        [SerializeField]
        private string defaultServerAddress = "185.246.65.199";
        
        [SerializeField]
        private string defaultServerPort = "9090";

        public override void Setup(ServerAddressSettingJson address)
        {
            _address = address.address;
            _port = address.port;
            
            Url = GetUrl();

            IsInitialized = true;
            OnUrlUpdate?.Invoke();
        }

        public override void SetupDefault()
        {
            _address = defaultServerAddress;
            _port = defaultServerPort;
            
            Url = GetUrl();
            
            IsInitialized = true;
            OnUrlUpdate?.Invoke();
        }

        public override ServerAddressSettingJson GetJson()
        {
            return new ServerAddressSettingJson 
            {
                key = Key,
                address = _address,
                port = _port
            };
        }
        
        private string GetUrl() => string.Format(format, Address, Port);
    }

    [Serializable]
    public class ServerAddressSettingJson : SettingJson
    {
        public string address;
        public string port;
    }
}
