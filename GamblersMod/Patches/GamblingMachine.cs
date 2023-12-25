using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BepInEx;
using BepInEx.Logging;
using System.Security.Policy;

namespace GamblersMod.Patches
{
    internal class GamblingMachine : MonoBehaviour
    {
        internal ManualLogSource mls;

        void Awake() {
            mls = BepInEx.Logging.Logger.CreateLogSource("GamblingMachine");
            mls.LogInfo("Gambling machine has awoken");
        }

        void Start() {
           // tag = "InteractTrigger";
           // gameObject.layer = 8; // This layer enables tooltip for SetHoverTipAndCurrentInteractTrigger() (interactable?)
            
        }
    }
}
