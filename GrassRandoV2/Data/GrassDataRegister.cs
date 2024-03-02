using GrassRando.IC;
using GrassRando.Rando;
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
