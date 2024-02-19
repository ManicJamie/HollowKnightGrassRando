using Modding;
using System;
using System.Collections.Generic;
using GrassRandoV2.IC;
using GrassRandoV2.Rando;
using RandoSettingsManager;
using RandoSettingsManager.SettingsManagement;
using System.Reflection;
using GrassCore;
using GrassRandoV2.Data;
using GrassRandoV2.Settings;

namespace GrassRandoV2
{
    public class GrassRandoV2Mod : Mod, ILocalSettings<SaveData>, IGlobalSettings<ConnectionSettings>
    {
        private static GrassRandoV2Mod? _instance;
        public static GrassRandoV2Mod Instance { get { _instance ??= new GrassRandoV2Mod(); return _instance; } }

        public SaveData saveData = new();

        public readonly GrassRegister_Global grassRegister = new();

        public ConnectionSettings settings = new();

        //sets up the ui mod stuff for the mod manager
        new public string GetName() => "Grass Randomizer";
        public override string GetVersion() => "v1.1";

        public GrassRandoV2Mod() : base("GrassRando")
        {
        }

        public override void Initialize()
        {
            Log("Initializing");

            GrassCoreMod.Instance.CutsEnabled = true; // Get grass events from GrassCore
            // GrassCore contains WeedKiller, but its easier to just do it ourselves in a module.

            // Menu pages
            RandoMenuPage.Hook();
            // RSM
            if (ModHooks.GetMod("RandoSettingsManager") is Mod) { HookRSM(); }

            // Register ItemChanger items & locations
            ICManager.DefineItemLocs();
            // Hook rando generator
            RandoManager.Hook();

            Log("Initialized");
        }

        private void HookRSM()
        {
            RandoSettingsManagerMod.Instance.RegisterConnection(new SimpleSettingsProxy<ConnectionSettings>(
                this,
                (st) => {
                    if (st == null) 
                    {
                        settings.Enabled = false;
                    }
                    else
                    {
                        settings = st;
                    }
                },
                () => settings.Enabled ? settings : null
            ));
        }

        // Save management
        public void OnLoadLocal(SaveData s) => saveData = s;
        public SaveData OnSaveLocal() => saveData ?? new();

        // Settings management
        public void OnLoadGlobal(ConnectionSettings s) => settings = s;
        public ConnectionSettings OnSaveGlobal() => settings ?? new();

    }
}
