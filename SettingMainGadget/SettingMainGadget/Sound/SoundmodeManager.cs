using SettingMainGadget.TextResources;
using SettingCore;
using Tizen.NUI;
using Tizen.Applications;
using Tizen.Multimedia;

namespace SettingMainGadget.Sound
{
    public enum Soundmode
    {
        SOUND_MODE_SOUND,
        SOUND_MODE_VIBRATE,
        SOUND_MODE_MUTE
    };

    internal class SoundmodeManager
    {
        private const string VconfSoundOn = "db/setting/sound/sound_on";
        private const string VconfVibrationOn = "db/setting/sound/vibration_on";
        private const string SoundSystemLevel = "sound_system_level";
        private const string SoundNotificationLevel = "sound_notification_level";

        public static Soundmode GetSoundmode()
        {
            if (!Tizen.Vconf.TryGetBool(VconfSoundOn, out bool isSoundOn))
            {
                Logger.Warn($"could not get value for {VconfSoundOn}");
            }
            if (!Tizen.Vconf.TryGetBool(VconfVibrationOn, out bool isVibrationOn))
            {
                Logger.Warn($"could not get value for {VconfVibrationOn}");
            }

            if (isSoundOn)
                return Soundmode.SOUND_MODE_SOUND;
            else if (isVibrationOn)
                return Soundmode.SOUND_MODE_VIBRATE;

            return Soundmode.SOUND_MODE_MUTE;
        }

        public static void SetSoundmode(Soundmode soundmode)
        {
            bool have_sound = false, have_vibrations = false;

            switch (soundmode)
            {
                case Soundmode.SOUND_MODE_SOUND:
                    have_sound = true;
                    have_vibrations = false;
                    break;
                case Soundmode.SOUND_MODE_VIBRATE:
                    have_sound = false;
                    have_vibrations = true;
                    break;
                case Soundmode.SOUND_MODE_MUTE:
                    have_sound = false;
                    have_vibrations = false;
                    break;
            };

            if (!Tizen.Vconf.SetBool(VconfSoundOn, have_sound))
            {
                Logger.Warn($"could not set key {VconfSoundOn} with value {have_sound}");
            }

            if (!Tizen.Vconf.SetBool(VconfVibrationOn, have_vibrations))
            {
                Logger.Warn($"could not set key {VconfVibrationOn} with value {have_vibrations}");
            }

            if(soundmode == Soundmode.SOUND_MODE_SOUND)
            {
                // restore the previous sound level

                if (Preference.Contains(SoundSystemLevel))
                {
                    SettingAudioManager.SetVolumeLevel(AudioVolumeType.System, Preference.Get<int>(SoundSystemLevel));
                }

                if (Preference.Contains(SoundNotificationLevel))
                {
                    SettingAudioManager.SetVolumeLevel(AudioVolumeType.Notification, Preference.Get<int>(SoundNotificationLevel));
                }
            }
            else
            {
                // save the current sound level and set the value to 0

                Preference.Set(SoundSystemLevel, SettingAudioManager.GetVolumeLevel(AudioVolumeType.System));
                Preference.Set(SoundNotificationLevel, SettingAudioManager.GetVolumeLevel(AudioVolumeType.Notification));

                SettingAudioManager.SetVolumeLevel(AudioVolumeType.System, 0);
                SettingAudioManager.SetVolumeLevel(AudioVolumeType.Notification, 0);
            }
        }

        public static string GetSoundmodeName(NUIGadget gadget, Soundmode soundmode)
        {
            return soundmode switch
            {
                Soundmode.SOUND_MODE_SOUND => gadget.NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_HEADER_SOUND)),
                Soundmode.SOUND_MODE_VIBRATE => gadget.NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_HEADER_VIBRATE)),
                Soundmode.SOUND_MODE_MUTE => gadget.NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_HEADER_MUTE)),
                _ => string.Empty,
            };
        }
    }
}
