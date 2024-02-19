using ItemChanger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrassRandoV2.IC.Costs
{
    // From MoreLocations under MIT; https://github.com/BadMagic100/MoreLocations/blob/c4e59aac16ab2b7f2f311ca6e42b84e020e7756c/MoreLocations/ItemChanger/CostIconSupport/IMixedCostSupport.cs
    public interface IMixedCostSupport
    {
        /// <summary>
        /// Whether a given unwrapped cost is matched by this.
        /// </summary>
        public bool MatchesCost(Cost c);
        /// <summary>
        /// Gets a cost displayer for a given matching cost
        /// </summary>
        public CostDisplayer GetDisplayer(Cost c);
    }
}
