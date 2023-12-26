using BepInEx.Logging;
using UnityEngine;

namespace GamblersMod.Patches
{
    internal class GamblingMachine : MonoBehaviour
    {
        internal ManualLogSource mls;

        void Awake()
        {
            mls = BepInEx.Logging.Logger.CreateLogSource(Plugin.modGUID);
            mls.LogInfo("Gambling machine has Awoken");
        }

        void Start()
        {
            mls.LogInfo("Gambling machine has Started");
        }
    }
}
