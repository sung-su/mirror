using System;
using System.Collections.Generic; // for Dictionary
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.Applications;

using Tizen.System;

using SettingAppTextResopurces.TextResources;

using System.Runtime.InteropServices;



namespace SettingMain
{
    class SettingContent_SetDate : SettingContent_Base
    {

        public SettingContent_SetDate()
            : base()
        {
            mTitle = Resources.IDS_ST_BUTTON_BACK;
        }

        protected override View CreateContent(Window window)
        {
            var datepicker = new DatePicker()
            {
                //WidthSpecification = LayoutParamPolicies.MatchParent,
                //HeightSpecification = LayoutParamPolicies.MatchParent,

                Date = DateTime.Now,
            };

            var button = new Button()
            {
                // WidthSpecification = LayoutParamPolicies.MatchParent,
                // HeightSpecification = LayoutParamPolicies.MatchParent,
                Text = Resources.IDS_ST_BUTTON_OK
            };
            button.Clicked += (bo, be) =>
            {
                DateTime date = datepicker.Date;
                Tizen.Log.Debug("NUI", String.Format("local time : {0}.{1}.{2}", date.Year, date.Month, date.Day));
                DateTime utime = date.ToUniversalTime();
                Tizen.Log.Debug("NUI", String.Format("universal time : {0}.{1}.{2}", utime.Year, utime.Month, utime.Day));

                var dialog = new AlertDialog()
                {
                    Title = Resources.IDS_ST_BODY_SET_DATE,
                    Message = Resources.IDS_ST_BODY_DO_YOU_WANT_CHANGE_THE_DATE,
                };

                var buttonCancel = new Button()
                {
                    Text = Resources.IDS_ST_BUTTON_CANCEL,
                };
                buttonCancel.Clicked += (abo, abe) =>
                {
                    // Update Widget Content by sending message to pop the fourth page.
                    Bundle nextBundle2 = new Bundle();
                    nextBundle2.AddItem("WIDGET_ACTION", "POP");
                    String encodedBundle2 = nextBundle2.Encode();
                    SetContentInfo(encodedBundle2);
                };
                var buttonOK = new Button()
                {
                    Text = Resources.IDS_ST_BUTTON_OK,
                };
                buttonOK.Clicked += (abo, abe) =>
                {
                    DateTime setTime = datepicker.Date;
                    DateTime curTime = DateTime.Now;

                    DateTime ltime = new DateTime(setTime.Year, setTime.Month, setTime.Day, curTime.Hour, curTime.Minute, curTime.Second, curTime.Millisecond);
                    DateTime utime2 = ltime.ToUniversalTime();

                    
                    DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    TimeSpan delta = utime2 - epoch;
                    
                    Int64 timetick = Convert.ToInt64(delta.TotalSeconds);

                    Interop.Alarm.Alarmmgr_SetSystime(timetick);

                    // Update Widget Content by sending message to pop the fourth page.
                    Bundle nextBundle2 = new Bundle();
                    nextBundle2.AddItem("WIDGET_ACTION", "POP");
                    String encodedBundle2 = nextBundle2.Encode();
                    SetContentInfo(encodedBundle2);
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
            content.Add(new TextLabel(Resources.IDS_ST_BODY_SET_DATE));
            content.Add(datepicker);
            content.Add(button);

            return content;
        }
    }
}
