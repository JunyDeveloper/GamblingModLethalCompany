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

            GamblingMachine = UnityEngine.Object.Instantiate(Plugin.GamblingMachine, gamblingMachineSpawnPoint, Quaternion.Euler(0, 90, 0));
            GamblingMachine.tag = "Untagged";
            GamblingMachine.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            GamblingMachine.layer = LayerMask.NameToLayer("InteractableObject");
            GamblingMachine.AddComponent<AudioSource>();

            AudioSource gamblingMachineAudioSource = GamblingMachine.GetComponent<AudioSource>();
            gamblingMachineAudioSource.loop = true;
            gamblingMachineAudioSource.clip = Plugin.GamblingMachineMusicAudio;
            gamblingMachineAudioSource.volume = 0.4f;
            gamblingMachineAudioSource.spatialBlend = 1f;

            GamblingMachine.GetComponent<NetworkObject>().Spawn();

            gamblingMachineAudioSource.Play();

        }
    }
}
