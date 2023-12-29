using GamblersMod.RoundManagerCustomSpace;
using HarmonyLib;

namespace GamblersMod.Patches
{
    [HarmonyPatch(typeof(RoundManager))]
    internal class RoundManagerPatch
    {
        public static RoundManagerCustom RoundManagerCustom;

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void AwakePatch(RoundManager __instance)
        {
            Plugin.mls.LogInfo("RoundManagerPatch has awoken");
            RoundManagerCustom = __instance.gameObject.AddComponent<RoundManagerCustom>();
        }

        [HarmonyPatch("LoadNewLevelWait")]
        [HarmonyPrefix]
        public static void LoadNewLevelWaitPatch(RoundManager __instance)
        {
            Plugin.mls.LogInfo("FinishGeneratingNewLevelServerRpcPatch was called");
            // Remove it every level so it can be re-created when landing on "The Company" again. Also don't want this spawning on other moons so remove it there too
            if (__instance.currentLevel.levelID != 3 && RoundManagerCustom.GamblingMachine)
            {
                Plugin.mls.LogInfo("Despawning gambling machine...");
                RoundManagerCustom.DespawnGamblingMachineServerRpc();
            };

            // Only spawn this at "The Company" moon
            if (__instance.currentLevel.levelID == 3 && !RoundManagerCustom.GamblingMachine)
            {
                Plugin.mls.LogInfo("Spawning gambling machine...");
                RoundManagerCustom.SpawnGamblingMachineServerRpc();
            }

        }

    }
}
