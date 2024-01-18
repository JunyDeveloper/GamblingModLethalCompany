using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace GamblersMod
{
    public class GamblingMachineManager : MonoBehaviour
    {
        public List<GameObject> GamblingMachines;

        public static GamblingMachineManager Instance { get; private set; }
        void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
            Plugin.mls.LogMessage($"Gambling machine manager has awoken!");
            GamblingMachines = new List<GameObject>();
            DontDestroyOnLoad(gameObject);
        }

        public void Spawn(Vector3 spawnPoint, Quaternion quaternion)
        {
            Plugin.mls.LogMessage($"Spawning gambling machine... #{GamblingMachines.Count}");
            GameObject GamblingMachine = UnityEngine.Object.Instantiate(Plugin.GamblingMachine, spawnPoint, quaternion);
            GamblingMachine.tag = "Untagged";
            GamblingMachine.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            GamblingMachine.layer = LayerMask.NameToLayer("InteractableObject");
            GamblingMachine.GetComponent<NetworkObject>().Spawn();

            // Only the first gambling machine will play music
            if (GamblingMachines.Count >= 1)
            {
                GamblingMachine.GetComponent<AudioSource>().Pause();
            }

            GamblingMachines.Add(GamblingMachine);
        }

        public void DespawnAll()
        {
            Plugin.mls.LogMessage($"Despwawning gambling machine...");
            foreach (GameObject GamblingMachine in GamblingMachines)
            {
                GamblingMachine.GetComponent<NetworkObject>().Despawn();
            }
            Reset();
        }

        public void Reset()
        {
            Plugin.mls.LogInfo("Resetting gambling machine manager state...");
            GamblingMachines.Clear();
        }
    }
}
