using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using NAudio;
using NAudio.Wave;
using StardewModdingAPI;
using StardewValley;

namespace PVCMod
{
    class Receiver
    {
        private UdpClient udpClient;
        private WaveOutEvent waveOut;
        private BufferedWaveProvider waveProvider;
        private ModEntry ModEntry;
        private bool isReceiving;

        public Receiver(ModEntry modEntry)
        {
            ModEntry = modEntry;
            udpClient = new UdpClient(24643);
            waveOut = new WaveOutEvent();
            waveProvider = new BufferedWaveProvider(new WaveFormat(44100, 1));
            waveOut.Init(waveProvider);
            ChangeVolume();
        }

        public Receiver(ModEntry modEntry, int port)
        {
            ModEntry = modEntry;
            udpClient = new UdpClient(port);
            waveOut = new WaveOutEvent();
            waveProvider = new BufferedWaveProvider(new WaveFormat(44100, 1));
            waveOut.Init(waveProvider);
            ChangeVolume();
        }

        public void AddAudioData(byte[] data)
        {
            waveProvider.AddSamples(data, 0, data.Length);
            this.Play();
        }

        public void ChangeState()
        {
            if (isReceiving)
            {
                this.Stop();
                return;
            }

            this.Play();
        }

        public void Play()
        {
            ModEntry.Monitor.Log("Escuchando audios...", LogLevel.Error);

            if (waveOut.PlaybackState == PlaybackState.Playing)
                return;

            waveOut.Play();
            isReceiving = true;
        }

        public void Pause()
        {
            if (waveOut.PlaybackState != PlaybackState.Playing)
                return;

            waveOut.Pause();
        }

        public void Stop()
        {
            if (waveOut.PlaybackState == PlaybackState.Stopped)
                return;
            
            waveOut.Stop();
            isReceiving = false;
            ClearBuffer();
        }

        public void ClearBuffer()
        {
            waveProvider.ClearBuffer();
        }

        public int GetBufferedBytes()
        {
            return waveProvider.BufferedBytes;
        }

        public void Close()
        {
            udpClient.Close();
            waveOut.Stop();
            waveOut.Dispose();
        }

        public void ChangeVolume()
        {
            float volume = ModEntry.Config.UsersMicrophoneVolume;
            waveOut.Volume = volume;
        }
    }
}
