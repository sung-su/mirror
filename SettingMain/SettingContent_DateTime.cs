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

using SettingCore.TextResources;

namespace SettingMain
{
    class SettingContent_DateTime : SettingContent_Base
    {
        private DefaultLinearItem mDateItem = null;
        private DefaultLinearItem mTimeItem = null;
        private DefaultLinearItem mTimezoneItem = null;

        public SettingContent_DateTime()
            : base()
        {
            mTitle = Resources.IDS_ST_BODY_DATE_AND_TIME;
        }

        protected override View CreateContent(Window window)
        {
            // Content of the page which scrolls items vertically.
            var content = new ScrollableBase()
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
                ScrollingDirection = ScrollableBase.Direction.Vertical,
                HideScrollbar = false,
                Layout = new LinearLayout()
                {
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                },
            };

            DefaultLinearItem item = null;

            bool bAutoupdate = Vconf.GetBool("db/setting/automatic_time_update");
            item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_MBODY_AUTO_UPDATE, null, false, true);
            if (item != null)
            {
                var toggle = item.Extra as Switch;
                toggle.IsSelected = bAutoupdate;


                toggle.SelectedChanged += (o, e) =>
                {
                    if (e.IsSelected)
                    {
                        Vconf.SetBool("db/setting/automatic_time_update", true);
                        Tizen.Log.Debug("NUI", "Auto Update is ON!\n");
                        ApplyAutomaticTimeUpdate();
                    }
                    else
                    {
                        Vconf.SetBool("db/setting/automatic_time_update", false);
                        Tizen.Log.Debug("NUI", "Auto Update is OFF!\n");
                        ApplyAutomaticTimeUpdate();
                    }
                };

                content.Add(item);
            }

            item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_SET_DATE, DateTime.Now.ToShortDateString());
            mDateItem = item;
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    // Update Widget Content by sending message to add the third page in advance.
                    Bundle nextBundle = new Bundle();
                    nextBundle.AddItem("WIDGET_ID", "setdate@org.tizen.cssettings");
                    nextBundle.AddItem("WIDGET_WIDTH", window.Size.Width.ToString());
                    nextBundle.AddItem("WIDGET_HEIGHT", window.Size.Height.ToString());
                    nextBundle.AddItem("WIDGET_PAGE", "CONTENT_PAGE");
                    nextBundle.AddItem("WIDGET_ACTION", "PUSH");
                    String encodedBundle = nextBundle.Encode();
                    SetContentInfo(encodedBundle);

                };
                content.Add(item);
            }


            item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_SET_TIME, DateTime.Now.ToShortTimeString());
            mTimeItem = item;
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    // Update Widget Content by sending message to add the third page in advance.
                    Bundle nextBundle = new Bundle();
                    nextBundle.AddItem("WIDGET_ID", "settime@org.tizen.cssettings");
                    nextBundle.AddItem("WIDGET_WIDTH", window.Size.Width.ToString());
                    nextBundle.AddItem("WIDGET_HEIGHT", window.Size.Height.ToString());
                    nextBundle.AddItem("WIDGET_PAGE", "CONTENT_PAGE");
                    nextBundle.AddItem("WIDGET_ACTION", "PUSH");
                    String encodedBundle = nextBundle.Encode();
                    SetContentInfo(encodedBundle);

                };
                content.Add(item);
            }


            item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_TIME_ZONE, SettingContent_SetTimezone.GetTimezoneName());
            mTimezoneItem = item;
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    // Update Widget Content by sending message to add the third page in advance.
                    Bundle nextBundle = new Bundle();
                    nextBundle.AddItem("WIDGET_ID", "settimezone@org.tizen.cssettings");
                    nextBundle.AddItem("WIDGET_WIDTH", window.Size.Width.ToString());
                    nextBundle.AddItem("WIDGET_HEIGHT", window.Size.Height.ToString());
                    nextBundle.AddItem("WIDGET_PAGE", "CONTENT_PAGE");
                    nextBundle.AddItem("WIDGET_ACTION", "PUSH");
                    String encodedBundle = nextBundle.Encode();
                    SetContentInfo(encodedBundle);
                };
                content.Add(item);
            }

            ApplyAutomaticTimeUpdate();

            bool bTime24 = (Vconf.GetInt("db/menu_widget/regionformat_time1224") == 2);
            item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_MBODY_24_HOUR_CLOCK, Resources.IDS_ST_SBODY_SHOW_THE_TIME_IN_24_HOUR_FORMAT_INSTEAD_OF_12_HOUR_HAM_PM_FORMAT, false, true);
            if (item != null)
            {
                var toggle = item.Extra as Switch;
                toggle.IsSelected = bTime24;


                toggle.SelectedChanged += (o, e) =>
                {
                    if (e.IsSelected)
                    {
                        Vconf.SetInt("db/menu_widget/regionformat_time1224", 2);
                        Tizen.Log.Debug("NUI", "Auto Update is ON!\n");
                    }
                    else
                    {
                        Vconf.SetInt("db/menu_widget/regionformat_time1224", 1);
                        Tizen.Log.Debug("NUI", "Auto Update is OFF!\n");
                    }
                };

                content.Add(item);
            }

            return content;
        }


        protected override void OnCreate(string contentInfo, Window window)
        {
            base.OnCreate(contentInfo, window);

            SystemSettings.TimeChanged += SystemSettings_TimeChanged;
            SystemSettings.LocaleTimeZoneChanged += SystemSettings_LocaleTimeZoneChanged;
#if false
            
            SystemSettings.AutomaticTimeUpdateChanged += SystemSettings_AutomaticTimeUpdateChanged;
#endif
        }

        protected override void OnTerminate(string contentInfo, TerminationType type)
        {
            SystemSettings.TimeChanged -= SystemSettings_TimeChanged;
            SystemSettings.LocaleTimeZoneChanged -= SystemSettings_LocaleTimeZoneChanged;
#if false

            SystemSettings.AutomaticTimeUpdateChanged -= SystemSettings_AutomaticTimeUpdateChanged;
#endif
            base.OnTerminate(contentInfo, type);
        }

        private void SystemSettings_TimeChanged(object sender, Tizen.System.TimeChangedEventArgs e)
        {
            Tizen.Log.Debug("NUI", "SystemSettings_TimeChanged is called");

            if (mDateItem != null)
                mDateItem.SubText = DateTime.Now.ToShortDateString();
            if (mTimeItem != null)
                mTimeItem.SubText = DateTime.Now.ToShortTimeString();
        }

        private void SystemSettings_LocaleTimeZoneChanged(object sender, LocaleTimeZoneChangedEventArgs e)
        {
            Tizen.Log.Debug("NUI", "SystemSettings_LocaleTimeZoneChanged is called");

            if (mTimezoneItem != null)
                mTimezoneItem.SubText = SettingContent_SetTimezone.GetTimezoneName();
        }

        private void SystemSettings_AutomaticTimeUpdateChanged(object sender, AutomaticTimeUpdateChangedEventArgs e)
        {
            ApplyAutomaticTimeUpdate();
        }

        private void ApplyAutomaticTimeUpdate()
        {
#if false
            if (SystemSettings.AutomaticTimeUpdate)
            {
                if (mDateItem != null) mDateItem.IsEnabled = false;
                if (mTimeItem != null) mTimeItem.IsEnabled = false;
                if (mTimezoneItem != null) mTimezoneItem.IsEnabled = false;
            }
            else {
                if (mDateItem != null) mDateItem.IsEnabled = true;
                if (mTimeItem != null) mTimeItem.IsEnabled = true;
                if (mTimezoneItem != null) mTimezoneItem.IsEnabled = true;
            }
#else
            if (Vconf.GetBool("db/setting/automatic_time_update"))
            {
                if (mDateItem != null) mDateItem.IsEnabled = false;
                if (mTimeItem != null) mTimeItem.IsEnabled = false;
                if (mTimezoneItem != null) mTimezoneItem.IsEnabled = false;
            }
            else
            {
                if (mDateItem != null) mDateItem.IsEnabled = true;
                if (mTimeItem != null) mTimeItem.IsEnabled = true;
                if (mTimezoneItem != null) mTimezoneItem.IsEnabled = true;
            }
#endif
        }
    }
}
