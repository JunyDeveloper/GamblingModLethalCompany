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

            GameObject instantiatedGamblingMachine = UnityEngine.Object.Instantiate(Plugin.GamblingMachine, new Vector3(-27.808f, -2.6256f, -9.7409f), Quaternion.Euler(0, 90, 0));
            instantiatedGamblingMachine.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            instantiatedGamblingMachine.layer = LayerMask.NameToLayer("InteractableObject");
        }
    }
}
