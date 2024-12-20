using GrassCore;
using GrassRando;
using GrassRando.Data;
using GrassRando.IC;
using HKMirror.Reflection;
using ItemChanger.Modules;
using Modding;
using Modding.Utils;
using RandomizerCore.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GrassRandoV2.IC.Modules
{
#if DEBUG  // Debug module displaying & providing functions to rewrite grass groups
    internal static class LogicOverrides
    {
        public static Dictionary<GrassKey, string> all = [];
        public static List<GrassKey> history = [];

        public static string? Get(GrassKey key) => all.TryGetValue(key, out var value) ? value : null;
        public static string GetOrDefault(GrassKey key, string def) => all.TryGetValue(key, out var value) ? value : def;
        public static void Override(GrassKey key, string value)
        {
            all[key] = value;
            history.Add(key);
        }

        public static void DropLast()
        {
            var key = history.Pop();
            all.Remove(key);
        }

        public static void Serialize()
        {
            GameManager.instance.Reflect().ShowSaveIcon();  // To show that something happened
            JsonUtil.Serialize(all, "GrassDump.json");
            GameManager.instance.Reflect().HideSaveIcon();
        }
    }

    internal class DebugRenderModule : Module
    {
        private Behaviour? UtilBehaviour;
        private GrassKey? lastKey = null;

        private int newGroupId = 0;

        public override void Initialize()
        {
            UtilBehaviour ??= Behaviour.CreateBehaviour();

            var renderer = UtilBehaviour.gameObject.GetOrAddComponent<DebugRenderer>();
            
            var colorListener = UtilBehaviour.gameObject.AddComponent<HotkeyListener>();
            colorListener.key = KeyCode.Return;
            colorListener.execute = renderer.ResetColors;
            
            var listener = UtilBehaviour.gameObject.AddComponent<HotkeyListener>();
            listener.key = KeyCode.RightShift;
            listener.execute = DumpGroup;

            var undoListener = UtilBehaviour.gameObject.AddComponent<HotkeyListener>();
            undoListener.key = KeyCode.RightControl;
            undoListener.execute = () => {
                if (LogicOverrides.all.Count > 0) 
                {
                    LogicOverrides.DropLast();
                    LogicOverrides.Serialize();
                }
            };

            var newGroup = UtilBehaviour.gameObject.AddComponent<HotkeyListener>();
            newGroup.key = KeyCode.Menu;
            newGroup.execute = NewGroup;
        }

        public override void Unload()
        {
            GameObject.Destroy(UtilBehaviour?.gameObject);
            UtilBehaviour = null;
        }

        public void DumpGroup()
        {
            var heroPos = (Vector2)HeroController.instance.gameObject.transform.position;
            var ordered = LocationRegistrar.Instance.activeSceneGrassLocations
                .Select(kvp => new
                {
                    key = kvp.Key,
                    distance = (kvp.Key.Position - heroPos).magnitude,
                    logic = LogicOverrides.GetOrDefault(kvp.Key, kvp.Value.GetLogicDef().InfixSource)
                })
                .OrderBy(info => info.distance).ToList();
            var target = ordered.First();
            var newLogic = ordered.SkipWhile(info => info.logic == target.logic).First().logic;
            if (newLogic != null)
            {
                LogicOverrides.Override(target.key, newLogic);
                lastKey = target.key;
                LogicOverrides.Serialize();
            }
        }

        public void NewGroup()
        {
            var heroPos = (Vector2)HeroController.instance.gameObject.transform.position;
            var ordered = LocationRegistrar.Instance.activeSceneGrassLocations
                .Select(kvp => new
                {
                    key = kvp.Key,
                    distance = (kvp.Key.Position - heroPos).magnitude,
                    logic = LogicOverrides.GetOrDefault(kvp.Key, kvp.Value.GetLogicDef().InfixSource)
                })
                .OrderBy(info => info.distance).ToList();
            var target = ordered.First();

            var newGroup = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name + "_NEW_" + newGroupId++;
            LogicOverrides.Override(target.key, newGroup);
            lastKey = target.key;
            LogicOverrides.Serialize();
        }
    }

    internal class HotkeyListener : MonoBehaviour
    {
        public KeyCode key;
        public Action? execute;

        public void Update()
        {
            if (Input.GetKeyDown(key))
            {
                execute?.Invoke();
            }
        }
    }

    /// <summary>
    /// Draws colored circles on locations depending on their group
    /// </summary>
    public class DebugRenderer : MonoBehaviour
    {
        private static readonly Dictionary<string, Color> groupToColor = [];

        public static float Z = 0.005f;

        public void ResetColors() => groupToColor.Clear();

        public void OnGUI()
        {
            if (Event.current?.type != EventType.Repaint || Camera.main == null || GameManager.instance == null || GameManager.instance.isPaused)
            {
                return;
            }

            foreach (var kvp in LocationRegistrar.Instance.activeSceneGrassLocations)
            {
                var logic = LogicOverrides.Get(kvp.Key) ?? kvp.Value.GetLogicDef().InfixSource;
                if (!groupToColor.TryGetValue(logic, out Color color))
                {
                    color = GetColor();
                    groupToColor[logic] = color;
                }
                Utils.Drawing.DrawCircle(LocalToScreenPoint(kvp.Key.Position), 40, color, 4f, 4);
            }
        }

        private static Color GetColor()
        {
            return UnityEngine.Random.ColorHSV(hueMin: 0f, hueMax: 1f, saturationMin: 1f, saturationMax: 1f, valueMin: 1f, valueMax: 1f);
        }

        private static Vector2 LocalToScreenPoint(Vector2 point)
        {
            Vector2 result = Camera.main.WorldToScreenPoint(new Vector3(point.x, point.y, Z));
            return new Vector2((int)Math.Round(result.x), (int)Math.Round(Screen.height - result.y));
        }
    }
#endif
}
