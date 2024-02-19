using GrassRandoV2.Data;
using MenuChanger.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrassRandoV2.Settings
{
    public class ConnectionSettings
    {
        [MenuInclude] public bool Enabled { get; set; }
        [MenuInclude] public bool DisplayItems { get; set; }
        [MenuInclude] public bool GrassShop { get; set; }

        private bool GetGrassAreaFlagBit(GrassArea bit) => (allowedAreas & bit) != 0;
        private void SetGrassAreaFlagBit(GrassArea bit, bool value) => allowedAreas = value ? allowedAreas | bit : allowedAreas & ~bit;

        [MenuIgnore] public GrassArea allowedAreas;

        [MenuInclude]
        [MenuLabel("King's Pass")]
        public bool Kings_Pass { get { return GetGrassAreaFlagBit(GrassArea.KP); } set { SetGrassAreaFlagBit(GrassArea.KP, value); } }
        [MenuInclude]
        public bool Dirtmouth { get { return GetGrassAreaFlagBit(GrassArea.Dirtmouth); } set { SetGrassAreaFlagBit(GrassArea.Dirtmouth, value); } }
        [MenuInclude]
        [MenuLabel("Forgotten Crossroads")]
        public bool Crossroads { get { return GetGrassAreaFlagBit(GrassArea.Crossroads); } set { SetGrassAreaFlagBit(GrassArea.Crossroads, value); } }
        [MenuInclude]
        public bool Greenpath { get { return GetGrassAreaFlagBit(GrassArea.Greenpath); } set { SetGrassAreaFlagBit(GrassArea.Greenpath, value); } }
        [MenuInclude]
        [MenuLabel("Fungal Wastes")]
        public bool Fungal { get { return GetGrassAreaFlagBit(GrassArea.Fungal); } set { SetGrassAreaFlagBit(GrassArea.Fungal, value); } }
        [MenuInclude]
        [MenuLabel("Fog Canyon")]
        public bool Fog_Canyon { get { return GetGrassAreaFlagBit(GrassArea.Fog); } set { SetGrassAreaFlagBit(GrassArea.Fog, value); } }
        [MenuInclude]
        [MenuLabel("Queen's Gardens")]
        public bool Gardens { get { return GetGrassAreaFlagBit(GrassArea.Gardens); } set { SetGrassAreaFlagBit(GrassArea.Gardens, value); } }
        [MenuInclude]
        [MenuLabel("Resting Grounds")]
        public bool Grounds { get { return GetGrassAreaFlagBit(GrassArea.Grounds); } set { SetGrassAreaFlagBit(GrassArea.Grounds, value); } }
        [MenuInclude]
        [MenuLabel("City of Tears")]
        public bool City { get { return GetGrassAreaFlagBit(GrassArea.City); } set { SetGrassAreaFlagBit(GrassArea.City, value); } }
        [MenuInclude]
        [MenuLabel("Kingdom's Edge")]
        public bool Edge { get { return GetGrassAreaFlagBit(GrassArea.Edge); } set { SetGrassAreaFlagBit(GrassArea.Edge, value); } }
        [MenuInclude]
        public bool Deepnest { get { return GetGrassAreaFlagBit(GrassArea.Deepnest); } set { SetGrassAreaFlagBit(GrassArea.Deepnest, value); } }
        [MenuInclude]
        [MenuLabel("Ancient Basin")]
        public bool Basin { get { return GetGrassAreaFlagBit(GrassArea.Basin); } set { SetGrassAreaFlagBit(GrassArea.Basin, value); } }
        [MenuInclude]
        public bool Abyss { get { return GetGrassAreaFlagBit(GrassArea.Abyss); } set { SetGrassAreaFlagBit(GrassArea.Abyss, value); } }
        [MenuInclude]
        [MenuLabel("White Palace")]
        public bool Palace { get { return GetGrassAreaFlagBit(GrassArea.Palace); } set { SetGrassAreaFlagBit(GrassArea.Palace, value); } }
        [MenuInclude]
        public bool Godhome { get { return GetGrassAreaFlagBit(GrassArea.Godhome); } set { SetGrassAreaFlagBit(GrassArea.Godhome, value); } }

        private bool GetAreaTypeFlagBit(AreaType bit) => (allowedAreaTypes & bit) != 0;
        private void SetAreaTypeFlagBit(AreaType bit, bool value) => allowedAreaTypes = value ? allowedAreaTypes | bit : allowedAreaTypes & ~bit;

        [MenuIgnore] public AreaType allowedAreaTypes;

        [MenuInclude]
        [MenuLabel("Include Dream Grass")]
        internal bool IncludeDreams { get { return GetAreaTypeFlagBit(AreaType.Dream); } set { SetAreaTypeFlagBit(AreaType.Dream, value); } }

        public ConnectionSettings() { }

        public override string ToString()
        {
            return $"Enabled/{Enabled} DisplayItems/{DisplayItems} GrassShop/{GrassShop} Areas/{allowedAreas} Types/{allowedAreaTypes}";
        }
    }
}
