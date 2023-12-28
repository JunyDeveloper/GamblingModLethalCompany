using System.Collections;
using GameNetcodeStuff;
using UnityEngine;
using static GamblersMod.config.GambleConstants;

namespace GamblersMod.Patches
{
    internal class GamblingMachine : MonoBehaviour
    {
        // Cooldown
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

        // Current state
        public float currentGamblingOutcomeMultiplier;
        public GamblingOutcome currentGamblingOutcome;

        PlayerGamblingUIManager GamblingMachineUIHelper;

        void Awake()
        {
            Plugin.mls.LogInfo("GamblingMachine has Awoken");

            // UI Manipulation by the gambling machine
            GamblingMachineUIHelper = gameObject.AddComponent<PlayerGamblingUIManager>();

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

            // Rolls
            rollMinValue = 1;
            rollMaxValue = jackpotPercentage + triplePercentage + doublePercentage + halvedPercentage + removedPercentage;

            // Default current state
            currentGamblingOutcomeMultiplier = 1;
            currentGamblingOutcome = GamblingOutcome.DEFAULT;

            // Gambling cooldown
            gamblingMachineMaxCooldown = 4;
            gamblingMachineCurrentCooldown = 0;
        }

        void Start()
        {
            Plugin.mls.LogInfo("GamblingMachine has Started");
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
        public void BeginGamblingMachineCooldown()
        {
            gamblingMachineCurrentCooldown = gamblingMachineMaxCooldown;
            StartCoroutine(CountdownCooldownCoroutine());
        }

        public void EndGamblingMachineCooldown()
        {
            Plugin.mls.LogMessage("End gambling machine cooldown");
            GamblingMachineUIHelper.SetInteractionText("Press E to gamble");
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
                GamblingMachineUIHelper.SetInteractionText($"Cooling down... {gamblingMachineCurrentCooldown}");
                yield return new WaitForSeconds(1);
                gamblingMachineCurrentCooldown -= 1;
                Plugin.mls.LogMessage($"Gambling machine cooldown: {gamblingMachineCurrentCooldown}");
            }
            EndGamblingMachineCooldown();
        }
    }
}
