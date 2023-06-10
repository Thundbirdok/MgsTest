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

        private bool _isConnected;
        public bool IsConnected
        {
            get
            {
                return _isConnected;
            }

            private set
            {
                if (_isConnected == value)
                {
                    return;
                }

                _isConnected = value;
                
                OnConnectionStatusChanged?.Invoke();
            }
        }

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
        private string uri = "ws://185.246.65.199:9090/ws";

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

        private WebSocket _websocket;

        private bool _isInternetReachable;
        private bool _isConnecting;
        private bool _isClosing;
        private bool _isNoConnectionFired;

        private int _reconnectionIteration;

        private float _timer;

        private async void Start() => await Initialize();

        private async void OnDestroy() => await Dispose();

        private void Update()
        {
            if (_isInternetReachable == false)
            {
                if (Application.internetReachability != NetworkReachability.NotReachable)
                {
                    _isInternetReachable = true;

                    _ = Connect();
                }
            }
            
            _timer += Time.deltaTime;

            if (_timer < messageDispatchDelay)
            {
                return;
            }
            
            _timer = 0;
            
            if (_websocket.State == WebSocketState.Open)
            {
                _websocket.DispatchMessageQueue();
            }
        }
        
        private async Task Initialize()
        {
            _websocket = new WebSocket(uri);

            _websocket.OnOpen += OnWebsocketOpen;
            _websocket.OnClose += OnWebsocketClose;
            _websocket.OnMessage += OnWebsocketMessage;
            _websocket.OnError += OnWebsocketError;

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                _isInternetReachable = false;
                
                return;
            }
            
            await Connect();
        }

        private async Task Connect()
        {
            if (IsConnected || _isConnecting)
            {
                return;
            }
            
            _isConnecting = true;

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
            if (_websocket is not { State: WebSocketState.Open })
            {
                return;
            }

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

                    if (RandomStatus)
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

        private void OnWebsocketClose(WebSocketCloseCode code)
        {
            if (isLogToConsole)
            {
                Debug.Log("WebSocket closed with code: " + code);
            }
            
            IsConnected = false;
            _isConnecting = false;

            if (_isClosing)
            {
                return;
            }
            
            HandleConnectionLoss();
        }

        private static void OnWebsocketError(string errorMsg)
        {
            Debug.LogError("WebSocket error: " + errorMsg);
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

        private void OnWebsocketOpen()
        {
            _reconnectionIteration = 0;
            _isNoConnectionFired = false;

            IsConnected = true;
            
            if (isLogToConsole)
            {
                Debug.Log("WebSocket status is: " + _websocket.State);
            }
            
            SendRequest(REQUEST_GET_CURRENT_ODOMETER_OPERATION_NAME);
            SendRequest(REQUEST_GET_RANDOM_STATUS_OPERATION_NAME);
        }
        
        private async void HandleConnectionLoss()
        {
            if (_isNoConnectionFired == false)
            {
                _isNoConnectionFired = true;
                
                OnConnectionStatusChanged?.Invoke();
            }

            if (_reconnectionIteration > maxReconnectionIterations)
            {
                if (isLogToConsole)
                {
                    Debug.Log("Connection failed. No more reconnections");
                }
                
                return;
            }
            
            var delay = (int)Mathf.Pow(2, _reconnectionIteration) * 1000;
            
            if (isLogToConsole)
            {
                Debug.Log("Reconnection after " + delay + " milliseconds");
            }
            
            await Task.Delay(delay);
            
            ++_reconnectionIteration;

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
