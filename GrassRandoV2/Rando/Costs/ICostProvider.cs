using RandomizerCore.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrassRandoV2.Rando.Costs
{
    public interface ICostProvider
    {
        bool HasNonFreeCostsAvailable { get; }

        LogicCost Next(LogicManager lm, Random rng);

        void PreRandomize(Random rng);
    }
}
