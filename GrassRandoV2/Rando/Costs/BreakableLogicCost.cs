using ItemChanger;
using Newtonsoft.Json;
using RandomizerCore.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrassRandoV2.Rando.Costs
{
    public class BreakableLogicCost : LogicCost
    {
        public Term? term;
        public int logicCost;

        [JsonIgnore]
        private Func<int, Cost>? converter;

        public BreakableLogicCost(Term term, int logicCost, Func<int, Cost> icConverter)
        {
            this.term = term;
            this.logicCost = logicCost;
            this.converter = icConverter;
        }

        public override bool CanGet(ProgressionManager pm)
        {
            return pm.Has(term!, logicCost);
        }

        public override IEnumerable<Term> GetTerms()
        {
            if (term == null)
            {
                throw new InvalidOperationException("Term is undefined");
            }
            yield return term;
        }

        public Cost GetIcCost()
        {
            return converter?.Invoke(logicCost) ?? throw new InvalidOperationException("Cost converter is undefined");
        }
    }
}
