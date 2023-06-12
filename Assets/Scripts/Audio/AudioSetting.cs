namespace Audio
{
    using System;
    using UnityEngine;

    [CreateAssetMenu(fileName = "AudioSetting", menuName = "Settings/AudioSetting")]
    public class AudioSetting : ScriptableObject
    {
        public event Action OnInitialized;
        public event Action OnVolumeChanged;
        public event Action OnIsOnChanged;

        [NonSerialized]
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

        [NonSerialized]
        private bool _isMuted;

        public bool IsMuted
        {
            get
            {
                return _isMuted;
            }

            set
            {
                _isMuted = value;
                
                OnIsOnChanged?.Invoke();
                OnVolumeChanged?.Invoke();
            }
        }

        [NonSerialized]
        private bool _isOn;
        public bool IsOn
        {
            get
            {
                return !IsMuted && _isOn;
            }

            set
            {
                if (_isOn == value)
                {
                    return;
                }

                _isOn = value;

                if (IsOn)
                {
                    Volume = _value > 0 ? _value : defaultValue;
                }
                else
                {
                    Volume = 0;
                }
                
                OnIsOnChanged?.Invoke();
            }
        }

        private float _value;
        
        [NonSerialized]
        private float _volume;
        public float Volume
        {
            get
            {
                return IsOn ? _volume : 0;
            }

            set
            {
                _volume = value;

                if (IsOn)
                {
                    _value = value;
                }

                OnVolumeChanged?.Invoke();
            }
        }

        [field: SerializeField]
        public string Key { get; private set; }

        [SerializeField]
        private float defaultValue = 0.5f;

        [SerializeField]
        private bool defaultIsOn = true;

        public void Setup(AudioSettingJson save)
        {
            _value = save.value;
            Volume = save.value;
            IsOn = save.isOn;

            IsInitialized = true;
        }
        
        public void SetupDefault()
        {
            _value = defaultValue;
            Volume = defaultValue;
            IsOn = defaultIsOn;

            IsInitialized = true;
        }

        public AudioSettingJson GetJson()
        {
            return new AudioSettingJson() 
            {
                isOn = _isOn,
                value = _value,
                key = Key
            };
        }
    }
    
    [Serializable]
    public class AudioSettingJson
    {
        public bool isOn;
        public float value;
        public string key;
    }
}
