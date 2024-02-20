using GrassCore;
using JetBrains.Annotations;
using Newtonsoft.Json;
using RandomizerMod.RandomizerData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrassRando.Data
{ 
    /// <summary>
    /// GrassData object as deserialised from the json
    /// </summary>
    public class GrassData_New
    {
        [JsonProperty]
        public string locationName = ""; // Logic-safe name, based on HKTranslated scene name & integer id per-scene.
        [JsonProperty]
        public GrassArea grassArea; // Used as part of pool definition
        [JsonProperty]
        public AreaType areaType; // Used as part of pool definition
        [JsonProperty]
        public string logic = "";
        [JsonProperty]
        public int id; // uniquely identifies grass per-scene without referencing position or objectName, neither of which are human-readable as keys.
        /// <summary>
        /// Used instead of the GrassKey for defining worldMapPosition. Used to fake pin locations to better positions in RandoMapMod.
        /// </summary>
        [JsonProperty]
        public (string, float, float)? mapSceneOverride;

        [JsonIgnore] // Constructed below
        internal GrassKey key;

        public GrassData_New(string sceneName, string objectName, float x, float y)
        {
            key = new(sceneName, objectName, x, y);
        }

        public override string ToString()
        {
            return $"{key} \\\\ {locationName} \\\\ {grassArea} \\\\ {logic}";
        }

        public string GetGroupName()
        {
            return $"grass_{grassArea}";
        }

        public bool IsIncluded(GrassArea includedAreas, AreaType includedSubAreas) => includedAreas.HasFlag(grassArea) && includedSubAreas.HasFlag(areaType);
    }
}
