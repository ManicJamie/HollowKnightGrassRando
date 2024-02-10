using ItemChanger.Locations;
using ItemChanger;
using UnityEngine;
using System;
using HutongGames.PlayMaker;
using Modding;
using GrassCore;
using System.Linq;
using Newtonsoft.Json;

namespace GrassRandoV2.IC
{
    public class BreakableGrassLocation : AutoLocation
    {
        public GrassKey key;

        [JsonConstructor]
        public BreakableGrassLocation(GrassKey key)
        {
            this.key = key;
        }

        protected override void OnLoad()
        {
            LocationRegistrar.Instance.Add(this);
        }

        protected override void OnUnload() 
        {
            LocationRegistrar.Instance.Remove(this);
        }

        public void Obtain()
        {
            if (!Placement.AllObtained())
            {
                MessageType mt = GrassRandoV2Mod.Instance.settings.DisplayItems ? MessageType.Corner : MessageType.None;
                Placement.GiveAll(new GiveInfo() { FlingType = FlingType.DirectDeposit, MessageType = mt });
            } else
            {
                //TODO: log?
            }
        }
    }
}
