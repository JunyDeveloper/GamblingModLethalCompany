using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using LC_API.BundleAPI;
using UnityEngine;
using UnityEngine.UI;

namespace GamblersMod.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerControllerBPatch
    {
        static private bool isGamblingInteractionTextShowing = false;
        static private GamblingUtility gamblingUtility = new GamblingUtility("gamblingMachine", "Press E to gamble");

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        static void AwakePatch() {
            var mls = BepInEx.Logging.Logger.CreateLogSource("PlayerControllerBPatch");
            mls.LogInfo("Test mod works");
            //  C:/Program Files (x86)/Steam/steamapps/common/Lethal Company/Lethal Company_Data/StreamingAssets
            mls.LogInfo($"Application.streamingAssetsPath: {Application.streamingAssetsPath}");
        }

        internal ManualLogSource mls;
        /**
         * 
         * 
         * 
          [HarmonyPatch("SetHoverTipAndCurrentInteractTrigger")]
        [HarmonyPrefix]
        static void SetHoverTipAndCurrentInteractTrigger(ref Camera ___gameplayCamera, ref TMPro.TextMeshProUGUI ___cursorTip, ref UnityEngine.UI.Image ___cursorIcon, ref UnityEngine.CanvasGroup ___usernameAlpha, ref UnityEngine.Canvas ___usernameCanvas) {
            var mls = BepInEx.Logging.Logger.CreateLogSource("PlayerControllerB::UPDATE");
            
            Vector3 playerPosition = ___gameplayCamera.transform.position;
            Vector3 forwardDirection = ___gameplayCamera.transform.forward;

            Ray interactionRay = new Ray(playerPosition, forwardDirection);
            RaycastHit interactionRayHit;
            float interactionRayLength = 5.0f;

            Vector3 interactionRayEndpoint = forwardDirection * interactionRayLength;

            bool hitfound = Physics.Raycast(interactionRay, out interactionRayHit, interactionRayLength);

            if (hitfound)
            {
                GameObject hitGameObject = interactionRayHit.transform.gameObject;
                if (hitGameObject.name == "GamblingMachine")
                {
                    Debug.Log("Gambling machine HIT!");
                    ___cursorIcon.enabled = true;
                    ___cursorTip.text = "Press E to die...";
                    ___usernameAlpha.alpha = 1f;
                    ___usernameCanvas.gameObject.SetActive(true);

                    return; // Ignore the remaining code 
                    
                }
            }


            // if(Physics.Raycast())
        }
         */


        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void UpdatePatch(PlayerControllerB __instance) {
            Camera gameplayCamera = __instance.gameplayCamera;
           // if (!gameplayCamera) return;
            var mls = BepInEx.Logging.Logger.CreateLogSource("PlayerControllerB::UPDATE");

            // If looking at snowman, log
            Vector3 playerPosition = gameplayCamera.transform.position;
            //mls.LogInfo($"playerPosition: ${playerPosition}");
            Vector3 forwardDirection = gameplayCamera.transform.forward;
            //mls.LogInfo($"forwardDirection: ${forwardDirection}");

            Ray interactionRay = new Ray(playerPosition, forwardDirection);
            //mls.LogInfo($"interactionRay: ${interactionRay}");
            RaycastHit interactionRayHit;
            float interactionRayLength = 5.0f;
            
          
            int maskToCastRayOnlyFOrInteractableObjects = 1 << 9;
            int layerMask5And3And18 = Convert.ToInt32("11111111111110111111111111010111", 2);
            LayerMask[] layersToIgnore = { LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("UI"), LayerMask.NameToLayer("LineOfSight") };
            bool hitfound = Physics.Raycast(interactionRay, out interactionRayHit, interactionRayLength, maskToCastRayOnlyFOrInteractableObjects);

            // 

            // If the raycast hit air, then...
            //if () { 

            //}

           // Debug.Log(hitfound);

            // Ignore raycast for these layers
           // if (!layersToIgnore.Contains(interactionRayHit.transform.gameObject.layer)) {
           //     Debug.Log($"HIT RELEVANT LAYER: ${interactionRayHit.transform.gameObject.layer}");
           // }


            
           if (interactionRayHit.collider) {
                GameObject gameObjectHitByRayCast = interactionRayHit.transform.gameObject;
                Collider gameObjectColliderHitByRayCast = interactionRayHit.collider;

                // Debug.Log($"{gameObjectHitByRayCast.layer} == {LayerMask.NameToLayer("Player")}");
                // These layers are eating up the ray hits


                // Ignore ray cast hits
                //if (!layersToIgnore.Contains(gameObjectHitByRayCast.layer))
               // {
                    if (hitfound && gameObjectColliderHitByRayCast)
                    {
                        if (gameObjectHitByRayCast.name == "GamblingMachine" && !isGamblingInteractionTextShowing)
                        {
                            Debug.Log("HIT GAMBLING MACHINE");
                            gamblingUtility.ShowInteractionText();
                            isGamblingInteractionTextShowing = true;
                        }
                        else if (gameObjectHitByRayCast.name != "GamblingMachine" || !gameObjectColliderHitByRayCast)
                        {
                            Debug.Log("DID NOT HIT GAMBLING MACHINE");
                            gamblingUtility.HideInteractionText();
                            isGamblingInteractionTextShowing = false;
                        }
                    }
                    else
                    {
                        Debug.Log("NO HITS");
                        gamblingUtility.HideInteractionText();
                        isGamblingInteractionTextShowing = false;
                    }
               // }
            }
            else
            {
               Debug.Log($"AIR HITS: {interactionRayHit.collider}");
               gamblingUtility.HideInteractionText();
               isGamblingInteractionTextShowing = false;
            }
            
        }
    }
}
