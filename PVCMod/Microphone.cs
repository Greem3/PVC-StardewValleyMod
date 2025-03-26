using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using NAudio;
using StardewModdingAPI;

namespace PVCMod
{
    class Microphone
    {
        private WaveInEvent waveIn;
        private ModEntry ModEntry;
        private bool isRecording;

        public Microphone(ModEntry mod)
        {
            ModEntry = mod;
            waveIn = new WaveInEvent();
            waveIn.WaveFormat = new WaveFormat(44100, 1); // 44.1 kHz, mono
            waveIn.DataAvailable += OnDataAvailable;
        }

        public void ChangeState()
        {
            if (isRecording)
            {
                this.Stop();
                return;
            }
            
            this.Start();
        }

        public void Start()
        {
            ModEntry.Monitor.Log("Microfono activado", LogLevel.Alert);
            waveIn.StartRecording();
            isRecording = true;
        }

        public void Stop()
        {
            ModEntry.Monitor.Log("Microfono desactivado", LogLevel.Alert);
            waveIn.StopRecording();
            isRecording = false;
        }

        public void Close()
        {
            ModEntry.Monitor.Log("Cerrando uso de microfono", LogLevel.Alert);
            waveIn.StopRecording();
            waveIn.Dispose();
            isRecording = false;
        }

        private void OnDataAvailable(object? sender, WaveInEventArgs e)
        {
            ModEntry.Monitor.Log("Consiguiendo jugadores dentro del rango...", LogLevel.Alert);

            long[] playersInRange = ModEntry.PlayersInRange();

            ModEntry.Monitor.Log($"Jugadores conseguidos: {string.Join(", ", playersInRange)}", LogLevel.Alert);
            ModEntry.Helper.Multiplayer.SendMessage(
                e.Buffer,
                "PVCAudioAvailable",
                new string[]{ ModEntry.ModManifest.UniqueID },
                playersInRange
            );
        }
    }
}
