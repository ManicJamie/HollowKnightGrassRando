using System;
using UnityEngine;
using ItemChanger;
using ItemChanger.Internal;
using Newtonsoft.Json;

namespace GrassRando.IC
{
    [Serializable]
    public class GrassSprite : ISprite
    {
        private static SpriteManager ebsm = new(typeof(GrassSprite).Assembly, "GrassRandoV2.Resources.");

        public GrassSprite()
        {

        }

        [JsonIgnore]
        public Sprite Value => ebsm.GetSprite("grassIcon");
        public ISprite Clone() => (ISprite)MemberwiseClone();
    }

    [Serializable]
    public class SmallGrassSprite : ISprite
    {
        private static SpriteManager ebsm = new(typeof(GrassSprite).Assembly, "GrassRandoV2.Resources.");
        [JsonIgnore]
        public static readonly (int, int) size = (60, 60);

        public SmallGrassSprite()
        {

        }

        [JsonIgnore]
        public Sprite Value => ebsm.GetSprite("smallGrassIcon");
        public ISprite Clone() => (ISprite)MemberwiseClone();

        
    }
}
