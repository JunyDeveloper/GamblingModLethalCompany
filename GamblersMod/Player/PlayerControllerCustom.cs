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
        public bool isUsingGamblingMachine; // Can only use one gambling machine at a time

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
                    else if (GamblingMachineHit.numberOfUses <= 0)
                    {
                        PlayerGamblingUIManager.SetInteractionText($"This machine is all used up");
                    }
                    else
                    {
                        string interactKeyName = IngamePlayerSettings.Instance.playerInput.actions.FindAction("Interact").GetBindingDisplayString(0);
                        // string numberOfUses = GamblingMachineHit.numberOfUses.ToString();

                        if (isUsingGamblingMachine)
                        {
                            PlayerGamblingUIManager.SetInteractionText($"You're already using a machine");
                        }
                        else
                        {
                            PlayerGamblingUIManager.SetInteractionText($"Gamble: [{interactKeyName}]");
                        }


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

        public void ReleaseGamblingMachineLock()
        {
            Plugin.mls.LogInfo($"Releasing gambling machine lock for: {OwnerClientId}");
            isUsingGamblingMachine = false;
        }

        public void LockGamblingMachine()
        {
            Plugin.mls.LogInfo($"Locking gambling machine for: {OwnerClientId}");
            isUsingGamblingMachine = true;
        }

        void handleGamblingMachineInput(GamblingMachine GamblingMachineHit)
        {
            // Get the item the player is currently holding (null checks)
            GrabbableObject currentlyHeldObjectInHand = PlayerControllerOriginal.ItemSlots[PlayerControllerOriginal.currentItemSlot];

            // Don't do anything if nothing in hand OR still in cooldown
            if (!currentlyHeldObjectInHand)
            {
                return;
            }

            if (GamblingMachineHit.isInCooldownPhase() || GamblingMachineHit.numberOfUses <= 0 || isUsingGamblingMachine)
            {
                return;
            }

            Plugin.mls.LogInfo($"Gambling machine was interacted with by: {PlayerControllerOriginal.playerUsername}");

            GamblingMachineHit.SetCurrentGamblingCooldownToMaxCooldown();

            Plugin.mls.LogMessage($"Scrap value of {currentlyHeldObjectInHand.name} on hand: ▊{currentlyHeldObjectInHand.scrapValue}");
            GamblingMachineHit.ActivateGamblingMachineServerRPC(currentlyHeldObjectInHand, this);
            PlayerGamblingUIManager.SetInteractionText($"Cooling down... {GamblingMachineHit.gamblingMachineCurrentCooldown}");
        }
    }
}
