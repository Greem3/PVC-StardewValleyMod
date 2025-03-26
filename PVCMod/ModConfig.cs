using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewValley;

namespace PVCMod
{
    class ModConfig
    {
        public int ReceiverPort { get; set; } = 24643;
        public bool HoldToTalk { get; set; } = false;
        public SButton KeyToTalk { get; set; } = SButton.H;
        public SButton KeyToTest { get; set; } = SButton.J;
        public SButton KeyToDontHear { get; set; } = SButton.L;
        public float UsersMicrophoneVolume { get; set; } = 1.0f;
    }
}
