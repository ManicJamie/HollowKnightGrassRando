﻿using System;
using System.Collections.Generic;
using System.Linq;
using ItemChanger;
using Newtonsoft.Json;
using RandomizerCore.Logic;
using RandomizerCore.LogicItems;
using RandomizerMod.RC;
using RandomizerMod.Settings;
using RandomizerCore;
using System.Text.RegularExpressions;
using ItemChanger.UIDefs;
using RandomizerMod.Menu;
using Modding;
using RandomizerMod.Logging;
using UnityEngine;
using System.IO;
using System.Runtime.InteropServices;
using ItemChanger.Util;
using System.Linq;
using ItemChanger.Locations;
using ItemChanger;
using Satchel;
using ItemChanger.Util;
using RandomizerMod.IC;
using System.Drawing.Drawing2D;
using GrassRandoV2.Rando;

namespace GrassRandoV2.IC
{
    public class ICManager
    {
        #pragma warning disable 0649
        public class grassdata
        {
            public class RelativeLocation
            {
                public string sceneName;
                public float x;
                public float y;

                [Newtonsoft.Json.JsonIgnore]
                public (string, float, float) repr => (sceneName, x, y);
            }

            Regex rgx = new Regex("[^a-zA-Z0-9_]");
            Regex rgx_with_spaces = new Regex("[^a-zA-Z0-9_ ]");

            public string gameObj;
            public string fsmType;
            public Dictionary<string, string> logicOvr;
            public Dictionary<string, Dictionary<string, string>> logicSubs;
            public string sceneName;
            public string usrName;
            public string logic;
            public string persistantBool;
            public string requiredSetting;
            public string sprite;
            public List<RelativeLocation> locations;
            public List<string> alsoDestroy;
            public string ovrTermName;
            public int id;

            public string internalName = null;

            public string getItemName() => internalName != "" ? rgx_with_spaces.Replace(internalName, "") : $"Loc_Grass_{cleanSceneName()}_{cleanGameObjectPath()}";
            public string cleanGameObjectPath() => rgx.Replace(gameObj, "");
            public string cleanSceneName() => rgx.Replace(sceneName, "");
            public string getLocationName() => internalName != "" ? rgx_with_spaces.Replace(internalName, "") : $"Loc_Grass_{cleanSceneName()}_{cleanGameObjectPath()}";
            public string getTermName() => ovrTermName ?? $"GRASS_{cleanSceneName()}_{cleanGameObjectPath()}";
            public string getGroupName() => GRASS_GROUPS[fsmType].Item1;

            public bool shouldBeIncluded(GenerationSettings gs)
            {
                if (!GrassRandoV2Mod.settings.anygrass) { return false; }

                if ((fsmType == "FSM" || fsmType == "default grass") && !GrassRandoV2Mod.settings.randomizeGrass) { return false; }
                if((fsmType == "FSM" || fsmType == "quantum grass") && !GrassRandoV2Mod.settings.randomizeQuantumGrass) { return false; }
                if((sceneName == "RestingGrounds_04") && !GrassRandoV2Mod.settings.randomizeDreamNailGrass) { return false; }

                if((sceneName == "Tutorial_01" || sceneName == "Town" || sceneName == "Room_Town_Stag_Station") && !GrassRandoV2Mod.settings.kingsPassAndDM) { return false; }
                if((sceneName.Contains("Crossroads_")) && !GrassRandoV2Mod.settings.crossroads) { return false; }
                if((sceneName.Contains("Deepnest_East")) && !GrassRandoV2Mod.settings.edge) { return false; }
                if((sceneName.Contains("RestingGrounds") && !sceneName.Contains("RestingGrounds_04")) && !GrassRandoV2Mod.settings.resting) { return false; }
                if((sceneName.Contains("Fungus2")) && !GrassRandoV2Mod.settings.fungal) { return false; }
                if((sceneName.Contains("Fungus3")) && !GrassRandoV2Mod.settings.fogCan) { return false; }
                if((sceneName.Contains("Fungus1")) && !GrassRandoV2Mod.settings.greenpath) { return false; }
                if((sceneName.Contains("Deepnest") && !sceneName.Contains("Deepnest_East")) && !GrassRandoV2Mod.settings.deepNest) { return false; }
                if((sceneName.Contains("Deepnest") && !sceneName.Contains("Abyss")) && !GrassRandoV2Mod.settings.abyssAndBasin) { return false; }



                return true;
            } 

            public void initInternalName()
            {
                this.internalName = usrName + "(" + (id - 1).ToString() + ")";
            }

        }
        #pragma warning restore 0649

        //in game shop descriptions for the grass
        readonly string[] grassShopDesc =
            {
                "It's grass.",
                "Cow food.",
                "Devil's lettuce.",
                "Not the Devil's lettuce.",
                "You already know what this is.",
                "Gr-assssss!",
                "Grawass ~_~",
                "What's this green stuff?"
            };

        //groups for the two types of grass
        public readonly static Dictionary<string, (string, Func<bool>)> GRASS_GROUPS = new()
        {
            {"default grass", ("Regular Grass", ()=>GrassRandoV2Mod.settings.randomizeGrass) },
            {"quantum grass", ("Quantum Grass", ()=>GrassRandoV2Mod.settings.randomizeQuantumGrass) },
        };

        private static Dictionary<string, ItemGroupBuilder> definedGroups = new();


        public readonly static List<grassdata> gd = JsonConvert.DeserializeObject<List<grassdata>>(File.ReadAllText(Properties.Resource1.DestGrassData).ToString());

        public static List<BreakableGrassLocation> bgl = new();


        //adds items and locations to the rando

        public void RegisterItemsAndLocations()
        {
            System.Random rand = new(0x6969);

            //iterates through all of the grass data objects
            foreach (var grass in gd)
            {
                //sets up the internal name for the object,
                //i havent found a better time for this
                //they each need a seperate inernal name b/c theres so many
                //per screen
                grass.initInternalName();

                //sets up the location defenition for the item changer mod
                BreakableGrassLocation GSL = new()
                {
                    objectName = grass.gameObj,
                    fsmType = grass.fsmType,
                    name = grass.getLocationName(),
                    sceneName = grass.sceneName,
                    gd = grass,
                    //nonreplaceable = true,
                    tags = new()
                    {
                        InteropTagFactory.CmiLocationTag(
                            poolGroup: grass.getGroupName(),
                            sceneNames: new List<string> { grass.sceneName },
                            mapLocations: grass.locations.Select(x => x.repr).ToArray()
                        ),
                    }
                };

               bgl.Add(GSL);

                //sets up the item deferninition for the item changer mod
                BreakableGrassItem grassItem = new()
                {
                    objectName = grass.gameObj,
                    sceneName = grass.sceneName,
                    name = grass.getItemName(),
                    gd = grass,
                    UIDef = new MsgUIDef
                    {
                        name = new BoxedString("Grass"),
                        shopDesc = new BoxedString("\n" + grassShopDesc[rand.Next(0, grassShopDesc.Length)]),
                        sprite = new GrassSprite(grass.sprite)
                    },
                    tags = new()
                    {
                        InteropTagFactory.CmiSharedTag(poolGroup: grass.getGroupName())
                    }
                };

                //exception handler shouldn't be needed but...
                //it's here incase i fuck up the implementation of an item/location in future
                try
                {

                    Finder.DefineCustomLocation(GSL);
                    Finder.DefineCustomItem(grassItem);
                }
                catch (Exception ex)
                {
                    //Loggable.Log(ex.ToString());
                    //Modding.Logger.LogError(ex);
                    Modding.Logger.Log(GSL.gd.getLocationName());
                }


            }




        }

        public void AddGrassShop(RequestBuilder rb) 
        {
            rb.EditLocationRequest("GrassShop", info =>
            {
                info.getLocationDef = () => new()
                {
                    Name = "GrassShop",
                    SceneName = "Tutorial_0",
                    FlexibleCount = true,
                    AdditionalProgressionPenalty = true
                };
            });
        }


        //adds the grass to the item and location list
        private void AddGrass(RequestBuilder rb)
        {

            foreach(var grass in gd)
            {
                rb.EditItemRequest(grass.getItemName(), info =>
                {
                    info.getItemDef = () => new()
                    {
                        Name = grass.getItemName(),
                        Pool = grass.getGroupName(),
                        MajorItem = false,
                        PriceCap = 200
                    };
                });

                rb.EditLocationRequest(grass.getLocationName(), info =>
                {
                    info.getLocationDef = () => new()
                    {
                        Name = grass.getLocationName(),
                        SceneName = grass.sceneName,
                        FlexibleCount = false,
                        AdditionalProgressionPenalty = false
                    };
                });

            }
           
            foreach(var grass in gd)
            {
                if (grass.shouldBeIncluded(rb.gs))
                {
                    rb.AddItemByName(grass.getItemName());
                    rb.AddLocationByName(grass.getLocationName());
                }
                else if (GrassRandoV2Mod.settings.anygrass)
                {
                    rb.AddToVanilla(new(grass.getItemName(), grass.getLocationName()));
                }
            }
            /*
            if (GrassRandoV2Mod.st.addShop)
            {
                System.Random rand = new(rb.gs.Seed);
                rb.AddLocationByName(Grass_Shop);
                rb.EditLocationRequest(Grass_Shop, info =>
                {
                    info.getLocationDef() = () => new()
                    {
                        FlexibleCouunt = true,
                        Name = Grass_Shop,
                        AdditionalProgressionPenalty = rb.gs.ProgressionDepthSettings.MultiLocationPenalty
                    };
                    info.onRandoLocationCreation += (factory, location) =>
                    {
                        Term term = rb.lm.GetTerm("Grass");
                        int maxGrassCost = 0;
                        if (GrassRandoV2Mod.st.randomizeGrass)
                        {
                            maxGrassCost += gd.Count;
                        }
                        location.AddCost(new SimpleCost(term, rand.Next(1, maxGrassCost)));
                    };
                });
                rb.CostConverters.Subscribe(250f, GetCost);
            }*/

        }

        /*
        private static bool GetCost(LogicCost lc, out Cost cost)
        {
            if (lc.GetTerms()?.Any(x => x.Name == "LORE") == true && lc is SimpleCost simpleCost)
            {
                cost = new GrassCost()
                {
                    NeededGrass = simpleCost.threshold
                };
                return true;
            }
            else
                cost = default;
            return false;
        }*/



        //adds the grass logic and items via a hook
        public void Hook()
        {
            RandoController.OnCalculateHash += RandoController_OnCalculateHash;

            RCData.RuntimeLogicOverride.Subscribe(15f, ApplyLogic);

            RequestBuilder.OnUpdate.Subscribe(200f, AddGrass);
            //RequestBuilder.OnUpdate.Subscribe(100f, AddGrassShop);

            SettingsLog.AfterLogSettings += LogGrassRandoSettings;



        }

        private int RandoController_OnCalculateHash(RandoController arg1, int arg2)
        {
            int hash = 0;
            if(!GrassRandoV2Mod.settings.anygrass) { return hash; }
            if(GrassRandoV2Mod.settings.randomizeGrass) { hash += gd.Count; }

            return hash;

        }

        //logger for the grass rando settings
        private static void LogGrassRandoSettings(LogArguments args, System.IO.TextWriter tw)
        {
            tw.WriteLine("Logging Grass Rando settings:");
            using Newtonsoft.Json.JsonTextWriter jtw = new(tw) { CloseOutput = false, };
            RandomizerMod.RandomizerData.JsonUtil._js.Serialize(jtw, GrassRandoV2Mod.settings);
            tw.WriteLine();
        }

        //adds the grass logic to the rando
        private void ApplyLogic(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            if (GrassRandoV2Mod.settings.addShop)
            {
                lmb.AddLogicDef(new("Grass_Shop", "Town"));
            }
            foreach (var grass in gd)
            {
                if (!GrassRandoV2Mod.settings.anygrass) { continue; }

                Term grassTerm = lmb.GetOrAddTerm(grass.getTermName());
                lmb.AddItem(new SingleItem(grass.getItemName(), new TermValue(grassTerm, 1)));

                lmb.AddLogicDef(new(grass.getLocationName(), grass.logic));

                if(!grass.shouldBeIncluded(gs)) { continue; }

                foreach(var lovr in grass.logicOvr)
                {
                    lmb.DoLogicEdit(new(lovr.Key, lovr.Value));
                }

                foreach (var subDef in grass.logicSubs)
                {
                    foreach(var sub in subDef.Value)
                    {
                        lmb.DoSubst(new(subDef.Key, sub.Key, sub.Value));
                    }
                }

            }
        }


    }
}
