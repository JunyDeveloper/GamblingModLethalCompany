using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace GamblersMod.GamblingMachineManager
{
    public class GamblingMachineManager : MonoBehaviour
    {
        public static List<GameObject> GamblingMachines;

        public static GamblingMachineManager instance;
        void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);
        }

        public static void Spawn(Vector3 spawnPoint, Quaternion quaternion)
        {
            Plugin.mls.LogMessage($"Spawning gambling machine... #{GamblingMachines.Count}");
            GameObject GamblingMachine = UnityEngine.Object.Instantiate(Plugin.GamblingMachine, spawnPoint, quaternion);
            GamblingMachine.tag = "Untagged";
            GamblingMachine.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            GamblingMachine.layer = LayerMask.NameToLayer("InteractableObject");
            GamblingMachine.GetComponent<NetworkObject>().Spawn();
            GamblingMachines.Add(GamblingMachine);
        }

        public static void DespawnAll()
        {
            Plugin.mls.LogMessage($"Despwawning gambling machine...");
            foreach (GameObject GamblingMachine in GamblingMachines)
            {
                GamblingMachine.GetComponent<NetworkObject>().Despawn();
                GamblingMachines.RemoveAt(0);
            }
        }
    }
}
