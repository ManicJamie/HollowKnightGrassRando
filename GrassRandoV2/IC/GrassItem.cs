using ItemChanger;

namespace GrassRandoV2.IC
{
    public class GrassItem : AbstractItem
    {
        public static int count;

        public override void GiveImmediate(GiveInfo info)
        {
            count++;
            GrassRandoV2Mod.Instance.LogDebug($"New grass count: {count}");
        }

        public static bool Spend(int cost)
        {
            if (cost > count) { return false; }
            count -= cost;
            return true;
        }
    }
}
