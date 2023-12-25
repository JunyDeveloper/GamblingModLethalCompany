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
        static void Awake() {
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
        static void Update(ref Camera ___gameplayCamera) {
            var mls = BepInEx.Logging.Logger.CreateLogSource("PlayerControllerB::UPDATE");
           // mls.LogInfo($"Update START");

            if (UnityInput.Current.GetKeyDown(KeyCode.F9)) { 
                
            }

            // If looking at snowman, log
            Vector3 playerPosition = ___gameplayCamera.transform.position;
            //mls.LogInfo($"playerPosition: ${playerPosition}");
            Vector3 forwardDirection = ___gameplayCamera.transform.forward;
            //mls.LogInfo($"forwardDirection: ${forwardDirection}");

            Ray interactionRay = new Ray(playerPosition, forwardDirection);
            //mls.LogInfo($"interactionRay: ${interactionRay}");
            RaycastHit interactionRayHit;
            float interactionRayLength = 5.0f;

            Vector3 interactionRayEndpoint = forwardDirection * interactionRayLength;
            //Debug.DrawLine(playerPosition, interactionRayEndpoint);

            bool hitfound = Physics.Raycast(interactionRay, out interactionRayHit, interactionRayLength);
            Debug.Log($"Hitfound: ${hitfound}");
            // 1) Add tags so random UI collissions wont happen
            if (hitfound && interactionRayHit.transform.gameObject.name == "GamblingMachine")
            {
                GameObject hitGameObject = interactionRayHit.transform.gameObject;
                if (!hitGameObject.name.Contains("Player"))
                {
                    Debug.Log(hitGameObject.name);
                    if (hitGameObject.name == "GamblingMachine" && !isGamblingInteractionTextShowing)
                    {
                        gamblingUtility.ShowInteractionText();
                        isGamblingInteractionTextShowing = true;
                        /*
                        GameObject gamblingMachineInteractionTextCanvasObject = new GameObject();
                        gamblingMachineInteractionTextCanvasObject.name = "gamblingMachineInteractionTextCanvasObject";
                        gamblingMachineInteractionTextCanvasObject.AddComponent<Canvas>();

                        Canvas gamblingMachineInteractionTextCanvas;
                        gamblingMachineInteractionTextCanvas = gamblingMachineInteractionTextCanvasObject.GetComponent<Canvas>();
                        gamblingMachineInteractionTextCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                        gamblingMachineInteractionTextCanvasObject.AddComponent<CanvasScaler>();
                        gamblingMachineInteractionTextCanvasObject.AddComponent<GraphicRaycaster>();

                        GameObject gamblingMachineInteractionTextObject = new GameObject();
                        gamblingMachineInteractionTextObject.name = "gamblingMachineInteractionTextObject";
                        gamblingMachineInteractionTextObject.AddComponent<Text>();
                        gamblingMachineInteractionTextObject.transform.localPosition = new Vector3(gamblingMachineInteractionTextCanvas.GetComponent<RectTransform>().rect.width / 2, gamblingMachineInteractionTextCanvas.GetComponent<RectTransform>().rect.height / 2, 0);

                        Debug.Log(gamblingMachineInteractionTextCanvas.GetComponent<RectTransform>().rect.width / 2);
                        Debug.Log(gamblingMachineInteractionTextCanvas.GetComponent<RectTransform>().rect.height / 2);

                        Text gamblingMachineInteractionText;
                        gamblingMachineInteractionText = gamblingMachineInteractionTextObject.GetComponent<Text>();
                        gamblingMachineInteractionText.text = "Press E to win";
                        gamblingMachineInteractionText.alignment = TextAnchor.MiddleCenter;
                        gamblingMachineInteractionText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");

                        gamblingMachineInteractionText.transform.parent = gamblingMachineInteractionTextCanvasObject.transform;

                        UnityEngine.Object.Instantiate(gamblingMachineInteractionTextCanvasObject);
                        */
                    }
                    else if (hitGameObject.name != "GamblingMachine")
                    {
                        gamblingUtility.HideInteractionText();
                        isGamblingInteractionTextShowing = false;
                    }
                }
            }
            else {
                gamblingUtility.HideInteractionText();
                isGamblingInteractionTextShowing = false;
            }


            // if(Physics.Raycast())
        }
        /**
         * 
          [HarmonyPatch(typeof(JesterAI), "Start")]
        [HarmonyPostfix]
        private static void Postfix(JesterAI __instance)
        {
            screamingSFX = BundleLoader.GetLoadedAsset<AudioClip>("Assets/assetbundle/Audio/MyWay.wav");
            __instance.screamingSFX = BundleLoader.GetLoadedAsset<AudioClip>("Assets/assetbundle/Audio/MyWay.wav");
        }
         
         
         */

    }
}
