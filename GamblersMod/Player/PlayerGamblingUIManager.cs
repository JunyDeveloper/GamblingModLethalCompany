using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace GamblersMod.Patches
{
    public class PlayerGamblingUIManager : NetworkBehaviour
    {
        // UI Utility
        GameObject gamblingMachineInteractionTextCanvasObject; // The parent for the entire canvas
        Canvas gamblingMachineInteractionTextCanvas;
        GameObject gamblingMachineInteractionTextObject;
        GameObject gamblingMachineInteractionScrapInfoTextObject;
        Text gamblingMachineInteractionScrapInfoText;
        Text gamblingMachineInteractionText;

        string interactionName;
        string interactionText;

        void Awake()
        {
            // Gambling Interaction GUI
            gamblingMachineInteractionTextCanvasObject = new GameObject();
            gamblingMachineInteractionTextCanvasObject.transform.parent = transform;

            interactionName = "gamblingMachine";

            gamblingMachineInteractionTextCanvasObject.name = $"{interactionName}InteractionTextCanvasObject";
            gamblingMachineInteractionTextCanvasObject.AddComponent<Canvas>();
            gamblingMachineInteractionTextCanvasObject.SetActive(false);

            gamblingMachineInteractionTextCanvas = gamblingMachineInteractionTextCanvasObject.GetComponent<Canvas>();
            gamblingMachineInteractionTextCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            gamblingMachineInteractionTextCanvasObject.AddComponent<CanvasScaler>();
            gamblingMachineInteractionTextCanvasObject.AddComponent<GraphicRaycaster>();

            // Title
            gamblingMachineInteractionTextObject = new GameObject();
            gamblingMachineInteractionTextObject.name = $"{interactionName}InteractionTextObject";
            gamblingMachineInteractionTextObject.AddComponent<Text>();
            gamblingMachineInteractionTextObject.transform.localPosition = new Vector3((gamblingMachineInteractionTextCanvas.GetComponent<RectTransform>().rect.width / 2) - 20, (gamblingMachineInteractionTextCanvas.GetComponent<RectTransform>().rect.height / 2) - 50, 0);

            gamblingMachineInteractionText = gamblingMachineInteractionTextObject.GetComponent<Text>();
            gamblingMachineInteractionText.text = interactionText;
            gamblingMachineInteractionText.alignment = TextAnchor.MiddleCenter;
            gamblingMachineInteractionText.font = Plugin.GamblingFont;
            gamblingMachineInteractionText.rectTransform.sizeDelta = new Vector2(500, 400);
            gamblingMachineInteractionText.fontSize = 26;

            gamblingMachineInteractionText.transform.parent = gamblingMachineInteractionTextCanvasObject.transform;

            // Subtitle
            gamblingMachineInteractionScrapInfoTextObject = new GameObject();
            gamblingMachineInteractionScrapInfoTextObject.name = $"{interactionName}InteractionScrapInfoTextObject";
            gamblingMachineInteractionScrapInfoTextObject.AddComponent<Text>();
            gamblingMachineInteractionScrapInfoTextObject.transform.localPosition = new Vector3((gamblingMachineInteractionTextCanvas.GetComponent<RectTransform>().rect.width / 2) - 20, (gamblingMachineInteractionTextCanvas.GetComponent<RectTransform>().rect.height / 2) - 100, 0);

            gamblingMachineInteractionScrapInfoText = gamblingMachineInteractionScrapInfoTextObject.GetComponent<Text>();
            gamblingMachineInteractionScrapInfoText.text = interactionText;
            gamblingMachineInteractionScrapInfoText.alignment = TextAnchor.MiddleCenter;
            gamblingMachineInteractionScrapInfoText.font = Plugin.GamblingFont;
            gamblingMachineInteractionScrapInfoText.rectTransform.sizeDelta = new Vector2(500, 300);
            gamblingMachineInteractionScrapInfoText.fontSize = 18;
            gamblingMachineInteractionScrapInfoText.color = Color.green;

            gamblingMachineInteractionScrapInfoText.transform.parent = gamblingMachineInteractionTextCanvasObject.transform;

            // Hand icon
            // Vector3 interactionIconPosition = new Vector3((gamblingMachineInteractionTextCanvas.GetComponent<RectTransform>().rect.width / 2) - 20, (gamblingMachineInteractionTextCanvas.GetComponent<RectTransform>().rect.height / 2) - 100, 0);
            // GameObject interactionIcon = UnityEngine.Object.Instantiate(Plugin.GamblingHandIcon, interactionIconPosition, Quaternion.Euler(0, 90, 0));
            // interactionIcon.transform.parent = gamblingMachineInteractionTextCanvasObject.transform;

        }

        public void SetInteractionText(string text)
        {
            gamblingMachineInteractionText.text = text;
        }

        public void SetInteractionSubText(string text)
        {
            gamblingMachineInteractionScrapInfoText.text = text;
        }

        public void ShowInteractionText()
        {
            gamblingMachineInteractionTextCanvasObject.SetActive(true);
        }

        public void HideInteractionText()
        {
            gamblingMachineInteractionTextCanvasObject.SetActive(false);
        }
    }
}
