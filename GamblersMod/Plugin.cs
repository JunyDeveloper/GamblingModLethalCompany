using System.IO;
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

        // public static GameObject GamblingJackpotText;
        //public static GameObject GamblingTripleText;
        // public static GameObject GamblingDoubleText;
        // public static GameObject GamblingHalveText;
        //  public static GameObject GamblingRemoveText;

        public static GambleConfigSettingsSerializable UserConfig;

        public static ManualLogSource mls;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            UserConfig = new GambleConfigSettingsSerializable(Config);

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
            //GamblingDoubleScrapAudio = LoadAssetFromAssetBundleAndLogInfo<AudioClip>(gamblersBundle, "coindrop");
            //GamblingTripleScrapAudio = LoadAssetFromAssetBundleAndLogInfo<AudioClip>(gamblersBundle, "kaching");
            GamblingJackpotScrapAudio = LoadAssetFromAssetBundleAndLogInfo<AudioClip>(gamblersBundle, "holyshit");
            GamblingHalveScrapAudio = LoadAssetFromAssetBundleAndLogInfo<AudioClip>(gamblersBundle, "cricket");
            GamblingRemoveScrapAudio = LoadAssetFromAssetBundleAndLogInfo<AudioClip>(gamblersBundle, "womp");
            GamblingMachineMusicAudio = LoadAssetFromAssetBundleAndLogInfo<AudioClip>(gamblersBundle, "machineMusic");
            GamblingDoubleScrapAudio = LoadAssetFromAssetBundleAndLogInfo<AudioClip>(gamblersBundle, "doublekill");
            GamblingTripleScrapAudio = LoadAssetFromAssetBundleAndLogInfo<AudioClip>(gamblersBundle, "triplekill");
            GamblingMachineResultCanvas = LoadAssetFromAssetBundleAndLogInfo<GameObject>(gamblersBundle, "GamblingMachineResultCanvas"); // ?
            GamblingMachine = LoadAssetFromAssetBundleAndLogInfo<GameObject>(gamblersBundle, "GamblingMachine");

            // Attach the gambling machine script to the gambling machine game object
            GamblingMachine.AddComponent<GamblingMachine>();

            // GamblingJackpotText = LoadAssetFromAssetBundleAndLogInfo<GameObject>(gamblersBundle, "JackpotText");
            // GamblingTripleText = LoadAssetFromAssetBundleAndLogInfo<GameObject>(gamblersBundle, "TripleText");
            // GamblingDoubleText = LoadAssetFromAssetBundleAndLogInfo<GameObject>(gamblersBundle, "DoubleText");
            // GamblingHalveText = LoadAssetFromAssetBundleAndLogInfo<GameObject>(gamblersBundle, "HalveText");
            // GamblingRemoveText = LoadAssetFromAssetBundleAndLogInfo<GameObject>(gamblersBundle, "RemoveText");
            // GameObject gamblingMachine = LoadAssetFromAssetBundleAndLogInfo<GameObject>(gamblersBundle, "Snowman_03 1"); ; // I guess even tho it's nested I dont need to specify folder structure

            harmony.PatchAll(typeof(Plugin));
            harmony.PatchAll(typeof(PlayerControllerBPatch));
            harmony.PatchAll(typeof(RoundManagerPatch));
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
    }



}
