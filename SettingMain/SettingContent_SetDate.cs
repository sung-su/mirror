/*
 *  Copyright (c) 2022 Samsung Electronics Co., Ltd All Rights Reserved
 *
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License
 */

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
                    RequestWidgetPop();
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
            content.Add(new TextLabel(Resources.IDS_ST_BODY_SET_DATE));
            content.Add(datepicker);
            content.Add(button);

            return content;
        }
    }
}
