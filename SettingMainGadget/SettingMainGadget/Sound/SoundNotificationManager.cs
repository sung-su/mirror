using SettingMainGadget.TextResources;
using SettingCore;
using System;
using System.Linq;
using Tizen.Multimedia;
using Tizen.NUI;

namespace SettingMainGadget.Sound
{
    public class SoundNotificationManager
    {
        public static void SetNotificationSound(string notificationsound)
        {
            Tizen.Vconf.SetString(SettingAudioManager.VconfRingtonePath, notificationsound);
        }

        public static string GetNotificationSound()
        {
            // FIXME: ignoring return value
            Tizen.Vconf.TryGetString(SettingAudioManager.VconfRingtonePath, out string value);

            return value;
        }

        public static string GetFileName(string path)
        {
            String[] folders = path.Split('/');

            return folders.Length > 0 ? folders.Last() : path;
        }

        public static string SettingMediaBasename(NUIGadget gadget, string path)
        {
            if (string.IsNullOrEmpty(path))
                return gadget.NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_PHONEPROFILES_SILENT));

            string title = String.Empty;

            try
            {
                var extractor = new MetadataExtractor(path);
                Metadata metadata = extractor.GetMetadata();
                title = metadata.Title;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }

            return !string.IsNullOrEmpty(title) ? title : GetFileName(path);
        }

        public static string GetNotificationSoundName(NUIGadget gadget)
        {
            string path = GetNotificationSound();
            return SettingMediaBasename(gadget, path);
        }
    }
}
