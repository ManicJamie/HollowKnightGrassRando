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

        public readonly static Dictionary<string, RawWaypointDef> waypoints = new();

        static GrassDataRegister()
        {
            List<RawWaypointDef> waypointList = JsonUtil.Deserialize<List<RawWaypointDef>>("GrassRandoV2.Resources.waypoints.json");
            foreach (var waypoint in waypointList)
            {
                waypoints.Add(waypoint.name, waypoint);
            }
        }

        public static RawLogicDef? DecomposeLogic(string waypointName)
        {
            if (waypoints.TryGetValue(waypointName, out var waypointDef))
            {
                return waypointDef;
            }
            return null;
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

    }
}
