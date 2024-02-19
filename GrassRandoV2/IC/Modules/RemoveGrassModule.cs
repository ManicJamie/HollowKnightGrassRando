using ItemChanger.Extensions;
using ItemChanger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItemChanger.Modules;
using UnityEngine;
using UnityEngine.SceneManagement;
using GrassCore;

namespace GrassRandoV2.IC.Modules
{
    /// <summary>
    /// Removes grass on scene load as long as the placement has no items left to give.
    /// </summary>
    public class RemoveGrassModule : Module
    {
        public override void Initialize()
        {
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        }

        public override void Unload()
        {
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
        }

        private void SceneManager_activeSceneChanged(Scene source, Scene target)
        {
            var toKill = LocationRegistrar.Instance.GetObtainedGrass(target.name);
            var keyToGo = GetGrassInScene(target);

            foreach (var loc in toKill)
            {
                if (keyToGo.TryGetValue(loc.key, out var go))
                {
                    GameObject.Destroy(go);
                }
                else
                {
                    GrassRandoV2Mod.Instance.LogWarn($"RemoveGrassModule: Attempted to remove {loc.name} but key {loc.key} not present in scene!");
                }
            }
        }

        private Dictionary<GrassKey, GameObject> GetGrassInScene(Scene scene)
        {
            Dictionary<GrassKey, GameObject> result = new();

            foreach (GameObject go in scene.Traverse().ConvertAll<GameObject>(tuple => tuple.go))
            {
                if (GrassList.Contains(go))
                {
                    result.Add(new GrassKey(go), go);
                }
            }

            return result;
        }
    }
}
