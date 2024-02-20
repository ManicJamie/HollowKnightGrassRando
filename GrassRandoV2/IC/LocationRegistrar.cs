using GrassCore;
using InControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace GrassRando.IC
{
    public class LocationRegistrar
    {
        public Dictionary<string, Dictionary<GrassKey, BreakableGrassLocation>> GrassLocations = new();
        // Maintained separately to allow quick access time

        private static LocationRegistrar? _instance;
        public static LocationRegistrar Instance { get { _instance ??= new LocationRegistrar(); return _instance; }  }

        public LocationRegistrar()
        {
            GrassEventDispatcher.GrassWasCut += GrassCutHandler;
        }

        public void Add(BreakableGrassLocation location)
        {
            TryAddScene(location.sceneName!);
            GrassLocations[location.sceneName!][location.key] = location;
        }

        public void Remove(BreakableGrassLocation location)
        {
            if (!GrassLocations.TryGetValue(location.sceneName!, out var sceneDict)) { return; }
            sceneDict.Remove(location.key);
        }

        /// <summary>
        /// Retrieves a dictionary of GrassKey:GrassLocation for the current scene, or an empty dict if the scene has no locations.
        /// </summary>
        public Dictionary<GrassKey, BreakableGrassLocation>? GetLocationsInScene(string sceneName)
        {
            if (!GrassLocations.TryGetValue(sceneName, out var sceneDict)) { return new(); }
            return sceneDict;
        }

        /// <summary>
        /// Retrieves a list of locations in the scene that have no items left to collect
        /// </summary>
        public List<BreakableGrassLocation> GetObtainedGrass(string sceneName)
        {
            var sceneLocations = GetLocationsInScene(sceneName);
            if (sceneLocations == null) return new();

            List<BreakableGrassLocation> obtained = new();
            foreach (var kvp in sceneLocations)
            {
                var loc = kvp.Value;
                if (loc.Placement.AllObtained())
                {
                    obtained.Add(loc);
                }
            }

            return obtained;
        }

        /// <summary>
        /// Retrieves a list of locations in the scene that have items to collect
        /// </summary>
        public List<BreakableGrassLocation> GetUnobtainedGrass(string sceneName)
        {
            var sceneLocations = GetLocationsInScene(sceneName);
            if (sceneLocations == null) return new();

            List<BreakableGrassLocation> obtained = new();
            foreach (var kvp in sceneLocations)
            {
                var loc = kvp.Value;
                if (!loc.Placement.AllObtained())
                {
                    obtained.Add(loc);
                }
            }

            return obtained;
        }

        private BreakableGrassLocation? GetLocation(GrassKey key)
        {
            if (!GrassLocations.TryGetValue(key.SceneName, out Dictionary<GrassKey, BreakableGrassLocation> sceneDict)) { return null; }
            if (!sceneDict.TryGetValue(key, out BreakableGrassLocation location)) { return null; }
            return location;
        }

        private void GrassCutHandler(GrassKey key)
        {
            var location = GetLocation(key);
            if (location == null) { return; }

            location.Obtain();
        }

        private void TryAddScene(string sceneName)
        {
            if (!GrassLocations.ContainsKey(sceneName))
            {
                GrassLocations.Add(sceneName, new Dictionary<GrassKey, BreakableGrassLocation>());
            }
        }
    }
}
