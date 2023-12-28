using GamblersMod.config;
using GameNetcodeStuff;
using HarmonyLib;
using Unity.Netcode;
using UnityEngine;
using static GamblersMod.config.GambleConstants;

namespace GamblersMod.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerControllerBPatch : MonoBehaviour
    {
        static private bool isGamblingInteractionTextShowing;
        static private PlayerGamblingUIManager PlayerGamblingUIManager;

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        static void AwakePatch(PlayerControllerB __instance)
        {
            isGamblingInteractionTextShowing = false;

            // Attach script which will manange the gambling UI on the player
            PlayerGamblingUIManager = __instance.gameObject.AddComponent<PlayerGamblingUIManager>();
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void UpdatePatch(PlayerControllerB __instance)
        {
            Camera gameplayCamera = __instance.gameplayCamera;

            Vector3 playerPosition = gameplayCamera.transform.position;
            Vector3 forwardDirection = gameplayCamera.transform.forward;
            Ray interactionRay = new Ray(playerPosition, forwardDirection);
            RaycastHit interactionRayHit;
            float interactionRayLength = 5.0f;

            int maskToCastRayOnlyForInteractableObjects = 1 << 9;
            //int layerMask5And3And18 = Convert.ToInt32("11111111111110111111111111010111", 2);
            //LayerMask[] layersToIgnore = { LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("UI"), LayerMask.NameToLayer("LineOfSight") };
            bool hitfound = Physics.Raycast(interactionRay, out interactionRayHit, interactionRayLength, maskToCastRayOnlyForInteractableObjects);

            // An Interactable object was hit
            if (interactionRayHit.collider)
            {
                GameObject gameObjectHitByRayCast = interactionRayHit.transform.gameObject;

                // Collider with the name "GamblingMachine" was hit, so show tooltip on gambling machine
                if (gameObjectHitByRayCast.name.Contains("GamblingMachine") && !isGamblingInteractionTextShowing)
                {
                    PlayerGamblingUIManager.ShowInteractionText();
                    isGamblingInteractionTextShowing = true;

                    // Get the item the player is currently holding (null checks)
                    GrabbableObject currentlyHeldObjectInHand = __instance.ItemSlots[__instance.currentItemSlot];


                    if (currentlyHeldObjectInHand)
                    {
                        PlayerGamblingUIManager.SetInteractionSubText($"Scrap value on hand: ${currentlyHeldObjectInHand.scrapValue}");
                    }
                    else
                    {
                        PlayerGamblingUIManager.SetInteractionSubText("Please hold a scrap on your hand");
                    }
                }

                // Handle gambling machine input
                if (gameObjectHitByRayCast.name.Contains("GamblingMachine") && __instance.playerActions.FindAction("Interact").triggered)
                {
                    Plugin.mls.LogInfo($"Gambling machine was interacted with by: {__instance.playerUsername}");
                    GamblingMachine GamblingMachineHit = gameObjectHitByRayCast.GetComponent<GamblingMachine>();
                    handleGamblingMachineInput(__instance, GamblingMachineHit);
                }
            }
            // An interactable object was not hit (player is looking at something else)
            else
            {
                PlayerGamblingUIManager.HideInteractionText();
                isGamblingInteractionTextShowing = false;
            }
        }

        static private void handleGamblingMachineInput(PlayerControllerB __instance, GamblingMachine GamblingMachineHit)
        {
            // Get the item the player is currently holding (null checks)
            GrabbableObject currentlyHeldObjectInHand = __instance.ItemSlots[__instance.currentItemSlot];

            // Don't do anything if nothing in hand OR still in cooldown
            if (!currentlyHeldObjectInHand || GamblingMachineHit.isInCooldownPhase())
            {
                return;
            }

            Plugin.mls.LogMessage($"Scrap value of {currentlyHeldObjectInHand.name} on hand: ${currentlyHeldObjectInHand.scrapValue}");

            // Start cooldown phase
            AudioSource.PlayClipAtPoint(Plugin.GamblingDrumrollScrapAudio, __instance.transform.position, 0.6f);
            GamblingMachineHit.BeginGamblingMachineCooldown();
            GamblingMachineHit.StartDrumRollPhase(__instance, currentlyHeldObjectInHand);
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
