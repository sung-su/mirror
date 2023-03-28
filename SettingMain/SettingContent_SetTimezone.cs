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
using System.IO;
using System.Collections;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.Applications;
using System.Collections.ObjectModel;
using Tizen.System;

using SettingCore.TextResources;

namespace SettingMain
{


   

    class SettingContent_SetTimezone : SettingContent_Base
    {
        private ReadOnlyCollection<TimeZoneInfo> mTimezoneList;

        private void MakeTimezoneList() 
        {
            mTimezoneList = TimeZoneInfo.GetSystemTimeZones();
        }


        private Picker mPicker = null;

        private string[] PickerItems;
        
        public SettingContent_SetTimezone()
            : base()
        {
            mTitle = Resources.IDS_ST_BUTTON_BACK;

            // Init data list
            MakeTimezoneList();

            // Make menu list
            PickerItems = new string[mTimezoneList.Count];

            int i = 0;
            foreach (var timezone in mTimezoneList)
            {
                PickerItems[i] = timezone.DisplayName;
                i++;
            }
        }

        protected override View CreateContent(Window window)
        {
            var picker = new Picker()
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                // HeightSpecification = LayoutParamPolicies.MatchParent,
            };

            ReadOnlyCollection<string> rc = new ReadOnlyCollection<string>(PickerItems);
            picker.DisplayedValues = rc;
            picker.MinValue = 0;
            picker.MaxValue = PickerItems.Length - 1;
            Tizen.Log.Debug("NUI", string.Format("picker.MinValue : {0}, picker.MaxValue : {1}", picker.MinValue, picker.MaxValue));

            picker.CurrentValue = GetTimezoneIndex();
            Tizen.Log.Debug("NUI", "CurrentValue : " + picker.CurrentValue.ToString());

            mPicker = picker;


            var slider = new Slider()
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                //Size2D = new Size2D(100, 32),
                //Name = Program.ItemContentNameDescription,

                TrackThickness = 5,
                BgTrackColor = new Color(0, 0, 0, 0.1f),
                SlidedTrackColor = new Color(0.05f, 0.63f, 0.9f, 1),
                ThumbSize = new Size(20, 20),

                Direction = Slider.DirectionType.Horizontal,
                MinValue = 0.0F,
                MaxValue = 1.0F,
                CurrentValue = picker.CurrentValue/(float)((picker.MaxValue - picker.MinValue) + 1),

            };
            slider.ValueChanged += OnValueChanged;
            slider.SlidingStarted += OnSlidingStarted;
            slider.SlidingFinished += OnSlidingFinished;


            var button = new Button()
            {
                // WidthSpecification = LayoutParamPolicies.MatchParent,
                // HeightSpecification = LayoutParamPolicies.MatchParent,
                Text = Resources.IDS_ST_BUTTON_OK
            };
            button.Clicked += (bo, be) =>
            {

                Tizen.Log.Debug("NUI", String.Format("current : {0}", picker.CurrentValue));

                SetTimezoneIndex(picker.CurrentValue);

                RequestWidgetPop();
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
            content.Add(new TextLabel(Resources.IDS_ST_BODY_TIME_ZONE));
            content.Add(picker);
            content.Add(slider);
            content.Add(button);

            return content;
        }


        private void OnValueChanged(object sender, SliderValueChangedEventArgs args)
        {

        }

        private void OnSlidingStarted(object sender, SliderSlidingStartedEventArgs args)
        {
        }

        private void OnSlidingFinished(object sender, SliderSlidingFinishedEventArgs args)
        {
            var slider = sender as Slider;

            mPicker.CurrentValue = (int)(slider.CurrentValue * ((mPicker.MaxValue - mPicker.MinValue) + 1));

        }


        /// /////////////////////////////////////////////////////////////////////////

        /// <returns></returns>
        public int GetTimezoneIndex()
        {
            string timezoneId = GetTimezoneId();


            int i = 0;
            foreach (var timezone in mTimezoneList)
            {
                if (timezoneId.Equals(mTimezoneList[i].Id))
                    return i;
                i++;
            }

            return -1;
        }

        private void SetTimezoneIndex(int index)
        {
            Tizen.Log.Debug("NUI", "Set Timezone : {0} - {1}", mTimezoneList[index].Id, mTimezoneList[index].DisplayName);

            string timezoneId = mTimezoneList[index].Id;
            SetTimezone(timezoneId);
        }


        private static void SetTimezone(string timezoneId)
        {
            SystemSettings.LocaleTimeZone = timezoneId;
        }

        public static string GetTimezoneId()
        {
            return SystemSettings.LocaleTimeZone;
        }
        public static string GetTimezoneName()
        {
            // DO NOT USE TimeZoneInfo localtimezone = TimeZoneInfo.Local;
            // It take long time to sync TimeZoneInfo.Local after setting SystemSettings.LocaleTimeZone

            TimeZoneInfo localtimezone = TimeZoneInfo.FindSystemTimeZoneById(SystemSettings.LocaleTimeZone);
            var date1 = new DateTime(2015, 1, 15);
            var date2 = new DateTime(2015, 7, 15);

            TimeSpan o1 = localtimezone.GetUtcOffset(date1);
            TimeSpan o2 = localtimezone.GetUtcOffset(date2);
            string o1String = " 00:00";
            string o2String = " 00:00";

            if (o1 < TimeSpan.Zero)
                o1String = o1.ToString(@"\-hh\:mm");
            else if (o1 > TimeSpan.Zero)
                o1String = o1.ToString(@"\+hh\:mm");
            if (o2 < TimeSpan.Zero)
                o2String = o2.ToString(@"\-hh\:mm");
            else if (o2 > TimeSpan.Zero)
                o2String = o2.ToString(@"\+hh\:mm");
            return localtimezone.StandardName + ", GMT " + o1String;
        }
    }
}

