using ItemChanger;
using ItemChanger.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrassRando.IC.Costs
{
    // From MoreLocations under MIT; https://github.com/BadMagic100/MoreLocations/blob/master/MoreLocations/ItemChanger/MixedCostDisplayerSelectionStrategy.cs
    // Using this here since grass shop may want to contain multiple types of breakables later on, and using this makes that easy.
    public class MixedCostDisplayerSelectionStrategy : ICostDisplayerSelectionStrategy
    {
        /// <summary>
        /// The CostDisplayers this strategy can support.
        /// </summary>
        public List<IMixedCostSupport> Capabilities { get; set; } = new();

        /// <summary>
        /// Retrieves the appropriate CostDisplayer for the item's base cost.
        /// </summary>
        public CostDisplayer GetCostDisplayer(AbstractItem item)
        {
            Cost? c = item.GetTag<CostTag>()?.Cost;
            if (c == null) { return new GeoCostDisplayer(); }

            Cost baseCost = c.GetBaseCost();
            if (baseCost is MultiCost mc)
            {
                return GetDisplayerForBaseCost(mc.Select(cc => cc.GetBaseCost()));
            } else
            {
                return GetDisplayerForBaseCost(baseCost.Yield());
            }
        }

        private CostDisplayer GetDisplayerForBaseCost(IEnumerable<Cost> costs)
        {
            foreach (Cost c in costs)
            {
                CostDisplayer? best = Capabilities
                    .Where(cap => cap.MatchesCost(c))
                    .Select(cap => cap.GetDisplayer(c))
                    .FirstOrDefault();
                if (best != null) return best;
            }
            return new GeoCostDisplayer();
        }
    }
}
