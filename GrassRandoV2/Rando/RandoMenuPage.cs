using MenuChanger;
using MenuChanger.MenuElements;
using MenuChanger.MenuPanels;
using MenuChanger.Extensions;
using static RandomizerMod.Localization;
using RandomizerMod.Menu;
using System.Collections.Generic;
using GrassRando.Settings;

namespace GrassRando.Rando
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
                EntryButton.Text.color = GrassRandoMod.Instance.settings.Enabled ? Colors.TRUE_COLOR : Colors.DEFAULT_COLOR;
            }
        }

        public void PasteSettings()
        {
            ElementFactory.SetMenuValues(GrassRandoMod.Instance.settings);
            SetTopLevelButtonColor();
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

            ElementFactory = new(Page, GrassRandoMod.Instance.settings);

            IMenuElement[] topBarContents = new IMenuElement[]
            {
                ElementFactory.ElementLookup[nameof(GrassRandoMod.Instance.settings.Enabled)],
                ElementFactory.ElementLookup[nameof(GrassRandoMod.Instance.settings.DisplayItems)],
            };

            IMenuElement[] includesBarContents = new IMenuElement[]
            {
                ElementFactory.ElementLookup[nameof(GrassRandoMod.Instance.settings.IncludeDreams)],
                ElementFactory.ElementLookup[nameof(GrassRandoMod.Instance.settings.GrassShop)],

            };

            IMenuElement[] areasBarContents = new IMenuElement[]
            {
                ElementFactory.ElementLookup[nameof(GrassRandoMod.Instance.settings.Kings_Pass)],
                ElementFactory.ElementLookup[nameof(GrassRandoMod.Instance.settings.Dirtmouth)],
                ElementFactory.ElementLookup[nameof(GrassRandoMod.Instance.settings.Crossroads)],
                ElementFactory.ElementLookup[nameof(GrassRandoMod.Instance.settings.Greenpath)],
                ElementFactory.ElementLookup[nameof(GrassRandoMod.Instance.settings.Fungal)],
                ElementFactory.ElementLookup[nameof(GrassRandoMod.Instance.settings.Fog_Canyon)],
                ElementFactory.ElementLookup[nameof(GrassRandoMod.Instance.settings.Gardens)],
                ElementFactory.ElementLookup[nameof(GrassRandoMod.Instance.settings.Grounds)],
                ElementFactory.ElementLookup[nameof(GrassRandoMod.Instance.settings.City)],
                ElementFactory.ElementLookup[nameof(GrassRandoMod.Instance.settings.Edge)],
                ElementFactory.ElementLookup[nameof(GrassRandoMod.Instance.settings.Deepnest)],
                ElementFactory.ElementLookup[nameof(GrassRandoMod.Instance.settings.Basin)],
                ElementFactory.ElementLookup[nameof(GrassRandoMod.Instance.settings.Abyss)],
                ElementFactory.ElementLookup[nameof(GrassRandoMod.Instance.settings.Palace)],
                ElementFactory.ElementLookup[nameof(GrassRandoMod.Instance.settings.Godhome)],
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
