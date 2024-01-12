using Unity.Netcode;

namespace GamblersMod
{
    public class StartOfRoundCustom : NetworkBehaviour
    {
        void Awake()
        {

        }


        [ServerRpc]
        public void DespawnGamblingMachineServerRpc()
        {
            GamblingMachineManager.Instance.DespawnAll();
        }
    }
}
