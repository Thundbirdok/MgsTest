namespace ServerInteractions
{
    using System;
    using System.Threading.Tasks;
    using NativeWebSocket;
    using UnityEngine;

    public class ServerInteractionController : MonoBehaviour
    {
        public event Action OnConnectionStatusChanged;
        public event Action OnOdometerValueChanged;
        public event Action OnRandomStatusChanged;

        private ConnectionStatus _status = ConnectionStatus.Disconnected;
        public ConnectionStatus ConnectionStatus
        {
            get
            {
                return _websocket == null ? ConnectionStatus.Disconnected : _status;
            }

            private set
            {
                if (_status == value && _status != ConnectionStatus.Reconnecting)
                {
                    return;
                }
                
                _status = value;
                
                OnConnectionStatusChanged?.Invoke();
            }
        }

        public int ReconnectionIteration { get; private set; }

        [NonSerialized]
        private float _odometerValue;

        public float OdometerValue
        {
            get
            {
                return _odometerValue;
            }

            private set
            {
                _odometerValue = value;
                
                OnOdometerValueChanged?.Invoke();
            }
        }

        [NonSerialized]
        private bool _randomStatus;

        public bool RandomStatus
        {
            get
            {
                return _randomStatus;
            }

            private set
            {
                if (_randomStatus == value)
                {
                    return;
                }

                _randomStatus = value;
                
                OnRandomStatusChanged?.Invoke();
            }
        }

        [SerializeField]
        private ServerAddressSetting serverAddress;

        [SerializeField]
        private int maxReconnectionIterations = 4;

        [SerializeField]
        private float messageDispatchDelay = 1;

        [SerializeField]
        private bool isLogToConsole = true;

        private const string REQUEST_GET_CURRENT_ODOMETER_OPERATION_NAME = "getCurrentOdometer";
        private const string RESPONSE_GET_CURRENT_ODOMETER_OPERATION_NAME = "currentOdometer";

        private const string REQUEST_GET_RANDOM_STATUS_OPERATION_NAME = "getRandomStatus";
        private const string RESPONSE_GET_RANDOM_STATUS_OPERATION_NAME = "randomStatus";

        private const string BROADCAST_ODOMETER_OPERATION_NAME = "odometer_val";
        
        [NonSerialized]
        private WebSocket _websocket;

        [NonSerialized]
        private float _timer;

        private void Start()
        {
            serverAddress.OnUrlUpdate += Initialize;
            
            Initialize();
        }

        private async void OnDestroy()
        {
            serverAddress.OnUrlUpdate -= Initialize;
            
            await Dispose();
        }

        private void Update()
        {
            _timer += Time.deltaTime;

            if (_timer < messageDispatchDelay)
            {
                return;
            }
            
            _timer = 0;

            _websocket?.DispatchMessageQueue();
        }

        private void Initialize()
        {
            _ = InitializeAndWaitForCompletion();
        }
        
        private async Task InitializeAndWaitForCompletion()
        {
            if (_websocket != null)
            {
                _websocket.DispatchMessageQueue();
                
                await Dispose();
            }
        
            ReconnectionIteration = 0;
            ConnectionStatus = ConnectionStatus.Disconnected;
            
            _websocket = new WebSocket(serverAddress.Url);

            _websocket.OnOpen += OnWebsocketOpen;
            _websocket.OnClose += OnWebsocketClose;
            _websocket.OnMessage += OnWebsocketMessage;
            _websocket.OnError += OnWebsocketError;

            await Connect();
        }

        private async Task Connect()
        {
            if (ConnectionStatus is ConnectionStatus.Connected or ConnectionStatus.Connecting)
            {
                return;
            }

            ConnectionStatus = ConnectionStatus != ConnectionStatus.Reconnecting 
                ? ConnectionStatus.Connecting 
                : ConnectionStatus.Reconnecting;
            
            await _websocket.Connect();
        }

        private async Task Dispose()
        {
            await Disconnect();

            _websocket.OnOpen -= OnWebsocketOpen;
            _websocket.OnClose -= OnWebsocketClose;
            _websocket.OnMessage -= OnWebsocketMessage;
            _websocket.OnError -= OnWebsocketError;

            _websocket = null;
        }

        private async Task Disconnect()
        {
            if (ConnectionStatus is ConnectionStatus.Disconnected or ConnectionStatus.Disconnected)
            {
                return;
            }

            ConnectionStatus = ConnectionStatus.Disconnecting;
            
            await _websocket.Close();
        }

        private void ProcessMessage(string message)
        {
            var response = JsonUtility.FromJson<ResponseMessage>(message);

            switch (response.operation)
            {
                case BROADCAST_ODOMETER_OPERATION_NAME:
                    
                    OdometerValue = response.value;

                    break;
                
                case RESPONSE_GET_CURRENT_ODOMETER_OPERATION_NAME:
                    
                    OdometerValue = response.odometer;

                    break;
                
                case RESPONSE_GET_RANDOM_STATUS_OPERATION_NAME:
                    
                    RandomStatus = response.status;

                    if (response.odometer != 0)
                    {
                        OdometerValue = response.odometer;
                    }
                    
                    break;
                
                default:
                    
                    Debug.LogError("Unknown operation: " + response.operation);
                    
                    break;
            }
        }

        private async void SendRequest(string operation)
        {
            var request = new RequestMessage
            {
                operation = operation
            };
            
            var jsonRequest = JsonUtility.ToJson(request);
            
            await _websocket.SendText(jsonRequest);

            if (isLogToConsole)
            {
                Debug.Log("Send request: " + jsonRequest);
            }
        }

        private void OnWebsocketMessage(byte[] msg)
        {
            var message = System.Text.Encoding.UTF8.GetString(msg);

            if (isLogToConsole)
            {
                Debug.Log("Get message: " + message);
            }
            
            ProcessMessage(message);
        }

        private void OnWebsocketClose(WebSocketCloseCode code)
        {
            if (isLogToConsole)
            {
                Debug.Log("WebSocket closed with code: " + code);
            }

            if (ConnectionStatus == ConnectionStatus.Disconnecting)
            {
                ConnectionStatus = ConnectionStatus.Disconnected;
                
                return;
            }
            
            HandleConnectionLoss();
        }

        private void OnWebsocketError(string errorMsg)
        {
            Debug.LogError("WebSocket error: " + errorMsg);
        }

        private void OnWebsocketOpen()
        {
            ReconnectionIteration = 0;

            ConnectionStatus = ConnectionStatus.Connected;
            
            if (isLogToConsole)
            {
                Debug.Log("WebSocket status is: " + _websocket.State);
            }
            
            SendRequest(REQUEST_GET_CURRENT_ODOMETER_OPERATION_NAME);
            SendRequest(REQUEST_GET_RANDOM_STATUS_OPERATION_NAME);
        }

        private async void HandleConnectionLoss()
        {
            ConnectionStatus = ConnectionStatus.Reconnecting;

            var iteration = Mathf.Clamp(ReconnectionIteration, 0, maxReconnectionIterations);
            var delay = (int)Mathf.Pow(2, iteration) * 1000;
            
            if (isLogToConsole)
            {
                Debug.Log("Reconnection after " + delay + " milliseconds");
            }
            
            await Task.Delay(delay);
            
            ++ReconnectionIteration;
            
            await Connect();
        }

        [Serializable]
        public class RequestMessage
        {
            public string operation;
        }
        
        [Serializable]
        public class ResponseMessage
        {
            public string operation;
            public float value;
            public float odometer;
            public bool status;
        }
    }
}
