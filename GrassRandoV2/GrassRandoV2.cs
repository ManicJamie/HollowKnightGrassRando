using Modding;
using System;
using System.Collections.Generic;
using GrassRando.IC;
using GrassRando.Rando;
using RandoSettingsManager;
using RandoSettingsManager.SettingsManagement;
using System.Reflection;
using GrassCore;
using GrassRando.Data;
using GrassRando.Settings;

namespace GrassRando
{
    public class GrassRandoMod : Mod, ILocalSettings<SaveData>, IGlobalSettings<ConnectionSettings>
    {
        private static GrassRandoMod? _instance;
        public static GrassRandoMod Instance { get { _instance ??= new GrassRandoMod(); return _instance; } }

        public SaveData saveData = new();

        public readonly GrassRegister_Global grassRegister = new();

        public ConnectionSettings settings = new();

        //sets up the ui mod stuff for the mod manager
        new public string GetName() => "Grass Randomizer";
        public override string GetVersion() => GetType().Assembly.GetName().Version.ToString();

        public GrassRandoMod() : base("GrassRando")
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
                    if (st is null) 
                    {
                        settings.Enabled = false;
                    }
                    else
                    {
                        settings = st;
                    }
                    RandoMenuPage.Instance?.PasteSettings();
                },
                () => {
                    return settings.Enabled ? settings : null;
                }
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
