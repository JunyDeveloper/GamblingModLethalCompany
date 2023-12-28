using System.Collections;
using GameNetcodeStuff;
using UnityEngine;
using UnityEngine.UI;

namespace GamblersMod.Patches
{
    internal class GamblingUtility : MonoBehaviour
    {
        public enum GamblingOutcome { JACKPOT, TRIPLE, DOUBLE, HALVE, REMOVE, DEFAULT }

        int gamblingMachineMaxCooldown;
        public int gamblingMachineCurrentCooldown;

        // Multipliers for winning or losing
        int jackpotMultiplier;
        int tripleMultiplier;
        int doubleMultiplier;
        float halvedMultiplier;

        // Percentages for the outcome of gambling
        int jackpotPercentage;
        int triplePercentage;
        int doublePercentage;
        int halvedPercentage;
        int removedPercentage;
        int zeroMultiplier;

        // Dice roll range (inclusive)
        int rollMinValue;
        int rollMaxValue;

        public float currentGamblingOutcomeMultiplier;
        public GamblingOutcome currentGamblingOutcome;

        // UI Utility
        GameObject gamblingMachineInteractionTextCanvasObject;
        Canvas gamblingMachineInteractionTextCanvas;
        GameObject gamblingMachineInteractionTextObject;
        GameObject gamblingMachineInteractionScrapInfoTextObject;
        Text gamblingMachineInteractionScrapInfoText;
        Text gamblingMachineInteractionText;
        private static GamblingUtility Instance;

        string interactionName;
        string interactionText;

        void Awake()
        {
            Plugin.mls.LogInfo($"Gambling utility has awoken");

            DontDestroyOnLoad(gameObject);
        }

        public GamblingUtility()
        {
            Plugin.mls.LogInfo("GamblingUtility constructor");
            // Gambling rolling state
            jackpotMultiplier = Plugin.UserConfig.configJackpotMultiplier;
            tripleMultiplier = Plugin.UserConfig.configTripleMultiplier;
            doubleMultiplier = Plugin.UserConfig.configDoubleMultiplier;
            halvedMultiplier = Plugin.UserConfig.configHalveMultiplier;
            zeroMultiplier = Plugin.UserConfig.configZeroMultiplier;

            jackpotPercentage = Plugin.UserConfig.configJackpotChance;
            triplePercentage = Plugin.UserConfig.configTripleChance;
            doublePercentage = Plugin.UserConfig.configDoubleChance;
            halvedPercentage = Plugin.UserConfig.configHalveChance;
            removedPercentage = Plugin.UserConfig.configZeroChance;

            Plugin.mls.LogInfo($"jackpotMultiplier loaded from config: {jackpotMultiplier}");
            Plugin.mls.LogInfo($"tripleMultiplier loaded from config: {tripleMultiplier}");
            Plugin.mls.LogInfo($"doubleMultiplier loaded from config: {doubleMultiplier}");
            Plugin.mls.LogInfo($"halvedMultiplier loaded from config: {halvedMultiplier}");
            Plugin.mls.LogInfo($"zeroMultiplier loaded from config: {zeroMultiplier}");

            Plugin.mls.LogInfo($"jackpotPercentage loaded from config: {jackpotPercentage}");
            Plugin.mls.LogInfo($"triplePercentage loaded from config: {triplePercentage}");
            Plugin.mls.LogInfo($"doublePercentage loaded from config: {doublePercentage}");
            Plugin.mls.LogInfo($"halvedPercentage loaded from config: {halvedPercentage}");
            Plugin.mls.LogInfo($"removedPercentage loaded from config: {removedPercentage}");

            rollMinValue = 1;
            rollMaxValue = jackpotPercentage + triplePercentage + doublePercentage + halvedPercentage + removedPercentage;

            currentGamblingOutcomeMultiplier = 1;
            currentGamblingOutcome = GamblingOutcome.DEFAULT;

            // Gambling cooldown
            gamblingMachineMaxCooldown = 4;
            gamblingMachineCurrentCooldown = 0;

            // Gambling Interaction GUI
            gamblingMachineInteractionTextCanvasObject = new GameObject();

            interactionName = "gamblingMachine";
            interactionText = "Press E to gamble";

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
            gamblingMachineInteractionText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            gamblingMachineInteractionText.rectTransform.sizeDelta = new Vector2(300, 200);
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
            gamblingMachineInteractionScrapInfoText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            gamblingMachineInteractionScrapInfoText.rectTransform.sizeDelta = new Vector2(300, 200);
            gamblingMachineInteractionScrapInfoText.fontSize = 15;
            gamblingMachineInteractionScrapInfoText.color = Color.green;

            gamblingMachineInteractionScrapInfoText.transform.parent = gamblingMachineInteractionTextCanvasObject.transform;

            UnityEngine.Object.Instantiate(gamblingMachineInteractionTextCanvasObject);
        }

        public void GenerateGamblingOutcome()
        {
            int roll = Random.Range(rollMinValue, rollMaxValue);

            Plugin.mls.LogMessage($"rollMinValue: {rollMinValue}");
            Plugin.mls.LogMessage($"rollMaxValue: {rollMaxValue}");
            Plugin.mls.LogMessage($"Roll value: {roll}");

            bool isJackpotRoll = (roll >= rollMinValue && roll <= jackpotPercentage); // [0 - JACKPOT]

            int tripleStart = jackpotPercentage;
            int tripleEnd = jackpotPercentage + triplePercentage;
            bool isTripleRoll = (roll > tripleStart && roll <= tripleEnd); // [JACKPOT - (JACKPOT + TRIPLE)]

            int doubleStart = tripleEnd;
            int doubleEnd = tripleEnd + doublePercentage;
            bool isDoubleRoll = (roll > doubleStart && roll <= doubleEnd); // [(JACKPOT + TRIPLE) - (JACKPOT + TRIPLE + DOUBLE)]

            int halvedStart = doubleEnd;
            int halvedEnd = doubleEnd + halvedPercentage;
            bool isHalvedRoll = (roll > halvedStart && roll <= halvedEnd); // [(JACKPOT + TRIPLE + DOUBLE) - (JACKPOT + TRIPLE + DOUBLE + HALVED)]

            if (isJackpotRoll)
            {
                Plugin.mls.LogMessage($"Rolled Jackpot");
                currentGamblingOutcomeMultiplier = jackpotMultiplier;
                currentGamblingOutcome = GamblingOutcome.JACKPOT;
            }
            else if (isTripleRoll)
            {
                Plugin.mls.LogMessage($"Rolled Triple");
                currentGamblingOutcomeMultiplier = tripleMultiplier;
                currentGamblingOutcome = GamblingOutcome.TRIPLE;
            }
            else if (isDoubleRoll)
            {
                Plugin.mls.LogMessage($"Rolled Double");
                currentGamblingOutcomeMultiplier = doubleMultiplier;
                currentGamblingOutcome = GamblingOutcome.DOUBLE;
            }
            else if (isHalvedRoll)
            {
                Plugin.mls.LogMessage($"Rolled Halved");
                currentGamblingOutcomeMultiplier = halvedMultiplier;
                currentGamblingOutcome = GamblingOutcome.HALVE;
            }
            else
            {
                Plugin.mls.LogMessage($"Rolled Remove");
                currentGamblingOutcomeMultiplier = zeroMultiplier;
                currentGamblingOutcome = GamblingOutcome.REMOVE;
            }
        }

        public void StartDrumRollPhase(PlayerControllerB __instance, GrabbableObject currentlyHeldObjectInHand)
        {
            StartCoroutine(StartGamblingMachineDrumRollPhaseCoroutine(__instance, currentlyHeldObjectInHand));
        }

        IEnumerator StartGamblingMachineDrumRollPhaseCoroutine(PlayerControllerB __instance, GrabbableObject currentlyHeldObjectInHand)
        {
            yield return new WaitForSeconds(gamblingMachineMaxCooldown);

            GenerateGamblingOutcome();
            int newScrapValue = (int)Mathf.Floor(currentlyHeldObjectInHand.scrapValue * currentGamblingOutcomeMultiplier);
            currentlyHeldObjectInHand.SetScrapValue(newScrapValue);

            if (currentGamblingOutcome == GamblingOutcome.JACKPOT)
            {
                Plugin.mls.LogMessage($"JACKPOT: ${currentlyHeldObjectInHand.scrapValue}");
                AudioSource.PlayClipAtPoint(Plugin.GamblingJackpotScrapAudio, __instance.transform.position, 0.6f);
            }
            else if (currentGamblingOutcome == GamblingOutcome.TRIPLE)
            {
                Plugin.mls.LogMessage($"TRIPLE: ${currentlyHeldObjectInHand.scrapValue}");
                AudioSource.PlayClipAtPoint(Plugin.GamblingTripleScrapAudio, __instance.transform.position, 0.6f);
            }
            else if (currentGamblingOutcome == GamblingOutcome.DOUBLE)
            {
                Plugin.mls.LogMessage($"DOUBLE: ${currentlyHeldObjectInHand.scrapValue}");
                AudioSource.PlayClipAtPoint(Plugin.GamblingDoubleScrapAudio, __instance.transform.position, 0.6f);
            }
            else if (currentGamblingOutcome == GamblingOutcome.HALVE)
            {
                Plugin.mls.LogMessage($"HALVE: ${currentlyHeldObjectInHand.scrapValue}");
                AudioSource.PlayClipAtPoint(Plugin.GamblingHalveScrapAudio, __instance.transform.position, 0.6f);
            }
            else if (currentGamblingOutcome == GamblingOutcome.REMOVE)
            {
                Plugin.mls.LogMessage($"REMOVE: ${currentlyHeldObjectInHand.scrapValue}");
                AudioSource.PlayClipAtPoint(Plugin.GamblingRemoveScrapAudio, __instance.transform.position, 0.6f);
                //Object.Destroy(currentlyHeldObjectInHand); // can destroy on network
            }
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

        public void BeginGamblingMachineCooldown()
        {
            gamblingMachineCurrentCooldown = gamblingMachineMaxCooldown;
            StartCoroutine(CountdownCooldownCoroutine());
        }

        public void EndGamblingMachineCooldown()
        {
            Plugin.mls.LogMessage("End gambling machine cooldown");
            SetInteractionText("Press E to gamble");
        }

        public bool isInCooldownPhase()
        {
            return gamblingMachineCurrentCooldown > 0;
        }

        IEnumerator CountdownCooldownCoroutine()
        {
            Plugin.mls.LogMessage("Start gambling machine cooldown");
            while (gamblingMachineCurrentCooldown > 0)
            {
                SetInteractionText($"Cooling down... {gamblingMachineCurrentCooldown}");
                yield return new WaitForSeconds(1);
                gamblingMachineCurrentCooldown -= 1;
                Plugin.mls.LogMessage($"Gambling machine cooldown: {gamblingMachineCurrentCooldown}");
            }
            EndGamblingMachineCooldown();
        }
    }
}
