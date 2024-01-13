namespace GamblersMod.config
{
    public class GambleConstants
    {
        // Section keys
        public readonly static string GAMBLING_GENERAL_SECTION_KEY = "General Machine Settings";
        public readonly static string GAMBLING_CHANCE_SECTION_KEY = "Gambling Chances";
        public readonly static string GAMBLING_MULTIPLIERS_SECTION_KEY = "Gambling Multipliers";
        public readonly static string GAMBLING_AUDIO_SECTION_KEY = "Audio";

        // General Subsection keys
        public readonly static string CONFIG_MAXCOOLDOWN = "gamblingMachineMaxCooldown";
        public readonly static string CONFIG_NUMBER_OF_USES = "Number Of Uses";
        public readonly static string CONFIG_NUMBER_OF_MACHINES = "Number Of Machines";

        // Chance subsection keys
        public readonly static string CONFIG_JACKPOT_CHANCE_KEY = "JackpotChance";
        public readonly static string CONFIG_TRIPLE_CHANCE_KEY = "TripleChance";
        public readonly static string CONFIG_DOUBLE_CHANCE_KEY = "DoubleChance";
        public readonly static string CONFIG_HALVE_CHANCE_KEY = "HalveChance";
        public readonly static string CONFIG_ZERO_CHANCE_KEY = "ZeroChance";

        // Multipliers subsection keys
        public readonly static string CONFIG_JACKPOT_MULTIPLIER = "JackpotMultiplier";
        public readonly static string CONFIG_TRIPLE_MULTIPLIER = "TripleMultiplier";
        public readonly static string CONFIG_DOUBLE_MULTIPLIER = "DoubleMultiplier";
        public readonly static string CONFIG_HALVE_MULTIPLIER = "HalveMultiplier";
        public readonly static string CONFIG_ZERO_MULTIPLIER = "ZeroMultiplier";

        // Audio subsection keys
        public readonly static string CONFIG_GAMBLING_MUSIC_ENABLED = "GambleMachineMusicEnabled";
        public readonly static string CONFIG_GAMBLING_MUSIC_VOLUME = "GambleMachineMusicVolume";

        // Used for host and client message communication for configuration
        public readonly static string ON_HOST_RECIEVES_CLIENT_CONFIG_REQUEST = "OnHostRecievesClientConfigRequest";
        public readonly static string ON_CLIENT_RECIEVES_HOST_CONFIG_REQUEST = "OnClientRecievesHostConfigRequest";

        public struct GamblingOutcome
        {
            public static string JACKPOT = "JACKPOT";
            public static string TRIPLE = "TRIPLE";
            public static string DOUBLE = "DOUBLE";
            public static string HALVE = "HALVE";
            public static string REMOVE = "REMOVE";
            public static string DEFAULT = "DEFAULT";
        }
    }
}
