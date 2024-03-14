using ItemChanger.Tags;
using ItemChanger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrassRando.Data;
using ItemChanger.UIDefs;
using UnityEngine.XR;

namespace GrassRando.IC
{
    public static class ICManager
    {
        private static readonly string[] grassShopDesc =
            {
                "It's grass.",
                "Cow food.",
                "Devil's lettuce.",
                "Not the Devil's lettuce.",
                "You already know what this is.",
                "Gr-assssss!",
                "Grawass ~_~",
                "What's this green stuff?",
                "Someone has to keep the lawn clean.",
                "I don't have a use for it.",
                "This grass doesn't taste very good.",
                "I recommend washing it before you eat it.",
                "Smoking this grass is not recommended.",
                "Not very nutritional.",
                "Not enough to break your fall.",
                "Not tall enough to hide in.",
                "This one didn't have any geo in it.",
                "This grass has a funky smell."
            };


        public static Dictionary<string, BreakableGrassLocation> grassLocations = new();

        private static readonly Random rand = new();
        private static T GetRandom<T>(this T[] array)
        {
            return array[rand.Next(0, grassShopDesc.Length)];
        }

        /// <summary>
        /// Define the items & locations we are adding
        /// </summary>
        public static void DefineItemLocs()
        {
            Finder.DefineCustomItem(CreateGrassItem());
            Finder.DefineCustomLocation(CreateGrassShop());
            foreach (var g in GrassDataRegister.gd)
            {
                var loc = CreateGrassLocation(g);
                grassLocations.Add(loc.name, loc);
                Finder.DefineCustomLocation(loc);
            }
        }

        public static GrassItem CreateGrassItem() 
        {
            return new()
            {
                name = "Grass",
                UIDef = new MsgUIDef
                {
                    name = new BoxedString("Grass"),
                    shopDesc = new BoxedString("\n" + grassShopDesc.GetRandom()),
                    sprite = new GrassSprite()
                },
                tags = new()
                    {
                        InteropTagFactory.CmiSharedTag(poolGroup: "Grass")
                    }
            };
        }

        public static GrassShopLocation CreateGrassShop()
        {
            return new()
            {
                name = "Grass_Shop", // TODO: replace with localized const
                sceneName = "Room_Slug_Shrine",
                objectName = "Quirrel Slug Shrine",
                fsmName = "npc_control",
            };
        }

        private static BreakableGrassLocation CreateGrassLocation(GrassData gd)
        {
            return new(gd.key)
            {
                name = gd.locationName,
                sceneName = gd.key.SceneName,
                //nonreplaceable = true,
                tags = new()
                    {
                        InteropTagFactory.CmiLocationTag(
                            poolGroup: gd.GetGroupName(),
                            mapLocations: new (string, float, float)[]
                            {
                                gd.mapSceneOverride ?? (gd.key.SceneName, gd.key.Position.x, gd.key.Position.y)
                            },
                            compassLocation: (gd.key.SceneName, gd.key.Position.x, gd.key.Position.y)
                        ),
                    }
            };
            
        }
    }
}
