using SettingAppTextResopurces.TextResources;
using SettingCore;

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
        }

        public static string GetSoundmodeName(Soundmode soundmode)
        {
            return soundmode switch
            {
                Soundmode.SOUND_MODE_SOUND => Resources.IDS_ST_HEADER_SOUND,
                Soundmode.SOUND_MODE_VIBRATE => Resources.IDS_ST_HEADER_VIBRATE,
                Soundmode.SOUND_MODE_MUTE => Resources.IDS_ST_HEADER_MUTE,
                _ => string.Empty,
            };
        }
    }
}
