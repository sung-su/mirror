using SettingCore.TextResources;
using SettingCore;
using System.Collections.Generic;
using Tizen.System;

namespace SettingMainGadget.Display
{
    public class DisplayTimeOutManager
    {
        private const string VconfScreenTimeOut = "db/setting/lcd_backlight_normal";

        public class ScreenTimeoutInfo
        {
            private readonly string Name = null;
            private readonly int Value;

            public ScreenTimeoutInfo(string name, int value)
            {
                Name = name;
                Value = value;
            }

            public string GetName()
            {
                return Name;
            }

            public int GetValue()
            {
                return Value;
            }
        };

        public static readonly List<ScreenTimeoutInfo> TimeoutList = new List<ScreenTimeoutInfo>
        {
            new ScreenTimeoutInfo(Resources.IDS_ST_BODY_ALWAYS_ON, 0),
            new ScreenTimeoutInfo(Resources.IDS_ST_BODY_15SEC, 15),
            new ScreenTimeoutInfo(Resources.IDS_ST_BODY_30SEC, 30),
            new ScreenTimeoutInfo(Resources.IDS_ST_BODY_1_MINUTE, 60),
            new ScreenTimeoutInfo(Resources.IDS_ST_BODY_2_MINUTES, 120),
            new ScreenTimeoutInfo(Resources.IDS_ST_BODY_5_MINUTES, 300),
            new ScreenTimeoutInfo(Resources.IDS_ST_BODY_10_MINUTES, 600)
        };

        public static int GetScreenTimeoutIndex()
        {
            if (!Tizen.Vconf.TryGetInt(VconfScreenTimeOut, out int value))
            {
                Logger.Warn($"could not get value for {VconfScreenTimeOut}");
            }

            Logger.Debug($"ScreenTimeout value: {value}");

            int index;
            if (value < 15)
            {
                index = 0;
            }
            else if (value >= 15 && value < 30)
            {
                index = 1;
            }
            else if (value >= 30 && value < 60)
            {
                index = 2;
            }
            else if (value >= 60 && value < 120)
            {
                index = 3;
            }
            else if (value >= 120 && value < 300)
            {
                index = 4;
            }
            else if (value >= 300 && value < 600)
            {
                index = 5;
            }
            else
            {
                index = 6;
            }

            return index;
        }

        public static string GetScreenTimeoutName()
        {
            return TimeoutList[GetScreenTimeoutIndex()].GetName();
        }

        public static void SetScreenTimeout(int index)
        {
            SystemSettings.ScreenBacklightTime = TimeoutList[index].GetValue();
            Logger.Debug($"ScreenTimeOut changed value to: {TimeoutList[index].GetName()}");
        }
    }
}
