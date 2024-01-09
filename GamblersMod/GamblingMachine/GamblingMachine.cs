using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using static GamblersMod.config.GambleConstants;

namespace GamblersMod.Patches
{
    internal class GamblingMachine : NetworkBehaviour
    {
        // Cooldown
        int gamblingMachineMaxCooldown;
        public int gamblingMachineCurrentCooldown = 0;

        // Multipliers for winning or losing
        float jackpotMultiplier;
        float tripleMultiplier;
        float doubleMultiplier;
        float halvedMultiplier;
        float zeroMultiplier;

        // Percentages for the outcome of gambling
        int jackpotPercentage;
        int triplePercentage;
        int doublePercentage;
        int halvedPercentage;
        int removedPercentage;

        // Audio
        bool isMusicEnabled = true;
        float musicVolume = 0.35f;

        // Dice roll range (inclusive)
        int rollMinValue;
        int rollMaxValue;
        int currentRoll = 100;

        // Current state
        public float currentGamblingOutcomeMultiplier = 1;
        public string currentGamblingOutcome = GamblingOutcome.DEFAULT;

        // TODO (Think about this better)
        private Coroutine CountdownCooldownCoroutineBeingRan;

        void Awake()
        {
            Plugin.mls.LogInfo("GamblingMachine has Awoken");

            //General
            gamblingMachineMaxCooldown = Plugin.CurrentUserConfig.configMaxCooldown;

            // Multipliers
            jackpotMultiplier = Plugin.CurrentUserConfig.configJackpotMultiplier;
            tripleMultiplier = Plugin.CurrentUserConfig.configTripleMultiplier;
            doubleMultiplier = Plugin.CurrentUserConfig.configDoubleMultiplier;
            halvedMultiplier = Plugin.CurrentUserConfig.configHalveMultiplier;
            zeroMultiplier = Plugin.CurrentUserConfig.configZeroMultiplier;

            // Chance
            jackpotPercentage = Plugin.CurrentUserConfig.configJackpotChance;
            triplePercentage = Plugin.CurrentUserConfig.configTripleChance;
            doublePercentage = Plugin.CurrentUserConfig.configDoubleChance;
            halvedPercentage = Plugin.CurrentUserConfig.configHalveChance;
            removedPercentage = Plugin.CurrentUserConfig.configZeroChance;

            // Audio 
            isMusicEnabled = Plugin.CurrentUserConfig.configGamblingMusicEnabled;
            musicVolume = Plugin.CurrentUserConfig.configGamblingMusicVolume;

            Plugin.mls.LogInfo($"GamblingMachine: gamblingMachineMaxCooldown loaded from config: {gamblingMachineMaxCooldown}");

            Plugin.mls.LogInfo($"GamblingMachine: jackpotMultiplier loaded from config: {jackpotMultiplier}");
            Plugin.mls.LogInfo($"GamblingMachine: tripleMultiplier loaded from config: {tripleMultiplier}");
            Plugin.mls.LogInfo($"GamblingMachine: doubleMultiplier loaded from config: {doubleMultiplier}");
            Plugin.mls.LogInfo($"GamblingMachine: halvedMultiplier loaded from config: {halvedMultiplier}");
            Plugin.mls.LogInfo($"GamblingMachine: zeroMultiplier loaded from config: {zeroMultiplier}");

            Plugin.mls.LogInfo($"GamblingMachine: jackpotPercentage loaded from config: {jackpotPercentage}");
            Plugin.mls.LogInfo($"GamblingMachine: triplePercentage loaded from config: {triplePercentage}");
            Plugin.mls.LogInfo($"GamblingMachine: doublePercentage loaded from config: {doublePercentage}");
            Plugin.mls.LogInfo($"GamblingMachine: halvedPercentage loaded from config: {halvedPercentage}");
            Plugin.mls.LogInfo($"GamblingMachine: removedPercentage loaded from config: {removedPercentage}");

            Plugin.mls.LogInfo($"GamblingMachine: gamblingMusicEnabled loaded from config: {isMusicEnabled}");
            Plugin.mls.LogInfo($"GamblingMachine: gamblingMusicVolume loaded from config: {musicVolume}");

            InitAudioSource();

            // Rolls
            rollMinValue = 1;
            rollMaxValue = jackpotPercentage + triplePercentage + doublePercentage + halvedPercentage + removedPercentage;
        }

        void Start()
        {
            Plugin.mls.LogInfo("GamblingMachine has Started");
        }

        public void GenerateGamblingOutcomeFromCurrentRoll()
        {
            bool isJackpotRoll = (currentRoll >= rollMinValue && currentRoll <= jackpotPercentage); // [0 - JACKPOT]

            int tripleStart = jackpotPercentage;
            int tripleEnd = jackpotPercentage + triplePercentage;
            bool isTripleRoll = (currentRoll > tripleStart && currentRoll <= tripleEnd); // [JACKPOT - (JACKPOT + TRIPLE)]

            int doubleStart = tripleEnd;
            int doubleEnd = tripleEnd + doublePercentage;
            bool isDoubleRoll = (currentRoll > doubleStart && currentRoll <= doubleEnd); // [(JACKPOT + TRIPLE) - (JACKPOT + TRIPLE + DOUBLE)]

            int halvedStart = doubleEnd;
            int halvedEnd = doubleEnd + halvedPercentage;
            bool isHalvedRoll = (currentRoll > halvedStart && currentRoll <= halvedEnd); // [(JACKPOT + TRIPLE + DOUBLE) - (JACKPOT + TRIPLE + DOUBLE + HALVED)]

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

        public void PlayGambleResultAudio(string outcome)
        {
            if (outcome == GamblingOutcome.JACKPOT)
            {
                AudioSource.PlayClipAtPoint(Plugin.GamblingJackpotScrapAudio, transform.position, 0.6f);
            }
            else if (outcome == GamblingOutcome.TRIPLE)
            {
                AudioSource.PlayClipAtPoint(Plugin.GamblingTripleScrapAudio, transform.position, 0.6f);
            }
            else if (outcome == GamblingOutcome.DOUBLE)
            {
                AudioSource.PlayClipAtPoint(Plugin.GamblingDoubleScrapAudio, transform.position, 0.6f);
            }
            else if (outcome == GamblingOutcome.HALVE)
            {
                AudioSource.PlayClipAtPoint(Plugin.GamblingHalveScrapAudio, transform.position, 0.6f);
            }
            else if (outcome == GamblingOutcome.REMOVE)
            {
                AudioSource.PlayClipAtPoint(Plugin.GamblingRemoveScrapAudio, transform.position, 0.6f);
            }
        }

        public void PlayDrumRoll()
        {
            AudioSource.PlayClipAtPoint(Plugin.GamblingDrumrollScrapAudio, transform.position, 0.6f);
        }

        public void BeginGamblingMachineCooldown(Action onCountdownFinish)
        {
            SetCurrentGamblingCooldownToMaxCooldown();
            if (CountdownCooldownCoroutineBeingRan != null)
            {
                StopCoroutine(CountdownCooldownCoroutineBeingRan); // Insurance
            }
            CountdownCooldownCoroutineBeingRan = StartCoroutine(CountdownCooldownCoroutine(onCountdownFinish));
        }

        public bool isInCooldownPhase()
        {
            return gamblingMachineCurrentCooldown > 0;
        }

        IEnumerator CountdownCooldownCoroutine(Action onCountdownFinish)
        {
            Plugin.mls.LogInfo("Start gambling machine cooldown");
            while (gamblingMachineCurrentCooldown > 0)
            {
                yield return new WaitForSeconds(1);
                gamblingMachineCurrentCooldown -= 1;
                Plugin.mls.LogMessage($"Gambling machine cooldown: {gamblingMachineCurrentCooldown}");
            }
            onCountdownFinish();
            Plugin.mls.LogMessage("End gambling machine cooldown");
        }

        public void SetCurrentGamblingCooldownToMaxCooldown()
        {
            gamblingMachineCurrentCooldown = gamblingMachineMaxCooldown;
        }

        public void SetRoll(int newRoll)
        {
            currentRoll = newRoll;
        }

        public int RollDice()
        {
            int roll = UnityEngine.Random.Range(rollMinValue, rollMaxValue);

            Plugin.mls.LogMessage($"rollMinValue: {rollMinValue}");
            Plugin.mls.LogMessage($"rollMaxValue: {rollMaxValue}");
            Plugin.mls.LogMessage($"Roll value: {currentRoll}");

            return roll;
        }

        public int GetScrapValueBasedOnGambledOutcome(GrabbableObject scrap)
        {
            return (int)Mathf.Floor(scrap.scrapValue * currentGamblingOutcomeMultiplier);
        }

        private void InitAudioSource()
        {
            // Music by default plays on awake
            if (!isMusicEnabled)
            {
                GetComponent<AudioSource>().Pause();
            }

            GetComponent<AudioSource>().volume = musicVolume;
        }
    }
}
