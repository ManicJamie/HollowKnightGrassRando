﻿using RandomizerMod.Logging;
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
using RandomizerCore.Json;
using GrassRando.Settings;
using UnityEngine;
using RandomizerCore.Randomization;

namespace GrassRando.Rando
{
    internal static class RandoManager
    {
        private const string ItemName = "Grass";
        private const string ShopName = "Grass_Shop";
        private const string ShopLogic = "(Room_Slug_Shrine[left1] | Bench-Lake_of_Unn) + LISTEN?TRUE + QUIRREL?TRUE";
        // Room_Slug_Shrine logic with coalescence for Lore Rando interop

        private static BreakableLogicCostProvider? breakableCostProvider;

        public static void Hook()
        {
            RCData.RuntimeLogicOverride.Subscribe(-1000f, RegisterWaypointLogic);
            RCData.RuntimeLogicOverride.Subscribe(-500f, RegisterItemsLogic);
            RCData.RuntimeLogicOverride.Subscribe(-500f, RegisterLocationsLogic);

            RequestBuilder.OnUpdate.Subscribe(float.NegativeInfinity, SetupCostManagement);
            RequestBuilder.OnUpdate.Subscribe(-500f, SetupGrassShopRefs);
            RequestBuilder.OnUpdate.Subscribe(-100f, ApplyShopCostRandomization);
            RequestBuilder.OnUpdate.Subscribe(0f, BuildRequest);
            RequestBuilder.OnUpdate.Subscribe(100f, ApplyGrassShopConstraint);

            SettingsLog.AfterLogSettings += LogRandoSettings;

            //ModHooks.BeforeSavegameSaveHook += resetStuff;
        }

        public static void SetupCostManagement(RequestBuilder rb)
        {
            var connectionSettings = GrassRandoMod.Instance.settings;
            int count = GrassDataRegister.Filtered(connectionSettings.allowedAreas, connectionSettings.allowedAreaTypes).Count;
            breakableCostProvider = new("GRASS", (int)(0.2 * count), (int)(0.8 * count), amount => new BreakableCost(amount, BreakableType.Grass));
        }
        private static bool ConvertCosts(LogicCost lc, out Cost? cost)
        {
            if (lc is BreakableLogicCost cic)
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

            rb.EditLocationRequest(ShopName, info =>
            {
                info.getLocationDef = () => new LocationDef()
                {
                    Name = ShopName,
                    SceneName = SceneNames.Room_Slug_Shrine,
                    FlexibleCount = true,
                    AdditionalProgressionPenalty = true,
                };
            });
        }

        public static void RegisterWaypointLogic(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            if (!GrassRandoMod.Instance.settings.Enabled) { return; }

            lmb.DeserializeFile(LogicFileType.Waypoints, new JsonLogicFormat(), typeof(GrassRandoMod).Assembly.GetManifestResourceStream($"GrassRandoV2.Resources.waypoints.json"));
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

            foreach (var loc in GrassDataRegister.gd)
            {
                lmb.AddLogicDef(new(loc.locationName, loc.logic));
            }
            lmb.AddLogicDef(new(ShopName, ShopLogic));
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

            rb.EditLocationRequest(ShopName, info =>
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

        public static void ApplyGrassShopConstraint(RequestBuilder rb)
        {
            foreach (var igb in rb.EnumerateItemGroups())
            {
                if (igb.strategy is DefaultGroupPlacementStrategy dgps)
                {
                    dgps.ConstraintList.Add(new(PreventGrassInGrassShop, null, "GrassRando Grass Shop Constraint"));
                }
            }

            static bool PreventGrassInGrassShop(IRandoItem ri, IRandoLocation rl)
            {
                if (ri is GrassItem && rl is RandoModLocation rml && rml.LocationDef.Name == ShopName) 
                {
                    return false;
                }
                return true;
            }
        }

        public static void BuildRequest(RequestBuilder rb)
        {
            if (!GrassRandoMod.Instance.settings.Enabled) { return; }

            var connectionSettings = GrassRandoMod.Instance.settings;
            var locations = GrassDataRegister.gd;
            foreach (var loc in locations)
            {
                if (connectionSettings.allowedAreas.HasFlag(loc.grassArea) && connectionSettings.allowedAreaTypes.HasFlag(loc.areaType))
                {
                    rb.AddItemByName(ItemName);
                    rb.AddLocationByName(loc.locationName);
                } else
                {
                    rb.AddToVanilla(ItemName, loc.locationName);
                }
            }
            if (connectionSettings.GrassShop)
            {
                rb.AddLocationByName("Grass_Shop");
            }
        }

        public static void LogRandoSettings(LogArguments args, TextWriter w)
        {
            w.WriteLine("Logging GrassRando settings:");
            w.WriteLine(RandomizerMod.RandomizerData.JsonUtil.Serialize(GrassRandoMod.Instance.settings));
        }
    }
}
