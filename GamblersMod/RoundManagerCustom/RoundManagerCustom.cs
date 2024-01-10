using Unity.Netcode;
using UnityEngine;

namespace GamblersMod.RoundManagerCustomSpace
{
    internal class RoundManagerCustom : NetworkBehaviour
    {
        public RoundManager RoundManager;
        public GameObject GamblingMachine;

        void Awake()
        {
            RoundManager = GetComponent<RoundManager>();
        }

        [ServerRpc]
        public void DespawnGamblingMachineServerRpc()
        {
            GamblingMachine.GetComponent<NetworkObject>().Despawn();
        }

        [ServerRpc]
        public void SpawnGamblingMachineServerRpc()
        {
            Plugin.mls.LogInfo($"Attempting to spawn gambling machine at {RoundManager.currentLevel.name}");
            Vector3 gamblingMachineSpawnPoint = new Vector3(-27.808f, -2.6256f, -9.7409f);
            Plugin.GamblingMachineManager.GetComponent<GamblingMachineManager.GamblingMachineManager>().Spawn(gamblingMachineSpawnPoint, Quaternion.Euler(0, 90, 0));
        }
    }
}
