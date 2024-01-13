using HarmonyLib;

namespace GamblersMod.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    internal class StartOfRoundPatch
    {
        //public static StartOfRoundCustom StartOfRoundCustom;

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        static void AwakePatch(StartOfRound __instance)
        {
            Plugin.mls.LogInfo("StartOfRoundPatch has awoken");
            //StartOfRoundCustom = __instance.gameObject.AddComponent<StartOfRoundCustom>();
        }
    }
}
