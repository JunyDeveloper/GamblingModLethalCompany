using HarmonyLib;
using UnityEngine;

namespace GamblersMod.Patches
{
    [HarmonyPatch(typeof(RoundManager))]
    internal class RoundManagerPatch
    {
        [HarmonyPatch("FinishGeneratingNewLevelClientRpc")]
        [HarmonyPrefix]
        public static void FinishGeneratingNewLevelClientRpcPatch()
        {
            var mls = BepInEx.Logging.Logger.CreateLogSource("StartOfRoundPatch");

            Vector3 gamblingMachineSpawnPoint = new Vector3(-27.808f, -2.6256f, -9.7409f);
            GameObject instantiatedGamblingMachine = UnityEngine.Object.Instantiate(Plugin.GamblingMachine, gamblingMachineSpawnPoint, Quaternion.Euler(0, 90, 0));
            instantiatedGamblingMachine.tag = "Untagged";
            // GetComponent is inefficient doing it over agin
            instantiatedGamblingMachine.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            instantiatedGamblingMachine.layer = LayerMask.NameToLayer("InteractableObject");
            instantiatedGamblingMachine.AddComponent<AudioSource>();
            instantiatedGamblingMachine.GetComponent<AudioSource>().loop = true;
            instantiatedGamblingMachine.GetComponent<AudioSource>().clip = Plugin.GamblingMachineMusicAudio;
            instantiatedGamblingMachine.GetComponent<AudioSource>().volume = 0.4f;
            instantiatedGamblingMachine.GetComponent<AudioSource>().spatialBlend = 1f;
            // instantiatedGamblingMachine.GetComponent<AudioSource>().dopplerLevel = 0f;
            // instantiatedGamblingMachine.GetComponent<AudioSource>().rolloffMode = AudioRolloffMode.Linear;
            instantiatedGamblingMachine.GetComponent<AudioSource>().Play();

            // Plugin.GamblingMachineResultCanvas
        }
    }
}
