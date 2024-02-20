using ItemChanger;
using RandomizerCore.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrassRando.Rando.Costs
{
    public class BreakableLogicCostProvider : ICostProvider
    {
        private Func<int, Cost> converter;

        private readonly string term;
        private readonly int min, max;
        public BreakableLogicCostProvider(string term, int min, int max, Func<int, Cost> converter)
        {
            this.term = term;
            this.min = min;
            this.max = max;
            this.converter = converter;
        }

        public bool HasNonFreeCostsAvailable => true;

        public LogicCost Next(LogicManager lm, Random rng)
        {
            return new BreakableLogicCost(lm.GetTermStrict(term), rng.Next(min, max + 1), converter);
        }

        public void PreRandomize(Random rng) { }
    }
}
