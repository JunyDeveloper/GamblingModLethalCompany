using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using GamblersMod.config;
using GamblersMod.Patches;
using HarmonyLib;
using UnityEngine;

namespace GamblersMod
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class Plugin : BaseUnityPlugin
    {
        public const string modGUID = "Junypai.GamblersMod";
        public const string modName = "Gamblers Mod";
        public const string modVersion = "1.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        public static Plugin Instance;

        public static GameObject GamblingMachine;
        public static AudioClip GamblingJackpotScrapAudio;
        public static AudioClip GamblingHalveScrapAudio;
        public static AudioClip GamblingRemoveScrapAudio;
        public static AudioClip GamblingDoubleScrapAudio;
        public static AudioClip GamblingTripleScrapAudio;
        public static AudioClip GamblingDrumrollScrapAudio;
        public static GameObject GamblingATMMachine;
        public static AudioClip GamblingMachineMusicAudio;
        public static GameObject GamblingMachineResultCanvas;
        public static Font GamblingFont;
        public static GameObject GamblingHandIcon;

        // Configuration state
        public static GambleConfigSettingsSerializable UserConfigSnapshot; // Snapshot of the user configuration set in their CFG file
        public static GambleConfigSettingsSerializable RecentHostConfig; // This is the most recent host configuration that we pulled
        public static GambleConfigSettingsSerializable CurrentUserConfig; // Current user config state

        public static ManualLogSource mls;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            NetcodeWeaver();

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            CurrentUserConfig = new GambleConfigSettingsSerializable(Config);
            RecentHostConfig = new GambleConfigSettingsSerializable(Config);
            UserConfigSnapshot = new GambleConfigSettingsSerializable(Config);

            var DLLDirectoryName = Path.GetDirectoryName(this.Info.Location);

            mls.LogInfo($"Loading gambler bundle assets");
            // example/path/testbundle
            AssetBundle gamblersBundle = AssetBundle.LoadFromFile(Path.Combine(DLLDirectoryName, "gamblingmachinebundle"));

            if (!gamblersBundle)
            {
                mls.LogError("Unable to load gambler bundle assets");
            }
            else
            {
                mls.LogInfo("Gamblers bundle assets successfully loaded");
            }

            GamblingDrumrollScrapAudio = LoadAssetFromAssetBundleAndLogInfo<AudioClip>(gamblersBundle, "drumroll");
            GamblingJackpotScrapAudio = LoadAssetFromAssetBundleAndLogInfo<AudioClip>(gamblersBundle, "holyshit");
            GamblingHalveScrapAudio = LoadAssetFromAssetBundleAndLogInfo<AudioClip>(gamblersBundle, "cricket");
            GamblingRemoveScrapAudio = LoadAssetFromAssetBundleAndLogInfo<AudioClip>(gamblersBundle, "womp");
            GamblingMachineMusicAudio = LoadAssetFromAssetBundleAndLogInfo<AudioClip>(gamblersBundle, "machineMusic");
            GamblingDoubleScrapAudio = LoadAssetFromAssetBundleAndLogInfo<AudioClip>(gamblersBundle, "doublekill");
            GamblingTripleScrapAudio = LoadAssetFromAssetBundleAndLogInfo<AudioClip>(gamblersBundle, "triplekill");
            GamblingFont = LoadAssetFromAssetBundleAndLogInfo<Font>(gamblersBundle, "3270-Regular");
            GamblingMachine = LoadAssetFromAssetBundleAndLogInfo<GameObject>(gamblersBundle, "GamblingMachine");
            GamblingHandIcon = LoadAssetFromAssetBundleAndLogInfo<GameObject>(gamblersBundle, "HandIconGO");

            // Attach the gambling machine script to the gambling machine game object
            GamblingMachine.AddComponent<GamblingMachine>();

            new GameObject().AddComponent<GamblingMachineManager>();

            harmony.PatchAll(typeof(Plugin));
            harmony.PatchAll(typeof(GameNetworkManagerPatch));
            harmony.PatchAll(typeof(PlayerControllerBPatch));
            harmony.PatchAll(typeof(RoundManagerPatch));
            harmony.PatchAll(typeof(StartOfRoundPatch));
        }

        private T LoadAssetFromAssetBundleAndLogInfo<T>(AssetBundle bundle, string assetName) where T : UnityEngine.Object
        {
            var loadedAsset = bundle.LoadAsset<T>(assetName);

            if (!loadedAsset)
            {
                mls.LogError($"{assetName} asset failed to load");
            }
            else
            {
                mls.LogInfo($"{assetName} asset successfully loaded");
            }

            return loadedAsset;
        }

        private static void NetcodeWeaver()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in types)
            {
                var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                foreach (var method in methods)
                {
                    var attributes = method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                    if (attributes.Length > 0)
                    {
                        method.Invoke(null, null);
                    }
                }
            }
        }
    }
}
