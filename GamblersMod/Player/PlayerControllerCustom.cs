using GamblersMod.Patches;
using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GamblersMod.Player
{
    // PlayerControllerB but with my custom state
    internal class PlayerControllerCustom : NetworkBehaviour
    {
        private PlayerGamblingUIManager PlayerGamblingUIManager;
        private PlayerControllerB PlayerControllerOriginal;

        void Awake()
        {
            PlayerGamblingUIManager = gameObject.AddComponent<PlayerGamblingUIManager>();
            PlayerControllerOriginal = gameObject.GetComponent<PlayerControllerB>();
        }

        void Update()
        {
            if (!IsOwner)
            {
                return;
            }

            Camera gameplayCamera = PlayerControllerOriginal.gameplayCamera;

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
                if (gameObjectHitByRayCast.name.Contains("GamblingMachine"))
                {
                    PlayerGamblingUIManager.ShowInteractionText();

                    // Get the item the player is currently holding (null checks)
                    GrabbableObject currentlyHeldObjectInHand = PlayerControllerOriginal.ItemSlots[PlayerControllerOriginal.currentItemSlot];

                    GamblingMachine GamblingMachineHit = gameObjectHitByRayCast.GetComponent<GamblingMachine>();

                    // If in cooldown, show cooldown text
                    if (GamblingMachineHit.isInCooldownPhase())
                    {
                        PlayerGamblingUIManager.SetInteractionText($"Cooling down... {GamblingMachineHit.gamblingMachineCurrentCooldown}");
                    }
                    else
                    {

                        // string interactKeyName = PlayerControllerOriginal.playerActions.FindAction("Interact").GetBindingDisplayString(0);
                        string interactKeyName = IngamePlayerSettings.Instance.playerInput.actions.FindAction("Interact").GetBindingDisplayString(0); ;
                        PlayerGamblingUIManager.SetInteractionText($"Press {interactKeyName} to gamble");
                    }

                    // Object in hand so show subtext
                    if (currentlyHeldObjectInHand)
                    {
                        PlayerGamblingUIManager.SetInteractionSubText($"Scrap value on hand: ■{currentlyHeldObjectInHand.scrapValue}");
                    }
                    else
                    {
                        PlayerGamblingUIManager.SetInteractionSubText("Please hold a scrap on your hand");
                    }
                }

                // Handle gambling machine input
                // if (gameObjectHitByRayCast.name.Contains("GamblingMachine") && PlayerControllerOriginal.playerActions.FindAction("Interact").triggered)
                if (gameObjectHitByRayCast.name.Contains("GamblingMachine") && IngamePlayerSettings.Instance.playerInput.actions.FindAction("Interact").triggered)
                {
                    Plugin.mls.LogInfo($"Gambling machine was interacted with by: {PlayerControllerOriginal.playerUsername}");
                    GamblingMachine GamblingMachineHit = gameObjectHitByRayCast.GetComponent<GamblingMachine>();
                    handleGamblingMachineInput(GamblingMachineHit);
                }
            }
            // An interactable object was not hit (player is looking at something else)
            else
            {
                PlayerGamblingUIManager.HideInteractionText();
            }
        }

        [ServerRpc(RequireOwnership = false)]
        void ActivateGamblingMachineServerRPC(NetworkBehaviourReference GambleMachineHitRef, NetworkBehaviourReference scrapBeingGambledRef, ServerRpcParams serverRpcParams = default)
        {
            if (!IsServer) return;

            Plugin.mls.LogMessage("ActivateGamblingMachineServerRPC: Starting gambling machine cooldown phase in the server invoked by: " + serverRpcParams.Receive.SenderClientId);

            if (!GambleMachineHitRef.TryGet(out GamblingMachine GambleMachineHit))
            {
                Plugin.mls.LogError("ActivateGamblingMachineServerRPC: Failed to get gambling machine on server side.");
                return;
            }

            // Roll dice on server side and send roll value to clients
            int roll = GambleMachineHit.RollDice();

            ActivateGamblingMachineClientRPC(GambleMachineHitRef, scrapBeingGambledRef, roll);
        }

        [ClientRpc]
        void ActivateGamblingMachineClientRPC(NetworkBehaviourReference GambleMachineHitRef, NetworkBehaviourReference scrapBeingGambledRef, int roll)
        {
            Plugin.mls.LogInfo("ActivateGamblingMachineClientRPC: Activiating gambling machines on client...");

            if (!GambleMachineHitRef.TryGet(out GamblingMachine GambleMachineHit))
            {
                Plugin.mls.LogError("ActivateGamblingMachineClientRPC: Failed to get gambling machine on client side.");
                return;
            }

            // Set roll and update gambling machine state based on the roll
            GambleMachineHit.PlayDrumRoll();
            GambleMachineHit.SetRoll(roll);
            GambleMachineHit.GenerateGamblingOutcomeFromCurrentRoll();

            // Start cooldown for all clients
            GambleMachineHit.BeginGamblingMachineCooldown(() =>
            {
                if (!scrapBeingGambledRef.TryGet(out GrabbableObject scrapBeingGambled))
                {
                    Plugin.mls.LogError("ActivateGamblingMachineClientRPC: Failed to get scrap value on client side.");
                    return;
                }

                // Update scrap value for all client
                scrapBeingGambled.SetScrapValue(GambleMachineHit.GetScrapValueBasedOnGambledOutcome(scrapBeingGambled));
                GambleMachineHit.PlayGambleResultAudio();
            });
        }

        void handleGamblingMachineInput(GamblingMachine GamblingMachineHit)
        {
            // Get the item the player is currently holding (null checks)
            GrabbableObject currentlyHeldObjectInHand = PlayerControllerOriginal.ItemSlots[PlayerControllerOriginal.currentItemSlot];

            // Don't do anything if nothing in hand OR still in cooldown
            if (!currentlyHeldObjectInHand || GamblingMachineHit.isInCooldownPhase())
            {
                return;
            }

            Plugin.mls.LogMessage($"Scrap value of {currentlyHeldObjectInHand.name} on hand: ▊{currentlyHeldObjectInHand.scrapValue}");
            ActivateGamblingMachineServerRPC(GamblingMachineHit, currentlyHeldObjectInHand);
            PlayerGamblingUIManager.SetInteractionText($"Cooling down... {GamblingMachineHit.gamblingMachineCurrentCooldown}");
        }
    }
}
