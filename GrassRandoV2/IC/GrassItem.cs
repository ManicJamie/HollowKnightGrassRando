using GrassRando.IC.Modules;
using GrassRandoV2.IC.Modules;
using ItemChanger;
using System;

namespace GrassRando.IC
{
    public class GrassItem : AbstractItem
    {
        public delegate void GrassGiveEventHandler(int newCount);
        public static event GrassGiveEventHandler? GrassGiven;

        protected override void OnLoad()
        {
            ItemChangerMod.Modules.GetOrAdd<GrassCounterModule>();
        }

        public override void GiveImmediate(GiveInfo info)
        {
            GrassRandoMod.Instance.saveData.grassCount++;
            GrassGiven?.Invoke(GrassRandoMod.Instance.saveData.grassCount);
        }
    }
}
