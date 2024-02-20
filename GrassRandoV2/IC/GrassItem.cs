using ItemChanger;

namespace GrassRando.IC
{
    public class GrassItem : AbstractItem
    {
        public override void GiveImmediate(GiveInfo info)
        {
            GrassRandoMod.Instance.saveData.grassCount++;
            GrassRandoMod.Instance.LogDebug($"New grass count: {GrassRandoMod.Instance.saveData.grassCount}");
        }
    }
}
