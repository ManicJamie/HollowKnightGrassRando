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
using GrassRando.IC;
using GrassRando.Data;
using ItemChanger.Locations;
using GrassRando.Rando.Costs;
using GrassRando.IC.Costs;
using RandomizerMod.IC;

namespace GrassRando.Rando
{
    internal static class RandoManager
    {
        private const string ItemName = "Grass";
        private const string ShopLogic = "Room_Slug_Shrine[left1] | Bench-Lake_of_Unn"; // This logic copies the Room_Slug_Shrine logic; using it directly should be preferable if possible.

        private static BreakableLogicCostProvider? breakableCostProvider;

        public static void Hook()
        {
            RCData.RuntimeLogicOverride.Subscribe(-1000f, RegisterItemsLogic);
            RCData.RuntimeLogicOverride.Subscribe(-1000f, RegisterLocationsLogic);

            RequestBuilder.OnUpdate.Subscribe(float.NegativeInfinity, SetupCostManagement);
            RequestBuilder.OnUpdate.Subscribe(-500f, SetupGrassShopRefs);
            RequestBuilder.OnUpdate.Subscribe(-100f, ApplyShopCostRandomization);
            RequestBuilder.OnUpdate.Subscribe(0f, BuildRequest);

            SettingsLog.AfterLogSettings += LogRandoSettings;

            //ModHooks.BeforeSavegameSaveHook += resetStuff;
        }

        public static void SetupCostManagement(RequestBuilder rb)
        {
            breakableCostProvider = new("GRASS", 50, 200, amount => new IC.Costs.BreakableCost(amount, BreakableType.Grass));
        }

        private static bool ConvertCosts(LogicCost lc, out Cost? cost)
        {
            if (lc is Costs.BreakableLogicCost cic)
            {
                cost = cic.GetIcCost();
                return true;
            }
            cost = default;
            return false;
        }

        public static void SetupGrassShopRefs(RequestBuilder rb)
        {
            if (!GrassRandoMod.Instance.settings.Enabled) return;

            rb.EditLocationRequest("Grass_Shop", info =>
            {
                info.getLocationDef = () => new LocationDef()
                {
                    Name = "Grass_Shop",
                    SceneName = SceneNames.Room_Slug_Shrine,
                    FlexibleCount = true,
                    AdditionalProgressionPenalty = true,
                };
            });
        }

        public static void RegisterItemsLogic(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            if (!GrassRandoMod.Instance.settings.Enabled) { return; }

            Term term = lmb.GetOrAddTerm("GRASS", TermType.Int);
            TermValue tv = new(term, 1);

            List<TermValue> terms = new() { tv };
            lmb.AddItem(new MultiItem("Grass", terms.ToArray()));
        }

        public static void RegisterLocationsLogic(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            if (!GrassRandoMod.Instance.settings.Enabled) { return; }

            var connectionSettings = GrassRandoMod.Instance.settings;
            var includedLocations = GrassDataRegister.Filtered(connectionSettings.allowedAreas, connectionSettings.allowedAreaTypes);
            foreach (var loc in includedLocations)
            {
                lmb.AddLogicDef(new(loc.locationName, loc.logic));
            }
            lmb.AddLogicDef(new("Grass_Shop", ShopLogic));
        }

        // Cost randomization taken from BadMagic100/MoreLocations under MIT
        public static void ApplyShopCostRandomization(RequestBuilder rb)
        {
            rb.CostConverters.Subscribe(0f, ConvertCosts!);
            /*
            rb.rm.OnSendEvent += (eventType, _) =>
            {
                if (eventType == RandoEventType.Initializing) Prerandomize();
            };
            */

            if (!GrassRandoMod.Instance.settings.Enabled) return;

            rb.EditLocationRequest("Grass_Shop", info =>
            {
                info.customPlacementFetch += (factory, rp) =>
                {
                    if (factory.TryFetchPlacement(rp.Location.Name, out AbstractPlacement plt))
                    {
                        return plt;
                    }
                    ShopLocation loc = (ShopLocation)factory.MakeLocation(rp.Location.Name);
                    loc.costDisplayerSelectionStrategy = new MixedCostDisplayerSelectionStrategy()
                    {
                        Capabilities =
                        {
                            new BreakableCostSupport()
                        }
                    };
                    plt = loc.Wrap();
                    factory.AddPlacement(plt);
                    return plt;
                };
                info.onRandoLocationCreation += (factory, rl) =>
                {
                    if (breakableCostProvider == null)
                    {
                        return;
                    }
                    rl.AddCost(breakableCostProvider.Next(factory.lm, factory.rng));
                };
            });
            
        }

        public static void BuildRequest(RequestBuilder rb)
        {
            if (!GrassRandoMod.Instance.settings.Enabled) { return; }

            var connectionSettings = GrassRandoMod.Instance.settings;
            var includedLocations = GrassDataRegister.Filtered(connectionSettings.allowedAreas, connectionSettings.allowedAreaTypes);
            foreach (var loc in includedLocations)
            {
                rb.AddItemByName(ItemName);
                rb.AddLocationByName(loc.locationName);
                
            }
            if (connectionSettings.GrassShop)
            {
                rb.AddLocationByName("Grass_Shop");
            }
        }

        public static void LogRandoSettings(LogArguments args, TextWriter w)
        {
            w.WriteLine("Logging RopeRando settings:");
            w.WriteLine(RandomizerMod.RandomizerData.JsonUtil.Serialize(GrassRandoMod.Instance.settings));
        }
    }
}
