using ItemChanger;
using ItemChanger.Modules;
using Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine;
using HutongGames.PlayMaker;
using ItemChanger.Extensions;
using HutongGames.PlayMaker.Actions;
using System.Collections;
using System.Reflection.Emit;
using Newtonsoft.Json;

namespace GrassRandoV2.IC.Modules
{
    /// <summary>
    /// Keeps dream entry objects active if grass in the scene has not been obtained.
    /// </summary>
    public class ReopenDreamsModule : Module
    {
        private readonly Dictionary<string, string> sceneToDreamScene = new()
        {
            { "Crossroads_10_boss_defeated", "Dream_01_False_Knight" },
            { "Ruins1_24_boss_defeated", "Dream_02_Mage_Lord" },
            { "Abyss_19", "Dream_03_Infected_Knight" },
            { "Waterways_15", "Dream_04_White_Defender" },
            { "RestingGrounds_04", "Dream_Nailcollection" },
        };

        private readonly Dictionary<string, string> sceneToPdBool = new()
        {
            { "Crossroads_10_boss_defeated", "falseKnightDreamDefeated" },
            { "Ruins1_24_boss_defeated", "mageLordDreamDefeated" },
            { "Abyss_19", "infectedKnightDreamDefeated" },
            { "Waterways_15", "whiteDefenderDefeated" },
            { "RestingGrounds_04", "hasDreamNail" },
        };
    
        public override void Initialize()
        {
            // Ensure dnail entry spawns if grass is present
            Events.AddFsmEdit(new FsmID("FK Corpse", "Control"), EditConstructionFSM);
            Events.AddFsmEdit(new FsmID("Mage Lord Remains", "Control"), EditConstructionFSM);
            Events.AddFsmEdit(new FsmID("IK Remains", "Control"), EditConstructionFSM);

            // Hijack the DreamNailCutsceneEvent module to add our own check
            IBool defaultBool = ItemChangerMod.Modules.GetOrAdd<DreamNailCutsceneEvent>().Closed;
            ItemChangerMod.Modules.GetOrAdd<DreamNailCutsceneEvent>().Closed = new GrassBoolWrapper(defaultBool, "Dream_Nailcollection");

            // Add our logic to the extra deconstructors (No I do not know why there is logic both to prevent spawning AND to despawn. pasta moment)
            Events.AddFsmEdit(new("Dream Enter", "FSM"), EditDestructionFSM2);
        }

        public override void Unload()
        {
            Events.RemoveFsmEdit(new FsmID("FK Corpse", "Control"), EditConstructionFSM);
            Events.RemoveFsmEdit(new FsmID("Mage Lord Remains", "Control"), EditConstructionFSM);
            Events.RemoveFsmEdit(new FsmID("IK Remains", "Control"), EditConstructionFSM);

            // Unwrap the bool in default module we wrapped
            GrassBoolWrapper? boolWrapper = (GrassBoolWrapper?)ItemChangerMod.Modules.Get<DreamNailCutsceneEvent>()?.Closed;
            if (boolWrapper != null)
            {
                ItemChangerMod.Modules.Get<DreamNailCutsceneEvent>()!.Closed = boolWrapper.WrappedBool;
            }

            Events.RemoveFsmEdit(new("Dream Enter", "FSM"), EditDestructionFSM2);
        }

        private void EditConstructionFSM(PlayMakerFSM fsm)
        {
            // Both the check we want (dream nail) and that we want to override (falseKnightDreamDefeated) share a state & hence both call the same events.
            // For some reason these are chained the silly way around, so instead we'll just replace them with a composite check action.

            GrassRandoV2Mod.Instance.Log($"Editing fsm {fsm.FsmName} on scene {fsm.gameObject.scene.name}");
            var dreamScene = sceneToDreamScene[fsm.gameObject.scene.name];
            var pdBool = sceneToPdBool[fsm.gameObject.scene.name];

            // Get state we want & clear the checks to be replaced
            var checkState = fsm.GetState("Check");
            checkState.RemoveActionsOfType<PlayerDataBoolTest>();

            var glowEvent = new FsmEvent("GLOW");
            var noEvent = new FsmEvent("NO");

            checkState.AddLastAction(new CheckGrassDreamBossEntry(dreamScene, pdBool)
            {
                shouldEnter = glowEvent,
                shouldNotEnter = noEvent
            });
        }


        /// <summary>
        /// Dream enters are despawned when the pd bool is active; replace the check for this to include grass
        /// </summary>
        private void EditDestructionFSM2(PlayMakerFSM fsm)
        {
            GrassRandoV2Mod.Instance.Log($"Editing fsm {fsm.FsmName} on scene {fsm.gameObject.scene.name}");
            var dreamScene = sceneToDreamScene[fsm.gameObject.scene.name];
            var pdBool = sceneToPdBool[fsm.gameObject.scene.name];

            // Get state we want to replace action on
            var checkState = fsm.GetState("Check");

            // New action
            var destroyEvent = new FsmEvent("DESTROY");
            var action = new CheckGrassDreamBossDestructor(dreamScene, pdBool)
            {
                shouldDestroy = destroyEvent
            };

            checkState.ReplaceAction(action, 0);
        }
    }

    public class GrassBoolWrapper : IBool
    {
        public IBool WrappedBool;
        public string grassScene;

        public GrassBoolWrapper(IBool toWrap, string grassScene)
        {
            WrappedBool = toWrap;
            this.grassScene = grassScene;
        }

        [JsonIgnore]
        public bool Value { get
            {
                if (LocationRegistrar.Instance.GetUnobtainedGrass(grassScene).Count != 0)
                {
                    return false;
                }
                return WrappedBool.Value;
            } }

        public IBool Clone()
        {
            return new GrassBoolWrapper(WrappedBool.Clone(), grassScene);
        }
    }

    /// <summary>
    /// If we should spawn the dream entry object; has dnail & (grass in the scene is uncollected OR boss undefeated)
    /// </summary>
    internal class CheckGrassDreamBossEntry : FsmStateAction
    {
        public FsmString dreamScene;
        public FsmString pdBool;

        public FsmEvent? shouldEnter;
        public FsmEvent? shouldNotEnter;

        public CheckGrassDreamBossEntry(string dreamScene, string pdBool)
        {
            this.dreamScene = dreamScene;
            this.pdBool = pdBool;
        }

        public override void OnEnter()
        {
            if (PlayerData.instance.GetBool("hasDreamNail") && 
                (!PlayerData.instance.GetBool(pdBool.Value) || LocationRegistrar.Instance.GetUnobtainedGrass(dreamScene.Value).Count != 0))
            {
                Fsm.Event(shouldEnter);
            }
            else
            {
                Fsm.Event(shouldNotEnter);
            }
            Finish();
        }
    }

    /// <summary>
    /// If we should despawn the dream entry object; pdBool is high AND no grass present
    /// </summary>
    internal class CheckGrassDreamBossDestructor : FsmStateAction
    {
        public FsmString dreamScene;
        public FsmString pdBool;

        public FsmEvent? shouldDestroy;
        public FsmEvent? shouldNotDestroy;

        public CheckGrassDreamBossDestructor(string dreamScene, string pdBool)
        {
            this.dreamScene = dreamScene;
            this.pdBool = pdBool;
        }

        public override void OnEnter()
        {
            if (PlayerData.instance.GetBool(pdBool.Value) && LocationRegistrar.Instance.GetUnobtainedGrass(dreamScene.Value).Count != 0)
            {
                Fsm.Event(shouldDestroy);
            }
            else
            {
                Fsm.Event(shouldNotDestroy);
            }
            Finish();
        }
    }
}
