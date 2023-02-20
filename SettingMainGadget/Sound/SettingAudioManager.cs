using SettingCore;
using System;
using Tizen.Multimedia;
using Tizen.NUI;
using Tizen.System;

namespace SettingMainGadget.Sound
{
    class SettingAudioManager
    {
        private const string ringtone = "media/settings/Ringtones/ringtone_sdk.mp3";
        private const string VconfRingtonePath = "db/setting/sound/noti/msg_ringtone_path";

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
            volume = volume < 0 ? 0 : volume;
            volume = volume > audioVolume.MaxLevel[type] ? audioVolume.MaxLevel[type] : volume;

            audioVolume.Level[type] = volume;
            Tizen.Log.Debug("NUI", $"SET {type} Volume : {audioVolume.Level[type]}");        
        }

        public static void PlayAudio(AudioStreamType type)
        {
            string path = String.Empty;

            switch (type)
            {
                case AudioStreamType.Media:
                    path = System.IO.Path.Combine(NUIApplication.Current.DirectoryInfo.Resource, ringtone);
                    break;
                case AudioStreamType.Notification:
                    if (!Tizen.Vconf.TryGetString(VconfRingtonePath, out string value))
                    {
                        Logger.Warn($"could not get vaule for {VconfRingtonePath}");
                        return;
                    }
                    path = value;
                    break;
                case AudioStreamType.System:
                    Feedback feedback = new Feedback();
                    if (feedback.IsSupportedPattern(FeedbackType.Sound, "Tap"))
                    {
                        feedback.Play(FeedbackType.Sound, "Tap");
                    }
                    return;
            }

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
