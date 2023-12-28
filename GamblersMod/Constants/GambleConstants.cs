namespace GamblersMod.config
{
    public class GambleConstants
    {
        public readonly static string GAMBLING_CHANCE_SECTION_KEY = "Gambling Chances";
        public readonly static string GAMBLING_MULTIPLIERS_SECTION_KEY = "Gambling Multipliers";

        public readonly static string CONFIG_JACKPOT_CHANCE_KEY = "JackpotChance";
        public readonly static string CONFIG_TRIPLE_CHANCE_KEY = "TripleChance";
        public readonly static string CONFIG_DOUBLE_CHANCE_KEY = "DoubleChance";
        public readonly static string CONFIG_HALVE_CHANCE_KEY = "HalveChance";
        public readonly static string CONFIG_ZERO_CHANCE_KEY = "ZeroChance";

        public readonly static string CONFIG_JACKPOT_MULTIPLIER = "JackpotMultiplier";
        public readonly static string CONFIG_TRIPLE_MULTIPLIER = "TripleMultiplier";
        public readonly static string CONFIG_DOUBLE_MULTIPLIER = "DoubleMultiplier";
        public readonly static string CONFIG_HALVE_MULTIPLIER = "HalveMultiplier";
        public readonly static string CONFIG_ZERO_MULTIPLIER = "ZeroMultiplier";

        // Used for host and client message communication for configuration
        public readonly static string ON_HOST_RECIEVES_CLIENT_CONFIG_REQUEST = "OnHostRecievesClientConfigRequest";
        public readonly static string ON_CLIENT_RECIEVES_HOST_CONFIG_REQUEST = "OnClientRecievesHostConfigRequest";

        public enum GamblingOutcome { JACKPOT, TRIPLE, DOUBLE, HALVE, REMOVE, DEFAULT }
    }
}
