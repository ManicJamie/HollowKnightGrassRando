using ItemChanger.Locations;
using ItemChanger;
using UnityEngine;
using System;
using HutongGames.PlayMaker;
using Modding;
using GrassCore;
using System.Linq;
using Newtonsoft.Json;
using GrassRando.Data;
using GrassRando.IC.Modules;

namespace GrassRando.IC
{
    public class BreakableGrassLocation : AutoLocation
    {
        public GrassKey key;
        public AreaType type;

        [JsonConstructor]
        public BreakableGrassLocation(GrassKey key)
        {
            this.key = key;
        }

        protected override void OnLoad()
        {
            LocationRegistrar.Instance.Add(this);
            ItemChangerMod.Modules.GetOrAdd<RemoveGrassModule>();
            ItemChangerMod.Modules.GetOrAdd<ReopenDreamsModule>();
        }

        protected override void OnUnload() 
        {
            LocationRegistrar.Instance.Remove(this);
        }

        public void Obtain()
        {
            if (!Placement.AllObtained())
            {
                MessageType mt = GrassRandoMod.Instance.settings.DisplayItems
                    ? MessageType.Corner
                    : (Placement.Items.Where((item) => item is not GrassItem).Count() > 0) ? MessageType.Corner : MessageType.None;
                Placement.GiveAll(new GiveInfo() { FlingType = FlingType.DirectDeposit, MessageType = mt });
            }
        }
    }
}
