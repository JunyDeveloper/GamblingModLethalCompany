using Unity.Netcode;

namespace GamblersMod
{
    internal class NetworkHandler : NetworkBehaviour
    {
        public static NetworkHandler Instance { get; private set; }
    }
}
