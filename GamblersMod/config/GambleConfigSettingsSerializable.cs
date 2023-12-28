using System;
using BepInEx.Configuration;
using static GamblersMod.config.GambleConstants;

namespace GamblersMod.config
{
    [Serializable]
    public class GambleConfigSettingsSerializable
    {
        public int configJackpotChance;
        public int configTripleChance;
        public int configDoubleChance;
        public int configHalveChance;
        public int configZeroChance;

        public int configJackpotMultiplier;
        public int configTripleMultiplier;
        public int configDoubleMultiplier;
        public float configHalveMultiplier;
        public int configZeroMultiplier;

        public GambleConfigSettingsSerializable(ConfigFile configFile)
        {
            configFile.Bind(GAMBLING_CHANCE_SECTION_KEY, CONFIG_JACKPOT_CHANCE_KEY, 3, "Chance to roll a jackpot. Ex. If set to 3, you have a 3% chance to get a jackpot. Make sure ALL your chance values add up to 100 or else the math won't make sense!");
            configFile.Bind(GAMBLING_CHANCE_SECTION_KEY, CONFIG_TRIPLE_CHANCE_KEY, 11, "Chance to roll a triple. Ex. If set to 11, you have a 11% chance to get a triple. Make sure ALL your chance values add up to 100 or else the math won't make sense!");
            configFile.Bind(GAMBLING_CHANCE_SECTION_KEY, CONFIG_DOUBLE_CHANCE_KEY, 27, "Chance to roll a double. Ex. If set to 27, you have a 27% chance to get a double. Make sure ALL your chance values add up to 100 or else the math won't make sense!");
            configFile.Bind(GAMBLING_CHANCE_SECTION_KEY, CONFIG_HALVE_CHANCE_KEY, 47, "Chance to roll a halve. Ex. If set to 47, you have a 47% chance to get a halve. Make sure ALL your chance values add up to 100 or else the math won't make sense!");
            configFile.Bind(GAMBLING_CHANCE_SECTION_KEY, CONFIG_ZERO_CHANCE_KEY, 12, "Chance to roll a zero. Ex. If set to 12, you have a 12% chance to get a zero. Make sure ALL your chance values add up to 100 or else the math won't make sense!");

            configFile.Bind(GAMBLING_MULTIPLIERS_SECTION_KEY, CONFIG_JACKPOT_MULTIPLIER, 10, "Jackpot multiplier");
            configFile.Bind(GAMBLING_MULTIPLIERS_SECTION_KEY, CONFIG_TRIPLE_MULTIPLIER, 3, "Triple multiplier");
            configFile.Bind(GAMBLING_MULTIPLIERS_SECTION_KEY, CONFIG_DOUBLE_MULTIPLIER, 2, "Double multiplier");
            configFile.Bind(GAMBLING_MULTIPLIERS_SECTION_KEY, CONFIG_HALVE_MULTIPLIER, 0.5f, "Halve multiplier");
            configFile.Bind(GAMBLING_MULTIPLIERS_SECTION_KEY, CONFIG_ZERO_MULTIPLIER, 0, "Zero multiplier");

            configJackpotChance = GetConfigFileKeyValue<int>(configFile, GAMBLING_CHANCE_SECTION_KEY, CONFIG_JACKPOT_CHANCE_KEY);
            configTripleChance = GetConfigFileKeyValue<int>(configFile, GAMBLING_CHANCE_SECTION_KEY, CONFIG_TRIPLE_CHANCE_KEY);
            configDoubleChance = GetConfigFileKeyValue<int>(configFile, GAMBLING_CHANCE_SECTION_KEY, CONFIG_DOUBLE_CHANCE_KEY);
            configHalveChance = GetConfigFileKeyValue<int>(configFile, GAMBLING_CHANCE_SECTION_KEY, CONFIG_HALVE_CHANCE_KEY);
            configZeroChance = GetConfigFileKeyValue<int>(configFile, GAMBLING_CHANCE_SECTION_KEY, CONFIG_ZERO_CHANCE_KEY);

            configJackpotMultiplier = GetConfigFileKeyValue<int>(configFile, GAMBLING_MULTIPLIERS_SECTION_KEY, CONFIG_JACKPOT_MULTIPLIER);
            configTripleMultiplier = GetConfigFileKeyValue<int>(configFile, GAMBLING_MULTIPLIERS_SECTION_KEY, CONFIG_TRIPLE_MULTIPLIER);
            configDoubleMultiplier = GetConfigFileKeyValue<int>(configFile, GAMBLING_MULTIPLIERS_SECTION_KEY, CONFIG_DOUBLE_MULTIPLIER);
            configHalveMultiplier = GetConfigFileKeyValue<float>(configFile, GAMBLING_MULTIPLIERS_SECTION_KEY, CONFIG_HALVE_MULTIPLIER);
            configZeroMultiplier = GetConfigFileKeyValue<int>(configFile, GAMBLING_MULTIPLIERS_SECTION_KEY, CONFIG_ZERO_MULTIPLIER);

            var pluginLogger = Plugin.mls;
            pluginLogger.LogInfo($"Jackpot chance value from config: {configJackpotChance}");
            pluginLogger.LogInfo($"Triple chance value from config: {configTripleChance}");
            pluginLogger.LogInfo($"Double chance value from config: {configDoubleChance}");
            pluginLogger.LogInfo($"Halve chance value from config: {configHalveChance}");
            pluginLogger.LogInfo($"Zero chance value from config: {configZeroChance}");

            pluginLogger.LogInfo($"Jackpot multiplier value from config: {configJackpotMultiplier}");
            pluginLogger.LogInfo($"Triple multiplier value from config: {configTripleMultiplier}");
            pluginLogger.LogInfo($"Double multiplier value from config: {configDoubleMultiplier}");
            pluginLogger.LogInfo($"Halve multiplier value from config: {configHalveMultiplier}");
            pluginLogger.LogInfo($"Zero multiplier value from config: {configZeroMultiplier}");
        }

        private T GetConfigFileKeyValue<T>(ConfigFile configFile, string section, string key)
        {
            ConfigDefinition configDef = new ConfigDefinition(section, key);
            ConfigEntry<T> configEntry;
            configFile.TryGetEntry(configDef, out configEntry);
            return configEntry.Value;
        }
    }
}
