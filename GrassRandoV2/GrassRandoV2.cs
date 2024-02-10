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
    public class SaveData
    {
        public int knightDreamGrassBroken;
        public int mageDreamGrassBroken;
    }
    public class GrassRandoV2Mod : Mod, ILocalSettings<SaveData>, IGlobalSettings<ConnectionSettings>
    {
        private static GrassRandoV2Mod? _instance;
        public static GrassRandoV2Mod Instance { get { _instance ??= new GrassRandoV2Mod(); return _instance; } }

        public SaveData sd = new();

        public readonly GrassRegister_Global grassRegister = new();

        public ConnectionSettings settings = new();

        //sets up the ui mod stuff for the mod manager
        new public string GetName() => "Grass Randomizer";
        public override string GetVersion() => "v1.1";

        public GrassRandoV2Mod() : base("GrassRandoV2")
        {
        }

        //initilizations for the mod
        public override void Initialize()
        {
            Log("Prepping the Grando");

            GrassCore.GrassCore.Instance.CutsEnabled = true; // Get grass events from GrassCore
            GrassCore.GrassCore.Instance.WeedkillerEnabled = true; // Despawn already collected grass
            GrassCore.GrassCore.Instance.DisconnectWeedKiller = true; // Do not use GrassCore's internal grass dict (we will use our own)

            GrassCore.WeedKiller.Instance.Blacklist = grassRegister._grassStates; // Use our internal tracker for WeedKiller

            // Menu pages
            RandoMenuPage.Hook();
            // RSM
            if (ModHooks.GetMod("RandoSettingsManager") is Mod) { HookRSM(); }

            // Register ItemChanger items & locations
            ICManager.DefineItemLocs();
            // Hook rando generator
            RandoManager.Hook();

            Log("Grando is ready");
        }

        //sets up the hook for the rando settings managers
        private void HookRSM()
        {
            RandoSettingsManagerMod.Instance.RegisterConnection(new SimpleSettingsProxy<ConnectionSettings>(
                this,
                (st) => settings = st ?? settings,
                () => settings.Enabled ? settings : null
            ));
        }

        public void OnLoadLocal(SaveData s)
        {
            sd = s;
            grassRegister.Clear();
        }

        public SaveData OnSaveLocal()
        {
            return sd;
        }

        public void OnLoadGlobal(ConnectionSettings s)
        {
            settings = s;
        }

        public ConnectionSettings OnSaveGlobal()
        {
            return settings ?? new();
        }

    }
}
