using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace GamblersMod.Patches
{
    [HarmonyPatch(typeof(RoundManager))]
    internal class RoundManagerPatch
    {
        [HarmonyPatch("FinishGeneratingNewLevelClientRpc")]
        [HarmonyPrefix]
        public static void FinishGeneratingNewLevelClientRpcPatch() {
            var mls = BepInEx.Logging.Logger.CreateLogSource("StartOfRoundPatch");
            mls.LogInfo("StartOfRoundPatch openingDoorsSequence StartOfRoundPatch StartOfRoundPatch");

            GameObject instantiatedGamblingMachine = UnityEngine.Object.Instantiate(GamblersModBase.GamblingMachine, new Vector3(0, 0, 0), Quaternion.identity);
            instantiatedGamblingMachine.layer = LayerMask.NameToLayer("InteractableObject");
            // instantiatedGamblingMachine.tag = "InteractTrigger";
            // instantiatedGamblingMachine.transform.tag = "InteractTrigger";
            // instantiatedGamblingMachine.getCompoenent<C
            Debug.Log($"Instantiated Tag: {instantiatedGamblingMachine.tag}");
            // GamblersModBase.GamblingMachine.transform.position = ;
        }

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void StartPatch()
        {
            var mls = BepInEx.Logging.Logger.CreateLogSource("StartOfRoundPatch");
            mls.LogInfo("StartPatch StartPatch StartPatch StartOfRoundPatch");
        }
    }
}
