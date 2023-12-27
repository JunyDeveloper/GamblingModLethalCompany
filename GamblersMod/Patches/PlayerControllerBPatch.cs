using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace GamblersMod.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerControllerBPatch : MonoBehaviour
    {
        static private bool isGamblingInteractionTextShowing;
        static private GamblingUtility gamblingUtility;

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        static void AwakePatch(PlayerControllerB __instance)
        {
            isGamblingInteractionTextShowing = false;
            // TODO: Press (INTERACTION_KEY) to gamble
            // gamblingUtility = new GamblingUtility("gamblingMachine", "Press E to gamble");
            __instance.gameObject.AddComponent<GamblingUtility>();
            gamblingUtility = __instance.gameObject.GetComponent<GamblingUtility>();
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

            int maskToCastRayOnlyFOrInteractableObjects = 1 << 9;
            //int layerMask5And3And18 = Convert.ToInt32("11111111111110111111111111010111", 2);
            //LayerMask[] layersToIgnore = { LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("UI"), LayerMask.NameToLayer("LineOfSight") };
            bool hitfound = Physics.Raycast(interactionRay, out interactionRayHit, interactionRayLength, maskToCastRayOnlyFOrInteractableObjects);

            // An Interactable object was hit
            if (interactionRayHit.collider)
            {
                GameObject gameObjectHitByRayCast = interactionRayHit.transform.gameObject;

                // Collider with the name "GamblingMachine" was hit, so show tooltip on gambling machine
                if (gameObjectHitByRayCast.name.Contains("GamblingMachine") && !isGamblingInteractionTextShowing)
                {
                    gamblingUtility.ShowInteractionText();
                    isGamblingInteractionTextShowing = true;

                    // Get the item the player is currently holding (null checks)
                    GrabbableObject currentlyHeldObjectInHand = __instance.ItemSlots[__instance.currentItemSlot];


                    if (currentlyHeldObjectInHand)
                    {
                        gamblingUtility.SetInteractionSubText($"Scrap value on hand: ${currentlyHeldObjectInHand.scrapValue}");
                    }
                    else
                    {
                        gamblingUtility.SetInteractionSubText("Please hold a scrap on your hand");
                    }
                }

                // Handle gambling machine input
                if (gameObjectHitByRayCast.name.Contains("GamblingMachine") && __instance.playerActions.FindAction("Interact").triggered)
                {
                    Plugin.mls.LogInfo($"Gambling machine was interacted with by: {__instance.playerUsername}");
                    handleGamblingMachineInput(__instance);
                }
            }
            // An interactable object was not hit (player is looking at something else)
            else
            {
                gamblingUtility.HideInteractionText();
                isGamblingInteractionTextShowing = false;
            }
        }

        static private void handleGamblingMachineInput(PlayerControllerB __instance)
        {
            // Get the item the player is currently holding (null checks)
            GrabbableObject currentlyHeldObjectInHand = __instance.ItemSlots[__instance.currentItemSlot];

            // Don't do anything if nothing in hand OR still in cooldown
            if (!currentlyHeldObjectInHand || gamblingUtility.isInCooldownPhase())
            {
                return;
            }

            Plugin.mls.LogMessage($"Scrap value of {currentlyHeldObjectInHand.name} on hand: ${currentlyHeldObjectInHand.scrapValue}");

            // Start cooldown phase
            AudioSource.PlayClipAtPoint(Plugin.GamblingDrumrollScrapAudio, __instance.transform.position, 0.6f);
            gamblingUtility.BeginGamblingMachineCooldown();
            gamblingUtility.StartDrumRollPhase(__instance, currentlyHeldObjectInHand);
        }
    }
}
