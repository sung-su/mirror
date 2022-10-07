using System;
using System.Collections.Generic; // for Dictionary
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.Applications;

using Tizen.System;

using SettingAppTextResopurces.TextResources;



namespace SettingMain
{
    class SettingContent_SetTime : SettingContent_Base
    {
        public SettingContent_SetTime()
            : base()
        {
            mTitle = Resources.IDS_ST_BUTTON_BACK;
        }

        protected override View CreateContent(Window window)
        {
            var timepicker = new TimePicker()
            {
                //WidthSpecification = LayoutParamPolicies.MatchParent,
                //HeightSpecification = LayoutParamPolicies.MatchParent,

                Time = DateTime.Now,
            };

            var button = new Button()
            {
                // WidthSpecification = LayoutParamPolicies.MatchParent,
                // HeightSpecification = LayoutParamPolicies.MatchParent,
                Text = Resources.IDS_ST_BUTTON_OK
            };
            button.Clicked += (bo, be) =>
            {
                DateTime time = timepicker.Time;
                Tizen.Log.Debug("NUI", String.Format("local time - {0}:{1}:{2}", time.Hour, time.Minute, time.Second));
                DateTime utime = time.ToUniversalTime();
                Tizen.Log.Debug("NUI", String.Format("universal time - {0}:{1}:{2}", utime.Hour, utime.Minute, utime.Second));

                var dialog = new AlertDialog()
                {
                    Title = Resources.IDS_ST_BODY_SET_TIME,
                    Message = Resources.IDS_ST_BODY_DO_YOU_WANT_CHANGE_THE_TIME,
                };

                var buttonCancel = new Button()
                {
                    Text = Resources.IDS_ST_BUTTON_CANCEL,
                };
                buttonCancel.Clicked += (abo, abe) =>
                {
                    RequestWidgetPop();
                };
                var buttonOK = new Button()
                {
                    Text = Resources.IDS_ST_BUTTON_OK,
                };
                buttonOK.Clicked += (abo, abe) =>
                {
                    DateTime setTime = timepicker.Time;
                    DateTime curTime = DateTime.Now;

                    DateTime ltime = new DateTime(curTime.Year, curTime.Month, curTime.Day, setTime.Hour, setTime.Minute, setTime.Second, setTime.Millisecond);
                    DateTime utime2 = ltime.ToUniversalTime();


                    DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    TimeSpan delta = utime2 - epoch;

                    Int64 timetick = Convert.ToInt64(delta.TotalSeconds);

                    Interop.Alarm.Alarmmgr_SetSystime(timetick);

                    RequestWidgetPop();
                };

                dialog.Actions = new View[] { buttonCancel, buttonOK };
                window.Add(dialog);
            };


            var content = new View()
            {

                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
                Layout = new LinearLayout()
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                },
            };
            content.Add(new TextLabel(Resources.IDS_ST_BODY_SET_TIME));
            content.Add(timepicker);
            content.Add(button);

            return content;
        }
    }
}
