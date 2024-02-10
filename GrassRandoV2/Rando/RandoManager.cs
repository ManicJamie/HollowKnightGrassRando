using RandomizerMod.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RandomizerMod.RandomizerData;
using RandomizerCore.Logic;
using RandomizerCore.LogicItems;
using RandomizerMod.RC;
using RandomizerMod.Settings;
using ItemChanger;
using RandomizerCore;
using GrassRandoV2.IC;
using GrassRandoV2.Data;

namespace GrassRandoV2.Rando
{
    internal static class RandoManager
    {
        private const string ItemName = "Grass";


        public static void Hook()
        {
            RCData.RuntimeLogicOverride.Subscribe(200f, RegisterItemsLogic);
            RCData.RuntimeLogicOverride.Subscribe(200f, RegisterLocationsLogic);

            RequestBuilder.OnUpdate.Subscribe(200f, BuildRequest);

            SettingsLog.AfterLogSettings += LogRandoSettings;

            //ModHooks.BeforeSavegameSaveHook += resetStuff;
        }

        public static void RegisterItemsLogic(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            if (!GrassRandoV2Mod.Instance.settings.Enabled) { return; }

            Term term = lmb.GetOrAddTerm("GRASS", TermType.Int);
            TermValue tv = new(term, 1);

            List<TermValue> terms = new() { tv };
            lmb.AddItem(new MultiItem("Grass", terms.ToArray()));
        }

        public static void RegisterLocationsLogic(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            var connectionSettings = GrassRandoV2Mod.Instance.settings;
            var includedLocations = GrassDataRegister.Filtered(connectionSettings.allowedAreas, connectionSettings.allowedAreaTypes);
            foreach (var loc in includedLocations)
            {
                lmb.AddLogicDef(new(loc.locationName, loc.logic));
            }
        }

        public static void BuildRequest(RequestBuilder rb)
        {
            if (!GrassRandoV2Mod.Instance.settings.Enabled) { return; }

            var connectionSettings = GrassRandoV2Mod.Instance.settings;
            var includedLocations = GrassDataRegister.Filtered(connectionSettings.allowedAreas, connectionSettings.allowedAreaTypes);
            foreach (var loc in includedLocations)
            {
                rb.AddItemByName(ItemName);
                rb.AddLocationByName(loc.locationName);
            }
        }

        public static void LogRandoSettings(LogArguments args, TextWriter w)
        {
            w.WriteLine("Logging RopeRando settings:");
            w.WriteLine(RandomizerMod.RandomizerData.JsonUtil.Serialize(GrassRandoV2Mod.Instance.settings));
        }
    }
}
