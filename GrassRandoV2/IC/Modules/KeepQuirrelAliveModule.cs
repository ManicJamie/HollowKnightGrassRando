using ItemChanger;
using ItemChanger.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine;
using ItemChanger.Extensions;
using HutongGames.PlayMaker.Actions;

namespace GrassRandoV2.IC.Modules
{
    /// <summary>
    /// Prevents Quirrel from leaving Unn's shrine so we can use him as a shop.
    /// </summary>
    public class KeepQuirrelAliveModule : Module
    {
        private const string sceneName = "Room_Slug_Shring";
        private const string goName = "Quirrel Slug Shrine";
        private const string deactivate1 = "deactivate";
        private const string deactivate2 = "FSM";

        //TODO: Check IC for Grass Shop before hooking
        public override void Initialize()
        {
            GrassRandoV2Mod.Instance.LogDebug("Loading KeepQuirrelAliveModule");
            Events.AddFsmEdit(new FsmID(goName, deactivate1), RemoveFSM);
            Events.AddFsmEdit(new FsmID(goName, deactivate2), RemoveFSM);
        }

        public override void Unload()
        {
            GrassRandoV2Mod.Instance.LogDebug("Unloading KeepQuirrelAliveModule");
            Events.RemoveFsmEdit(new FsmID(goName, deactivate1), RemoveFSM);
            Events.RemoveFsmEdit(new FsmID(goName, deactivate2), RemoveFSM);
        }

        private void RemoveFSM(PlayMakerFSM fsm)
        {
            GrassRandoV2Mod.Instance.LogDebug("Editing Quirrel death FSMs");
            var state = fsm.GetState("Destroy");
            state.RemoveFirstActionOfType<DestroySelf>();
            state.RemoveFirstActionOfType<ActivateGameObject>();
        }
    }
}
