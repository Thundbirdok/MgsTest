using UnityEngine;

namespace ServerInteractions
{
    public enum ConnectionStatus
    {
        Disconnected,
        Disconnecting,
        Connected,
        Connecting,
        Reconnecting
    }
}
