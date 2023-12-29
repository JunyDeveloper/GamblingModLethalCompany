using GamblersMod.config;
using GamblersMod.Player;
using GameNetcodeStuff;
using HarmonyLib;
using Unity.Netcode;
using static GamblersMod.config.GambleConstants;

namespace GamblersMod.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerControllerBPatch
    {
        [HarmonyPatch("Awake")]
        [HarmonyPrefix]
        public static void Awake(PlayerControllerB __instance)
        {
            __instance.gameObject.AddComponent<PlayerControllerCustom>();
        }

        [HarmonyPatch(typeof(PlayerControllerB), "ConnectClientToPlayerObject")]
        [HarmonyPostfix]
        public static void ConnectClientToPlayerObjectPatch()
        {
            Plugin.mls.LogInfo($"ConnectClientToPlayerObjectPatch");

            // Register callback for host when client connects
            if (NetworkManager.Singleton.IsHost)
            {
                Plugin.mls.LogInfo($"Registering host config message handler: {Plugin.modGUID}_{ON_HOST_RECIEVES_CLIENT_CONFIG_REQUEST}");
                NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler($"{Plugin.modGUID}_{ON_HOST_RECIEVES_CLIENT_CONFIG_REQUEST}", GambleConfigNetworkHelper.OnHostRecievesClientConfigRequest);
                return;
            }

            // Client connected, so they need to request the configuration file from the host
            NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler($"{Plugin.modGUID}_{ON_CLIENT_RECIEVES_HOST_CONFIG_REQUEST}", GambleConfigNetworkHelper.OnClientRecievesHostConfigRequest);
            GambleConfigNetworkHelper.StartClientRequestConfigFromHost();
        }
    }
}
