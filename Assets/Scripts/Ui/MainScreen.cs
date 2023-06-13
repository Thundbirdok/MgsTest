using UnityEngine;

namespace Ui
{
    using ServerInteractions;
    using UnityEngine.UI;

    public class MainScreen : MonoBehaviour
    {
        [SerializeField]
        private ServerInteractionController serverInteractionController;

        [SerializeField]
        private ConnectionStatusView connectionStatusView;
        
        [SerializeField]
        private Toggle randomStatus;

        [SerializeField]
        private OdometerView odometer;

        [SerializeField]
        private Button menuButton;

        [SerializeField]
        private Menu menu;

        private void Awake()
        {
            odometer.Construct(serverInteractionController);
            connectionStatusView.Construct(serverInteractionController);
        }

        private void OnEnable()
        {
            randomStatus.isOn = serverInteractionController.RandomStatus;

            odometer.Enable();
            connectionStatusView.Enable();
            
            serverInteractionController.OnRandomStatusChanged += RandomStatusChanged;

            menuButton.onClick.AddListener(menu.Open);
            menu.Close();
        }

        private void OnDisable()
        {
            odometer.Disable();
            connectionStatusView.Disable();
            
            serverInteractionController.OnRandomStatusChanged -= RandomStatusChanged;

            menuButton.onClick.RemoveListener(menu.Open);
        }

        private void RandomStatusChanged() => randomStatus.isOn = serverInteractionController.RandomStatus;
    }
}
