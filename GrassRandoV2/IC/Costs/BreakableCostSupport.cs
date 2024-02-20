using ItemChanger;
using ItemChanger.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrassRando.IC.Costs
{
    public class BreakableCostSupport : IMixedCostSupport
    {
        public CostDisplayer GetDisplayer(Cost c)
        {
            return new BreakableCostDisplayer();
        }

        public bool MatchesCost(Cost c)
        {
            return c.GetBaseCost() is BreakableCost bcost;
        }
    }

    public class BreakableCostDisplayer : CostDisplayer
    {
        public override ISprite? CustomCostSprite { get; set; } = new GrassSprite();

        public override bool Cumulative => true;

        protected override bool SupportsCost(Cost cost) => cost is BreakableCost;

        protected override int GetSingleCostDisplayAmount(Cost cost) => ((BreakableCost)cost).amount;
    }
}
