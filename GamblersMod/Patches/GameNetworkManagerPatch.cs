using HarmonyLib;
using Unity.Netcode;

namespace GamblersMod.Patches
{
    [HarmonyPatch(typeof(GameNetworkManager))]
    internal class GameNetworkManagerPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void StartPatch(GameNetworkManager __instance)
        {
            Plugin.mls.LogInfo("Adding Gambling machine to network prefab");
            NetworkManager.Singleton.AddNetworkPrefab(Plugin.GamblingMachine);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameNetworkManager), "StartDisconnect")]
        public static void StartDisconnectPatch()
        {
            Plugin.mls.LogInfo("Player disconnected. Resetting the user's configuration settings.");
            Plugin.CurrentUserConfig = Plugin.UserConfigSnapshot; // Reset the user's configuration settings
            GamblingMachineManager.Instance.Reset();
        }
    }
}
