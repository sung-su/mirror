using SettingCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Tizen.System;

namespace SettingMainGadget.DateTime
{
    public static class DateTimeTimezoneManager
    {
        public static List<TimeZoneInfo> TimeZones { get; private set; } = new List<TimeZoneInfo>(TimeZoneInfo.GetSystemTimeZones());

        public static void SetTimezone(string timezoneId)
        {
            SystemSettings.LocaleTimeZone = timezoneId;
        }

        public static int GetTimezoneIndex()
        {
            string timezoneId = SystemSettings.LocaleTimeZone;
            var timezone = TimeZones.Where(x => x.Id == timezoneId).First();

            if(timezone != null)
            {
                Logger.Debug($"current timezone is: {timezone.DisplayName} - {timezone.Id}");
                return TimeZones.IndexOf(timezone);
            }

            return -1;
        }

        public static string GetTimezoneName()
        {
            // DO NOT USE TimeZoneInfo localtimezone = TimeZoneInfo.Local;
            // It take long time to sync TimeZoneInfo.Local after setting SystemSettings.LocaleTimeZone

            TimeZoneInfo localtimezone = TimeZoneInfo.FindSystemTimeZoneById(SystemSettings.LocaleTimeZone);
            var date = System.DateTime.Now;
            TimeSpan time = localtimezone.GetUtcOffset(date);
            string offset = time < TimeSpan.Zero ? time.ToString(@"\-hh\:mm") : time.ToString(@"\+hh\:mm");

            return $"GMT {offset}, {localtimezone.StandardName}";
        }
    }
}
