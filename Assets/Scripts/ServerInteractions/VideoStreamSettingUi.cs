using UnityEngine;

namespace ServerInteractions
{
    using TMPro;

    public class VideoStreamSettingUi : MonoBehaviour
    {
        [SerializeField]
        private VideoStreamAddressSetting setting;

        [SerializeField]
        private TMP_InputField field;
        
        private void OnEnable()
        {
            field.onEndEdit.AddListener(OnEndEdit);
            
            if (setting.IsInitialized)
            {
                UpdateField();
                
                return;
            }
            
            setting.OnInitialized += UpdateField;
        }

        private void OnDisable()
        {
            field.onEndEdit.RemoveListener(OnEndEdit);
        }

        private void UpdateField()
        {
            setting.OnInitialized -= UpdateField;
            
            field.text = setting.Address;
        }

        private void OnEndEdit(string text)
        {
            setting.Address = text;
        }
    }
}
