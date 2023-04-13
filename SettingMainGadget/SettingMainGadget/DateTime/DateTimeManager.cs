using SettingCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace SettingMainGadget.DateTime
{
    public static class DateTimeManager
    {
        private const string VconfAutoDateTimeUpdate = "db/setting/automatic_time_update";
        private const string VconfTimeFormat = "db/menu_widget/regionformat_time1224";

        public static bool Is24HourFormat
        {
            get
            {
                if (!Tizen.Vconf.TryGetInt(VconfTimeFormat, out int timeformat))
                {
                    Logger.Warn($"could not get value for {VconfTimeFormat}");
                }

                return timeformat.Equals(2) ? true : false;
            }
            set
            {
                Tizen.Vconf.SetInt(VconfTimeFormat, value ? 2 : 1);
                Logger.Debug(String.Format("time format changed to: {0}", value ? "24h" : "12h"));
            }
        }

        public static bool AutoTimeUpdate
        {
            get
            {
                if (!Tizen.Vconf.TryGetBool(VconfAutoDateTimeUpdate, out bool autoUpdate))
                {
                    Logger.Warn($"could not get value for {VconfAutoDateTimeUpdate}");
                }

                return autoUpdate;
            }
            set
            {
                Tizen.Vconf.SetBool(VconfAutoDateTimeUpdate, value);
                Logger.Debug($"date & time auto update value: {value}");
            }
        }

        public static string FormattedTime
        {
            get => System.DateTime.Now.ToString(Is24HourFormat ? "HH:mm" : "h:mm tt");
        }
    }
}
