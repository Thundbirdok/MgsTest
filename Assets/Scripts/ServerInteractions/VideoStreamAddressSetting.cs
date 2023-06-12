using UnityEngine;

namespace ServerInteractions
{
    using System;

    [CreateAssetMenu(fileName = "VideoStreamAddressSetting", menuName = "Settings/VideoStreamAddressSetting")]
    public class VideoStreamAddressSetting : ScriptableObject
    {
        public event Action OnInitialized;
        public event Action OnAddressChanged;
        
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

        private string _address;
        public string Address
        {
            get
            {
                return _address;
            }

            set
            {
                _address = value;
                
                OnAddressChanged?.Invoke();
            }
        }

        [SerializeField]
        private string defaultAddress = "rtsp://192.168.100.2:8554/";

        public void Setup(VideoStreamAddressSettingJson address)
        {
            Address = address.address;
            IsInitialized = true;
        }

        public void SetupDefault()
        {
            Address = defaultAddress;
            IsInitialized = true;
        }
    }

    [Serializable]
    public class VideoStreamAddressSettingJson
    {
        public string address;
    }
}
