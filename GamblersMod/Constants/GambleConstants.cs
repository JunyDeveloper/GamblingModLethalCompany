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
        public readonly static string CONFIG_MAXCOOLDOWN = "Max Cooldown";
        public readonly static string CONFIG_NUMBER_OF_USES = "Number Of Uses";
        public readonly static string CONFIG_NUMBER_OF_MACHINES = "Number Of Machines";

        // Chance subsection keys
        public readonly static string CONFIG_JACKPOT_CHANCE_KEY = "Jackpot Chance";
        public readonly static string CONFIG_TRIPLE_CHANCE_KEY = "Triple Chance";
        public readonly static string CONFIG_DOUBLE_CHANCE_KEY = "Double Chance";
        public readonly static string CONFIG_HALVE_CHANCE_KEY = "Halve Chance";
        public readonly static string CONFIG_ZERO_CHANCE_KEY = "Zero Chance";

        // Multipliers subsection keys
        public readonly static string CONFIG_JACKPOT_MULTIPLIER = "Jackpot Multiplier";
        public readonly static string CONFIG_TRIPLE_MULTIPLIER = "Triple Multiplier";
        public readonly static string CONFIG_DOUBLE_MULTIPLIER = "Double Multiplier";
        public readonly static string CONFIG_HALVE_MULTIPLIER = "Halve Multiplier";
        public readonly static string CONFIG_ZERO_MULTIPLIER = "Zero Multiplier";

        // Audio subsection keys
        public readonly static string CONFIG_GAMBLING_MUSIC_ENABLED = "Music Enabled";
        public readonly static string CONFIG_GAMBLING_MUSIC_VOLUME = "Machine Music Volume";

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
