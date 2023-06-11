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
        private bool _isOn;
        public bool IsOn
        {
            get
            {
                return _isOn;
            }

            set
            {
                if (_isOn == value)
                {
                    return;
                }

                _isOn = value;

                if (_isOn)
                {
                    Volume = Value > 0 ? Value : defaultValue;
                }
                else
                {
                    Volume = 0;
                }
                
                OnIsOnChanged?.Invoke();
            }
        }
        
        public float Value { get; private set; }
        
        [NonSerialized]
        private float _volume;
        public float Volume
        {
            get
            {
                return _volume;
            }

            set
            {
                _volume = value;

                if (IsOn)
                {
                    Value = value;
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
            Value = save.value;
            Volume = save.value;
            IsOn = save.isOn;

            IsInitialized = true;
        }
        
        public void SetupDefault()
        {
            Value = defaultValue;
            Volume = defaultValue;
            IsOn = defaultIsOn;

            IsInitialized = true;
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
