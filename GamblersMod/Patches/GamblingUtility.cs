using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;

namespace GamblersMod.Patches
{
    internal class GamblingUtility
    {
        GameObject gamblingMachineInteractionTextCanvasObject;
        Canvas gamblingMachineInteractionTextCanvas;
        GameObject gamblingMachineInteractionTextObject;
        Text gamblingMachineInteractionText;

        string interactionName;
        string interactionText;

        public GamblingUtility(string name, string text) {
            gamblingMachineInteractionTextCanvasObject = new GameObject();
            gamblingMachineInteractionTextObject = new GameObject();
            interactionName = name;
            interactionText = text;

            gamblingMachineInteractionTextCanvasObject.name = $"{interactionName}InteractionTextCanvasObject";
            gamblingMachineInteractionTextCanvasObject.AddComponent<Canvas>();
            gamblingMachineInteractionTextCanvasObject.SetActive(false);

            gamblingMachineInteractionTextCanvas = gamblingMachineInteractionTextCanvasObject.GetComponent<Canvas>();
            gamblingMachineInteractionTextCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            gamblingMachineInteractionTextCanvasObject.AddComponent<CanvasScaler>();
            gamblingMachineInteractionTextCanvasObject.AddComponent<GraphicRaycaster>();

            gamblingMachineInteractionTextObject.name = $"{interactionName}InteractionTextObject";
            gamblingMachineInteractionTextObject.AddComponent<Text>();
            gamblingMachineInteractionTextObject.transform.localPosition = new Vector3(gamblingMachineInteractionTextCanvas.GetComponent<RectTransform>().rect.width / 2, gamblingMachineInteractionTextCanvas.GetComponent<RectTransform>().rect.height / 2, 0);

            gamblingMachineInteractionText = gamblingMachineInteractionTextObject.GetComponent<Text>();
            gamblingMachineInteractionText.text = interactionText;
            gamblingMachineInteractionText.alignment = TextAnchor.MiddleCenter;
            gamblingMachineInteractionText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");

            gamblingMachineInteractionText.transform.parent = gamblingMachineInteractionTextCanvasObject.transform;

            UnityEngine.Object.Instantiate(gamblingMachineInteractionTextCanvasObject);
        }

        public void ShowInteractionText() {
            //Debug.Log("Try to show");
            gamblingMachineInteractionTextCanvasObject.SetActive(true);
        }

        public void HideInteractionText()
        {
            //Debug.Log("Try to hide");
            gamblingMachineInteractionTextCanvasObject.SetActive(false);
        }
    }
}
