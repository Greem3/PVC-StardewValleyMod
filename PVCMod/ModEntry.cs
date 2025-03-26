using System.Diagnostics;
using System.Numerics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Network;
using Microsoft.Xna.Framework;
using XnaVector2 = Microsoft.Xna.Framework.Vector2;

namespace PVCMod
{
    internal sealed class ModEntry : Mod
    {
        private Microphone userMicrophone;
        private Receiver audioReceiver;
        public ModConfig Config;

        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        }

        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            if (!Context.IsMultiplayer)
                return;

            Config = Helper.ReadConfig<ModConfig>();

            Helper.Events.Multiplayer.ModMessageReceived += OnModMessageReceived;
            Helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
            Helper.Events.Input.ButtonPressed += OnButtonPressed;

            audioReceiver = new Receiver(this, Config.ReceiverPort);
            userMicrophone = new Microphone(this);
            userMicrophone.Start();
            audioReceiver.Play();
        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (e.Button == Config.KeyToTalk)
            {
                userMicrophone.ChangeState();
                return;
            }

            if (e.Button == Config.KeyToDontHear)
            {
                audioReceiver.ChangeState();
            }
        }

        private void OnReturnedToTitle(object? sender, ReturnedToTitleEventArgs e)
        {
            userMicrophone.Stop();
            audioReceiver.Close();
        }

        private void OnModMessageReceived(object? sender, ModMessageReceivedEventArgs e)
        {
            Monitor.Log("Se envio un mensaje!", LogLevel.Error);

            if (e.FromModID != ModManifest.UniqueID)
                return;

            if (e.Type == "PVCAudioAvailable")
            {
                Monitor.Log($"Recibiendo audio de un jugador", LogLevel.Error);
                byte[] audioData = e.ReadAs<byte[]>();

                Debug.WriteLine($"Bytes recibidos: {string.Join("", audioData)}");

                audioReceiver.AddAudioData(audioData);
                return;
            }
        }

        public long[] PlayersInRange()
        {
            return Game1.getOnlineFarmers().Where(
                f =>
                {
                    //float distance = XnaVector2.Distance(f.Tile, Game1.player.Position);

                    //if (distance > 10f * Game1.tileSize)
                    //    return false;

                    return true;
                }
            )
            .Select(f => f.UniqueMultiplayerID)
            .ToArray();
        }
    }
}
