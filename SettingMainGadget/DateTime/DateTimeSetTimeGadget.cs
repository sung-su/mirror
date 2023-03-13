using SettingAppTextResopurces.TextResources;
using SettingCore;
using SettingMainGadget.DateTime;
using System;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;

namespace Setting.Menu.DateTime
{
    public class DateTimeSetTimeGadget : SettingCore.MenuGadget
    {
        public override string ProvideTitle() => Resources.IDS_ST_BODY_SET_TIME;

        protected override View OnCreate()
        {
            base.OnCreate();

            var content = new View()
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
                Layout = new LinearLayout()
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                },
            };

            var timepicker = new TimePicker()
            {
                Time = System.DateTime.Now,
                HeightResizePolicy = ResizePolicyType.FitToChildren,
                Is24HourView = DateTimeManager.Is24HourFormat,
            };

            var button = new Button("Tizen.NUI.Components.Button.Outlined")
            {
                Text = Resources.IDS_ST_BUTTON_OK
            };
            button.Clicked += (bo, be) =>
            {
                SetSystemTime(timepicker.Time);
                NavigateBack();
            };

            content.Add(timepicker);
            content.Add(button);

            return content;
        }

        private void SetSystemTime(System.DateTime time)
        {
            System.DateTime setTime = time;
            System.DateTime curTime = System.DateTime.Now;

            System.DateTime ltime = new System.DateTime(curTime.Year, curTime.Month, curTime.Day, setTime.Hour, setTime.Minute, setTime.Second, setTime.Millisecond);
            System.DateTime utime2 = ltime.ToUniversalTime();

            System.DateTime epoch = new System.DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan delta = utime2 - epoch;

            Int64 timetick = Convert.ToInt64(delta.TotalSeconds);
            SettingMainGadget.DateTime.Interop.Alarm.Alarmmgr_SetSystime(timetick);

            Logger.Debug($"system time was changed to: {time}");
        }
    }
}
