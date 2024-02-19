using ItemChanger;

namespace GrassRandoV2.IC
{
    public class GrassItem : AbstractItem
    {
        public override void GiveImmediate(GiveInfo info)
        {
            GrassRandoV2Mod.Instance.saveData.grassCount++;
            GrassRandoV2Mod.Instance.LogDebug($"New grass count: {GrassRandoV2Mod.Instance.saveData.grassCount}");
        }
    }
}
