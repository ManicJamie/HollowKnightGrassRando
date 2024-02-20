using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrassRando.Data
{
    /// <summary>
    /// Flag enum on area.
    /// </summary>
    [Flags]
    public enum GrassArea
    {
        None = 0,
        KP = 1 << 0,
        Dirtmouth = 1 << 1,
        Crossroads = 1 << 2,
        Greenpath = 1 << 3,
        Fungal = 1 << 4,
        Fog = 1 << 5,
        Gardens = 1 << 6,
        Grounds = 1 << 7,
        City = 1 << 8,
        Edge = 1 << 9,
        Deepnest = 1 << 10,
        Basin = 1 << 11,
        Abyss = 1 << 12,
        Palace = 1 << 13,
        Godhome = 1 << 14,

        // Helpers
        KPDirtmouth = KP | Dirtmouth,
    }

    [Flags]
    public enum AreaType
    {
        None = 0,
        Dream = 1 << 0
    }
}
