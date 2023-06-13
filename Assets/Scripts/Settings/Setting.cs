namespace Settings
{
    using System;
    using UnityEngine;

    public abstract class Setting<T> : ScriptableObject where T : SettingJson
    {
        [field: SerializeField]
        public string Key { get; private set; }
        public abstract T GetJson();
        public abstract void Setup(T json);
        public abstract void SetupDefault();
    }

    [Serializable]
    public abstract class SettingJson
    {
        public string key;
    }
}