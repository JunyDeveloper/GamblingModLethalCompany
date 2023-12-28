using HarmonyLib;
using UnityEngine;

namespace GamblersMod.Patches
{
    [HarmonyPatch(typeof(RoundManager))]
    internal class RoundManagerPatch
    {
        [HarmonyPatch("FinishGeneratingNewLevelClientRpc")]
        [HarmonyPrefix]
        public static void FinishGeneratingNewLevelClientRpcPatch(RoundManager __instance)
        {
            // Only spawn this at "The Company" moon
            if (__instance.currentLevel.levelID == 3)
            {
                Plugin.mls.LogInfo($"Attemping to spawn gambling machine at {__instance.currentLevel.name}");
                Vector3 gamblingMachineSpawnPoint = new Vector3(-27.808f, -2.6256f, -9.7409f);

                GameObject instantiatedGamblingMachine = UnityEngine.Object.Instantiate(Plugin.GamblingMachine, gamblingMachineSpawnPoint, Quaternion.Euler(0, 90, 0));
                instantiatedGamblingMachine.tag = "Untagged";
                instantiatedGamblingMachine.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                instantiatedGamblingMachine.layer = LayerMask.NameToLayer("InteractableObject");
                instantiatedGamblingMachine.AddComponent<AudioSource>();

                AudioSource gamblingMachineAudioSource = instantiatedGamblingMachine.GetComponent<AudioSource>();
                gamblingMachineAudioSource.loop = true;
                gamblingMachineAudioSource.clip = Plugin.GamblingMachineMusicAudio;
                gamblingMachineAudioSource.volume = 0.4f;
                gamblingMachineAudioSource.spatialBlend = 1f;

                // instantiatedGamblingMachine.GetComponent<AudioSource>().dopplerLevel = 0f;
                // instantiatedGamblingMachine.GetComponent<AudioSource>().rolloffMode = AudioRolloffMode.Linear;
                gamblingMachineAudioSource.Play();
            }
        }
    }
}
