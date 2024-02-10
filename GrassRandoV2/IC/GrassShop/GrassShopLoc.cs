using GrassRandoV2.Data;
using ItemChanger;
using ItemChanger.Locations;
using ItemChanger.Locations.SpecialLocations;
using RandomizerMod.RC;
using RandomizerMod.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GrassRandoV2.IC.GrassShop
{
    public class GrassShopLoc : CostChestLocation
    {

        public string objectName;
        //public string fsmType;
        public GrassData_New gd;
        //public Fsm fsm;
        private chestLoc cLoc;
        private tabletLoc tablet;

        public record GrassShopDeployer : Deployer
        {
            public string SceneName;
            public float X;
            public float Y;


            public override GameObject Instantiate()
            {
                GameObject go = new()
                {
                    name = "grassShop",

                };

                //go.scene = "Tutorial_0";

                SceneName = "Tutorial_0";
                X = 0;
                Y = 0;

                return go;
            }
        }

        private class chestLoc : ContainerLocation
        {
            protected override void OnLoad()
            {
                throw new NotImplementedException();
            }

            protected override void OnUnload()
            {
                throw new NotImplementedException();
            }
        }

        private class tabletLoc : PlaceableLocation
        {
            public override void PlaceContainer(GameObject obj, string containerType)
            {
                throw new NotImplementedException();
            }

            protected override void OnLoad()
            {
                throw new NotImplementedException();
            }

            protected override void OnUnload()
            {
                throw new NotImplementedException();
            }
        }

        public void Init()
        {
            chestLocation = cLoc;
            tabletLocation = tablet;
        }

        public static AbstractPlacement GetGrassShopPlacement(ICFactory fact, string locationName)
        {
            AbstractPlacement place = fact.MakeLocation(locationName).Wrap();
            //place.AddTag<ItemChanger.Tags.DestroyGrubRewardTag>().destroyRewards = GetRandomGrassRewards(fact.gs);
            return place;
        }

        protected override void OnLoad()
        {
            GrassShopDeployer gsd = new();
            gsd.Deploy();
        }

        protected override void OnUnload()
        {
            //throw new NotImplementedException();
        }
    }
}
