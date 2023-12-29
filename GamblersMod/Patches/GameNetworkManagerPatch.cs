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
            //__instance.GetComponent<NetworkManager>().AddNetworkPrefab(Plugin.GamblingMachine);
        }
    }
}
