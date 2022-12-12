using System;
using System.Collections.Generic;
using System.Text;
using Tizen.Multimedia;

namespace SettingMain
{
    class SettingAudioManager
    {
        private static AudioVolume audioVolume = AudioManager.VolumeController;

        public static int GetVolumeLevel(AudioVolumeType type)
        {
            return audioVolume.Level[type];
        }

        public static int GetMaxVolumeLevel(AudioVolumeType type)
        {
            return audioVolume.MaxLevel[type];
        }

        public static float GetPercentageVolumeLevel(AudioVolumeType type)
        {
            return (float)audioVolume.Level[type] / audioVolume.MaxLevel[type];
        }

        public static void SetVolumeLevel(AudioVolumeType type, int volume)
        {
            volume = volume > audioVolume.MaxLevel[type] ? audioVolume.MaxLevel[type] : volume;

            try
            {
                audioVolume.Level[type] = volume;
                Tizen.Log.Debug("NUI", $"SET {type} Volume : {audioVolume.Level[type]}");
            }
            catch (Exception ex)
            {
                Tizen.Log.Error("NUI", $"SET {type} Volume Error: {ex.Message}");
            }          
        }

        public static void PlayAudio(AudioStreamType type, string path)
        {
            try
            {
                WavPlayer.StartAsync(path, new AudioStreamPolicy(type));
            }
            catch (Exception ex)
            {
                Tizen.Log.Error("NUI", $"WavPlayer {type} Error: {ex.Message}");
            }
        }
    }
}
