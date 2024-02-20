using GrassRando.IC.Costs;
using GrassRando.IC.Modules;
using ItemChanger;
using ItemChanger.Locations;
using Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GrassRando.IC
{
    public class GrassShopLocation : CustomShopLocation
    {
        public GrassShopLocation()
        {
            dungDiscount = false;
            flingType = FlingType.DirectDeposit;
            costDisplayerSelectionStrategy = new MixedCostDisplayerSelectionStrategy()
            {
                Capabilities =
                {
                    new BreakableCostSupport()
                }
            };
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            ItemChangerMod.Modules.GetOrAdd<KeepQuirrelAliveModule>();
        }

        protected override void OnUnload()
        {
            base.OnUnload();
        }
    }
}
