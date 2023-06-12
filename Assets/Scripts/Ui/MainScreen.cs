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
        private Toggle connection;
        
        [SerializeField]
        private Toggle randomStatus;

        [SerializeField]
        private OdometerView odometer;

        [SerializeField]
        private NoConnectionPopup noConnectionPopup;

        [SerializeField]
        private Button menuButton;

        [SerializeField]
        private Menu menu;

        private void Awake()
        {
            odometer.Construct(serverInteractionController);
        }

        private void OnEnable()
        {
            connection.isOn = serverInteractionController.IsConnected;
            randomStatus.isOn = serverInteractionController.RandomStatus;

            odometer.Enable();
            
            serverInteractionController.OnConnectionStatusChanged += ConnectionStatusChanged;
            serverInteractionController.OnRandomStatusChanged += RandomStatusChanged;

            menuButton.onClick.AddListener(menu.Open);
            menu.Close();
        }

        private void OnDisable()
        {
            odometer.Disable();
            
            serverInteractionController.OnConnectionStatusChanged -= ConnectionStatusChanged;
            serverInteractionController.OnRandomStatusChanged -= RandomStatusChanged;

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

        private void RandomStatusChanged() => randomStatus.isOn = serverInteractionController.RandomStatus;
    }
}
