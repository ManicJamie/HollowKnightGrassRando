using MenuChanger;
using MenuChanger.MenuElements;
using MenuChanger.MenuPanels;
using MenuChanger.Extensions;
using static RandomizerMod.Localization;
using RandomizerMod.Menu;
using System.Collections.Generic;
using GrassRandoV2.Settings;

namespace GrassRandoV2.Rando
{

    public class RandoMenuPage
    {
        internal MenuPage Page;
        internal MenuElementFactory<ConnectionSettings> ElementFactory;
        internal VerticalItemPanel Container;

        internal SmallButton EntryButton;

        internal static RandoMenuPage? Instance { get; private set; }

        public static void Hook()
        {
            RandomizerMenuAPI.AddMenuPage((MenuPage lp) => Instance = new(lp), HandleTopLevelButton);
            MenuChangerMod.OnExitMainMenu += () => Instance = null;
        }

        private static bool HandleTopLevelButton(MenuPage landingPage, out SmallButton button)
        {
            button = Instance!.EntryButton;
            return true;
        }

        private void SetTopLevelButtonColor()
        {
            if (EntryButton != null)
            {
                EntryButton.Text.color = GrassRandoV2Mod.Instance.settings.Enabled ? Colors.TRUE_COLOR : Colors.DEFAULT_COLOR;
            }
        }

        public void PasteSettings()
        {
            var settings = GrassRandoV2Mod.Instance.settings;
            GrassRandoV2Mod.Instance.Log($"Pasting settings {GrassRandoV2Mod.Instance.settings}");
            ElementFactory.SetMenuValues(GrassRandoV2Mod.Instance.settings);
        }

        private RandoMenuPage(MenuPage lp)
        {
            // Set up page & set buttons on show
            Page = new MenuPage("Grass Randomizer", lp);
            // Page.BeforeShow += PasteSettings;
            lp.BeforeShow += SetTopLevelButtonColor;
            // Create entry button
            EntryButton = new(lp, Localize("Grass Randomizer"));
            EntryButton.AddHideAndShowEvent(lp, Page);

            ElementFactory = new(Page, GrassRandoV2Mod.Instance.settings);

            IMenuElement[] topBarContents = new IMenuElement[]
            {
                ElementFactory.ElementLookup[nameof(GrassRandoV2Mod.Instance.settings.Enabled)],
                ElementFactory.ElementLookup[nameof(GrassRandoV2Mod.Instance.settings.DisplayItems)],
            };

            IMenuElement[] includesBarContents = new IMenuElement[]
            {
                ElementFactory.ElementLookup[nameof(GrassRandoV2Mod.Instance.settings.IncludeDreams)],
                ElementFactory.ElementLookup[nameof(GrassRandoV2Mod.Instance.settings.GrassShop)],

            };

            IMenuElement[] areasBarContents = new IMenuElement[]
            {
                ElementFactory.ElementLookup[nameof(GrassRandoV2Mod.Instance.settings.Kings_Pass)],
                ElementFactory.ElementLookup[nameof(GrassRandoV2Mod.Instance.settings.Dirtmouth)],
                ElementFactory.ElementLookup[nameof(GrassRandoV2Mod.Instance.settings.Crossroads)],
                ElementFactory.ElementLookup[nameof(GrassRandoV2Mod.Instance.settings.Greenpath)],
                ElementFactory.ElementLookup[nameof(GrassRandoV2Mod.Instance.settings.Fungal)],
                ElementFactory.ElementLookup[nameof(GrassRandoV2Mod.Instance.settings.Fog_Canyon)],
                ElementFactory.ElementLookup[nameof(GrassRandoV2Mod.Instance.settings.Gardens)],
                ElementFactory.ElementLookup[nameof(GrassRandoV2Mod.Instance.settings.Grounds)],
                ElementFactory.ElementLookup[nameof(GrassRandoV2Mod.Instance.settings.City)],
                ElementFactory.ElementLookup[nameof(GrassRandoV2Mod.Instance.settings.Edge)],
                ElementFactory.ElementLookup[nameof(GrassRandoV2Mod.Instance.settings.Deepnest)],
                ElementFactory.ElementLookup[nameof(GrassRandoV2Mod.Instance.settings.Basin)],
                ElementFactory.ElementLookup[nameof(GrassRandoV2Mod.Instance.settings.Abyss)],
                ElementFactory.ElementLookup[nameof(GrassRandoV2Mod.Instance.settings.Palace)],
                ElementFactory.ElementLookup[nameof(GrassRandoV2Mod.Instance.settings.Godhome)],
            };

            GridItemPanel MainPanel = new(Page, new(0, 300), 2, SpaceParameters.VSPACE_SMALL, SpaceParameters.HSPACE_SMALL, false, topBarContents);
            GridItemPanel IncludesPanel = new(Page, new(0, 300), 1, SpaceParameters.VSPACE_SMALL, SpaceParameters.HSPACE_SMALL, false, includesBarContents);
            GridItemPanel AreasPanel = new(Page, new(0, 300), 3, SpaceParameters.VSPACE_SMALL, SpaceParameters.HSPACE_SMALL, false, areasBarContents);

            IMenuElement[] AllPanels = new IMenuElement[]
            {
                MainPanel,
                IncludesPanel,
                AreasPanel
            };

            Container = new(Page, new(0, 300), SpaceParameters.VSPACE_LARGE, true, AllPanels);
        }

    }
}
