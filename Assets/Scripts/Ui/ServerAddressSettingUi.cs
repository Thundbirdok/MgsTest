namespace Ui
{
    using ServerInteractions;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class ServerAddressSettingUi : MonoBehaviour
    {
        [SerializeField]
        private ServerAddressSetting setting;

        [SerializeField]
        private TMP_InputField addressField;
        
        [SerializeField]
        private TMP_InputField portField;

        [SerializeField]
        private Button apply;
        
        private void OnEnable()
        {
            apply.onClick.AddListener(Apply);

            if (setting.IsInitialized)
            {
                UpdateFields();
                
                return;
            }
            
            setting.OnInitialized += UpdateFields;
        }

        private void OnDisable() => apply.onClick.RemoveListener(Apply);

        private void UpdateFields()
        {
            setting.OnInitialized -= UpdateFields;
            
            addressField.text = setting.Address;
            portField.text = setting.Port;
        }

        private void Apply()
        {
            setting.Address = addressField.text;
            setting.Port = portField.text;
        }
    }
}
