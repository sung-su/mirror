using System;
using System.Collections.Generic;
using System.Linq;
using Tizen.System;

namespace SettingMainGadget.DateTime
{
    public static class DateTimeTimezoneManager
    {
        public static List<TimeZone> GetTimeZones()
        {
            var timeZones = new List<TimeZone>();
            foreach (var timeZoneInfo in TimeZoneInfo.GetSystemTimeZones())
            {
                timeZones.Add(new TimeZone(timeZoneInfo));
            }

            return timeZones;
        }

        public static void SetTimezone(string timezoneId)
        {
            SystemSettings.LocaleTimeZone = timezoneId;
        }

        public static (string offset, string timezoneName) GetTimezoneName()
        {
            // DO NOT USE TimeZoneInfo localtimezone = TimeZoneInfo.Local;
            // It take long time to sync TimeZoneInfo.Local after setting SystemSettings.LocaleTimeZone

            TimeZoneInfo localtimezone = TimeZoneInfo.FindSystemTimeZoneById(SystemSettings.LocaleTimeZone);
            var date = System.DateTime.Now;
            TimeSpan time = localtimezone.GetUtcOffset(date);
            string offset = time < TimeSpan.Zero ? time.ToString(@"\-hh\:mm") : time.ToString(@"\+hh\:mm");

            return (offset, $"GMT {offset}, {localtimezone.StandardName}");
        }

        public class TimeZone
        {
            public TimeZoneInfo Info { get; private set; }

            public TimeZone(TimeZoneInfo timeZone)
            {
                this.Info = timeZone;
            }

            public string City
            {
                get => Info.Id.Split('/').Last();
            }

            public string Continent
            {
                get => Info.Id.Split('/').First();
            }

            public string DisplayName
            {
                get
                {
                    var identifiers = Info.Id.Split('/').Reverse().ToList();
                    string place = String.Join(", ", identifiers.ToArray()).Replace('_', ' ');

                    var displayName = Info.DisplayName;
                    int index = displayName.LastIndexOf("Time") + 4;
                    displayName = displayName.Substring(0, index);

                    return $"{place} {displayName}";
                }
            }
        }
    }
}
