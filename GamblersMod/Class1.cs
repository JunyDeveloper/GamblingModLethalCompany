using BepInEx;
using BepInEx.Logging;
using GamblersMod.Patches;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GamblersMod
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class GamblersModBase : BaseUnityPlugin
    {
        private const string modGUID = "Junypai.GamblersMod";
        private const string modName = "Gamblers Mod";
        private const string modVersion = "1.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        private static GamblersModBase Instance;

        public static GameObject GamblingMachine;

        internal ManualLogSource mls;

        void Awake() {
            if (Instance == null) {
                Instance = this;
            }
            // xyz
            // -26.3777 - 1.3256 - 23.8357
            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            mls.LogInfo("Test mod works");
            mls.LogInfo($"this.Info.Location: {this.Info.Location}");
            mls.LogInfo($"Path.GetDirectoryName(this.Info.Location)h: {Path.GetDirectoryName(this.Info.Location)}");

            var DLLDirectoryName = Path.GetDirectoryName(this.Info.Location);

            // example/path/testbundle
            AssetBundle gamblersBundle = AssetBundle.LoadFromFile(Path.Combine(DLLDirectoryName, "loop"));
            mls.LogInfo($"gamblersBundle: {gamblersBundle}");

            GameObject gamblersAudio = gamblersBundle.LoadAsset<GameObject>("a.prefab");
            GameObject gamblingMachinePrefab = gamblersBundle.LoadAsset<GameObject>("Snowman/Prefabs/Snowman_01.prefab");
            GameObject hah = gamblersBundle.LoadAsset<GameObject>("Snowman_03 1"); // I guess even tho it's nested I dont need to specify folder structure
            hah.AddComponent<GamblingMachine>();

            mls.LogInfo($"mainTexture??? {hah.transform.GetChild(0).GetComponent<Renderer>().sharedMaterial.mainTexture}");
            mls.LogInfo($"gamblingMachinePrefab V2: {hah}");
            GamblingMachine = hah;
            // AudioSource gambelersAudioSource = gamblersAudio.GetComponent<AudioSource>();
            //mls.LogInfo($"gamblersAudio: {gamblersAudio}");
            //mls.LogInfo($"gambelersAudioSource: {gamblersAudio.GetComponent<AudioSource>()}");
            //mls.LogInfo($"gamblingMachinePrefab: {gamblingMachinePrefab}");

            if (gamblersAudio == null ) {
                mls.LogError("Failed to load gamblersAudio");
            }

            harmony.PatchAll(typeof(GamblersModBase));
            harmony.PatchAll(typeof(PlayerControllerBPatch));
            harmony.PatchAll(typeof(RoundManagerPatch));
        }

       // [HarmonyPatch(typeof())]
       // void logPositionCoroutine() {
        //    mls.LogInfo("");
      //  }

        //IEnumerator LogPosition() {
        //    
        //}

        /**
         * If the current moon is The Company, render a cube at X,Y,Z position
         * GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
         * cube.transform.position = new Vector3(0, 0.5f, 0);
         * 
         * Logger to log out X,Y,Z Of player so I know where to put it
        // Company moon
        if (currentLevel.levelID == 3) {
                
        }
        */
    }

   
    
}
