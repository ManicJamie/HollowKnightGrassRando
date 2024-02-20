using System;
using UnityEngine;
using ItemChanger;
using ItemChanger.Internal;

namespace GrassRando.IC
{
    [Serializable]
    public class GrassSprite : ISprite
    {
        private static SpriteManager ebsm = new(typeof(GrassSprite).Assembly, "GrassRandoV2.Resources.");

        public GrassSprite()
        {

        }

        [Newtonsoft.Json.JsonIgnore]
        public Sprite Value => ebsm.GetSprite("grassIcon");
        public ISprite Clone() => (ISprite)MemberwiseClone();
    }
}
