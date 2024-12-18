using GrassCore;
using GrassRando.IC;
using GrassRando.Rando;
using RandomizerCore.Logic;
using RandomizerMod.RandomizerData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrassRando.Data
{
    public static class GrassDataRegister
    {
        public readonly static List<GrassData> gd = JsonUtil.Deserialize<List<GrassData>>("GrassRandoV2.Resources.Locations.json");
        public readonly static List<RawWaypointDef> waypoints = JsonUtil.Deserialize<List<RawWaypointDef>>("GrassRandoV2.Resources.waypoints.json");

        public readonly static Dictionary<GrassKey, GrassData> dict = new();

        static GrassDataRegister()
        {
            foreach (var g in gd)
            {
                dict[g.key] = g; 
            }
        }

        public static List<GrassData> Filtered(GrassArea includedAreas, AreaType includedSubAreas)
        {
            List<GrassData> output = new();
            foreach (var g in gd)
            {
                if (includedAreas.HasFlag(g.grassArea) && includedSubAreas.HasFlag(g.areaType))
                {
                    output.Add(g);
                }
            }
            return output;
        }

        public static bool IsWaypoint(string? logicStr)
        {
            if (logicStr == null) return false;
            return waypoints.Exists((raw) => raw.name == logicStr);
        }
    }
}
