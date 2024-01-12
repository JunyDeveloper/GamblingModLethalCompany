using System;
using BepInEx.Configuration;
using static GamblersMod.config.GambleConstants;

namespace GamblersMod.config
{
    [Serializable]
    public class GambleConfigSettingsSerializable
    {
        // General
        public int configMaxCooldown;
        public int configNumberOfUses;
        public int configNumberOfMachines;

        // Gambling chances
        public int configJackpotChance;
        public int configTripleChance;
        public int configDoubleChance;
        public int configHalveChance;
        public int configZeroChance;

        // Gambling multipliers
        public float configJackpotMultiplier;
        public float configTripleMultiplier;
        public float configDoubleMultiplier;
        public float configHalveMultiplier;
        public float configZeroMultiplier;

        // Audio
        public bool configGamblingMusicEnabled;
        public float configGamblingMusicVolume;

        public GambleConfigSettingsSerializable(ConfigFile configFile)
        {
            // General
            configFile.Bind(GAMBLING_GENERAL_SECTION_KEY, CONFIG_MAXCOOLDOWN, 4, "Cooldown of the machine. Reducing this will cause the drumroll sound to not sync & may also cause latency issues");
            configFile.Bind(GAMBLING_GENERAL_SECTION_KEY, CONFIG_NUMBER_OF_USES, 9999, "Number of times a gambling machine can be used");
            configFile.Bind(GAMBLING_GENERAL_SECTION_KEY, CONFIG_NUMBER_OF_MACHINES, 1, "How many gambling machines will be spawned (max 3)");

            // Chance
            configFile.Bind(GAMBLING_CHANCE_SECTION_KEY, CONFIG_JACKPOT_CHANCE_KEY, 3, "Chance to roll a jackpot. Ex. If set to 3, you have a 3% chance to get a jackpot. Make sure ALL your chance values add up to 100 or else the math won't make sense!");
            configFile.Bind(GAMBLING_CHANCE_SECTION_KEY, CONFIG_TRIPLE_CHANCE_KEY, 11, "Chance to roll a triple. Ex. If set to 11, you have a 11% chance to get a triple. Make sure ALL your chance values add up to 100 or else the math won't make sense!");
            configFile.Bind(GAMBLING_CHANCE_SECTION_KEY, CONFIG_DOUBLE_CHANCE_KEY, 27, "Chance to roll a double. Ex. If set to 27, you have a 27% chance to get a double. Make sure ALL your chance values add up to 100 or else the math won't make sense!");
            configFile.Bind(GAMBLING_CHANCE_SECTION_KEY, CONFIG_HALVE_CHANCE_KEY, 50, "Chance to roll a halve. Ex. If set to 47, you have a 47% chance to get a halve. Make sure ALL your chance values add up to 100 or else the math won't make sense!");
            configFile.Bind(GAMBLING_CHANCE_SECTION_KEY, CONFIG_ZERO_CHANCE_KEY, 9, "Chance to roll a zero. Ex. If set to 12, you have a 12% chance to get a zero. Make sure ALL your chance values add up to 100 or else the math won't make sense!");

            // Multipliers
            configFile.Bind(GAMBLING_MULTIPLIERS_SECTION_KEY, CONFIG_JACKPOT_MULTIPLIER, 10f, "Jackpot multiplier");
            configFile.Bind(GAMBLING_MULTIPLIERS_SECTION_KEY, CONFIG_TRIPLE_MULTIPLIER, 3f, "Triple multiplier");
            configFile.Bind(GAMBLING_MULTIPLIERS_SECTION_KEY, CONFIG_DOUBLE_MULTIPLIER, 2f, "Double multiplier");
            configFile.Bind(GAMBLING_MULTIPLIERS_SECTION_KEY, CONFIG_HALVE_MULTIPLIER, 0.5f, "Halve multiplier");
            configFile.Bind(GAMBLING_MULTIPLIERS_SECTION_KEY, CONFIG_ZERO_MULTIPLIER, 0f, "Zero multiplier");

            // Audio
            configFile.Bind(GAMBLING_AUDIO_SECTION_KEY, CONFIG_GAMBLING_MUSIC_ENABLED, true, "Enable gambling machine music (CLIENT SIDE)");
            configFile.Bind(GAMBLING_AUDIO_SECTION_KEY, CONFIG_GAMBLING_MUSIC_VOLUME, 0.35f, "Gambling machine music volume (CLIENT SIDE)");

            configMaxCooldown = GetConfigFileKeyValue<int>(configFile, GAMBLING_GENERAL_SECTION_KEY, CONFIG_MAXCOOLDOWN);

            configJackpotChance = GetConfigFileKeyValue<int>(configFile, GAMBLING_CHANCE_SECTION_KEY, CONFIG_JACKPOT_CHANCE_KEY);
            configTripleChance = GetConfigFileKeyValue<int>(configFile, GAMBLING_CHANCE_SECTION_KEY, CONFIG_TRIPLE_CHANCE_KEY);
            configDoubleChance = GetConfigFileKeyValue<int>(configFile, GAMBLING_CHANCE_SECTION_KEY, CONFIG_DOUBLE_CHANCE_KEY);
            configHalveChance = GetConfigFileKeyValue<int>(configFile, GAMBLING_CHANCE_SECTION_KEY, CONFIG_HALVE_CHANCE_KEY);
            configZeroChance = GetConfigFileKeyValue<int>(configFile, GAMBLING_CHANCE_SECTION_KEY, CONFIG_ZERO_CHANCE_KEY);

            configJackpotMultiplier = GetConfigFileKeyValue<float>(configFile, GAMBLING_MULTIPLIERS_SECTION_KEY, CONFIG_JACKPOT_MULTIPLIER);
            configTripleMultiplier = GetConfigFileKeyValue<float>(configFile, GAMBLING_MULTIPLIERS_SECTION_KEY, CONFIG_TRIPLE_MULTIPLIER);
            configDoubleMultiplier = GetConfigFileKeyValue<float>(configFile, GAMBLING_MULTIPLIERS_SECTION_KEY, CONFIG_DOUBLE_MULTIPLIER);
            configHalveMultiplier = GetConfigFileKeyValue<float>(configFile, GAMBLING_MULTIPLIERS_SECTION_KEY, CONFIG_HALVE_MULTIPLIER);
            configZeroMultiplier = GetConfigFileKeyValue<float>(configFile, GAMBLING_MULTIPLIERS_SECTION_KEY, CONFIG_ZERO_MULTIPLIER);

            configGamblingMusicEnabled = GetConfigFileKeyValue<bool>(configFile, GAMBLING_AUDIO_SECTION_KEY, CONFIG_GAMBLING_MUSIC_ENABLED);
            configGamblingMusicVolume = GetConfigFileKeyValue<float>(configFile, GAMBLING_AUDIO_SECTION_KEY, CONFIG_GAMBLING_MUSIC_VOLUME);

            configNumberOfUses = GetConfigFileKeyValue<int>(configFile, GAMBLING_GENERAL_SECTION_KEY, CONFIG_NUMBER_OF_USES);
            configNumberOfMachines = GetConfigFileKeyValue<int>(configFile, GAMBLING_GENERAL_SECTION_KEY, CONFIG_NUMBER_OF_MACHINES);

            LogInitializedConfigsValues();
        }

        private void LogInitializedConfigsValues()
        {
            var pluginLogger = Plugin.mls;
            pluginLogger.LogInfo($"Cooldown value from config: {configMaxCooldown}");

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

            pluginLogger.LogInfo($"Music enabled from config: {configGamblingMusicEnabled}");
            pluginLogger.LogInfo($"Music volume from config: {configGamblingMusicVolume}");

            pluginLogger.LogInfo($"Number of uses from config: {configNumberOfUses}");
            pluginLogger.LogInfo($"Number of machines from config: {configNumberOfMachines}");
        }

        private T GetConfigFileKeyValue<T>(ConfigFile configFile, string section, string key)
        {
            ConfigDefinition configDef = new ConfigDefinition(section, key);
            ConfigEntry<T> configEntry;
            Plugin.mls.LogInfo($"Getting configuration entry: Section: {section} Key: {key}");
            bool configExists = configFile.TryGetEntry(configDef, out configEntry);
            if (!configExists) Plugin.mls.LogError($"Failed to get configuration value. Section: {section} Key: {key}");
            return configEntry.Value;
        }
    }
}
