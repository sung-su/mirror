using SettingAppTextResopurces.TextResources;
using SettingCore;
using System;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;

namespace Setting.Menu.DateTime
{
    public class DateTimeSetDateGadget : SettingCore.MenuGadget
    {
        public override string ProvideTitle() => Resources.IDS_ST_BODY_SET_DATE;

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

            var datepicker = new DatePicker()
            {
                Date = System.DateTime.Now,
                HeightResizePolicy = ResizePolicyType.FitToChildren,
            };

            var button = new Button("Tizen.NUI.Components.Button.Outlined")
            {
                Text = Resources.IDS_ST_BUTTON_OK
            };
            button.Clicked += (bo, be) =>
            {
                SetSystemTime(datepicker.Date);
                NavigateBack();
            };

            content.Add(datepicker);
            content.Add(button);

            return content;
        }

        private void SetSystemTime(System.DateTime date)
        {
            System.DateTime setTime = date;
            System.DateTime curTime = System.DateTime.Now;

            System.DateTime ltime = new System.DateTime(setTime.Year, setTime.Month, setTime.Day, curTime.Hour, curTime.Minute, curTime.Second, curTime.Millisecond);
            System.DateTime utime2 = ltime.ToUniversalTime();

            System.DateTime epoch = new System.DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan delta = utime2 - epoch;

            Int64 timetick = Convert.ToInt64(delta.TotalSeconds);
            SettingMainGadget.DateTime.Interop.Alarm.Alarmmgr_SetSystime(timetick);

            Logger.Debug($"system date was changed to: {date}");
        }
    }
}
