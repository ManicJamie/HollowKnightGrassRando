using ItemChanger.Extensions;
using ItemChanger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrassRando.IC.Costs
{
    public enum BreakableType
    {
        Grass = 1
    }

    public sealed record BreakableCost(int amount, BreakableType type, ComparisonOperator op = ComparisonOperator.Ge) : Cost
    {
        public override bool CanPay() => GrassRandoMod.Instance.saveData.grassCount.Compare(op, amount);
        public override void OnPay() { }
        public override bool HasPayEffects() => false;
        public override bool Includes(Cost c)
        {
            if (c is BreakableCost pic) return pic.amount <= amount && pic.op == op && pic.type == type;
            return base.Includes(c);
        }
        public override string GetCostText()
        {
            var name = type switch
            {
                BreakableType.Grass => Language.Language.Get("Grass", "Exact"),
                _ => ""
            };
            return $"{amount} {name}";
        }
    }
}
