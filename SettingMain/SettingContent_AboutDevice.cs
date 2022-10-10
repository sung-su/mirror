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
using System.ComponentModel;
using System.Diagnostics;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.Applications;
using Tizen.Telephony;
using Tizen.Network.Bluetooth;

using SettingAppTextResopurces.TextResources;

namespace SettingMain
{
    class SettingContent_AboutDevice : SettingContent_Base
    {
        private DefaultLinearItem mDevicenameItem;

        public SettingContent_AboutDevice()
            : base()
        {
            mTitle = Resources.IDS_ST_BODY_ABOUT_DEVICE;

            mDevicenameItem = null;
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

            item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_HEADER_MANAGE_CERTIFICATES_ABB);
            content.Add(item);
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    RequestWidgetPush("certificates@org.tizen.cssetting-certificates");
                };
            }

            item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_OPEN_SOURCE_LICENCES);
            content.Add(item);
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    RequestWidgetPush("legalinfo@org.tizen.cssettings");
                };
            }


            /////////////////////////////////////////////////////

            content.Add(SettingItemCreator.CreateItemStatic(Resources.IDS_ST_BODY_DEVICE_INFO));

            String name = Vconf.GetString("db/setting/device_name");
            item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_NAME, name);
            mDevicenameItem = item;
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    RequestWidgetPush("renamedevice@org.tizen.cssettings");
                };
                content.Add(item);
            }

            string valuestring;
            bool result  = Tizen.System.Information.TryGetValue<string>("http://tizen.org/system/model_name", out valuestring);
            if (result)
                item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_MODEL_NUMBER, valuestring);
            else
                item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_MODEL_NUMBER, Resources.IDS_ST_HEADER_UNAVAILABLE);
            content.Add(item);

            result = Tizen.System.Information.TryGetValue<string>("http://tizen.org/feature/platform.version", out valuestring);
            if (result)
                item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_MBODY_TIZEN_VERSION, valuestring);
            else
                item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_MBODY_TIZEN_VERSION, Resources.IDS_ST_HEADER_UNAVAILABLE);
            content.Add(item);


            result = Tizen.System.Information.TryGetValue<string>("http://tizen.org/system/platform.processor", out valuestring);
            if (result)
                item = SettingItemCreator.CreateItemWithCheck("CPU", valuestring);
            else
                item = SettingItemCreator.CreateItemWithCheck("CPU", Resources.IDS_ST_HEADER_UNAVAILABLE);
            content.Add(item);

            var memusage = new Tizen.System.SystemMemoryUsage();
            float ram_total_gb = memusage.Total / 1000000.0F;
            item = SettingItemCreator.CreateItemWithCheck("RAM", String.Format("{0:0.0} GB",ram_total_gb));
            content.Add(item);

            bool result1 = Tizen.System.Information.TryGetValue<int>("http://tizen.org/feature/screen.width", out int screenwidth);
            bool result2 = Tizen.System.Information.TryGetValue<int>("http://tizen.org/feature/screen.height", out int screenheight);

            if (result1 && result2)
                item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_RESOLUTION, String.Format("{0:d} x {1:d}", screenwidth, screenheight));
            else
                item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_RESOLUTION, Resources.IDS_ST_HEADER_UNAVAILABLE);
            content.Add(item);

            item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_STATUS, Resources.IDS_ST_BODY_SHOW_NETWORK_STATUS_AND_OTHER_INFORMATION);
            if (item != null)
            {

                item.Clicked += (o, e) =>
                {
                    RequestWidgetPush("devicestatus@org.tizen.cssettings");
                };
                content.Add(item);
            }

            return content;
        }
        private void SystemSettings_DeviceNameChanged(object sender, Tizen.System.DeviceNameChangedEventArgs e)
        {
            Tizen.Log.Debug("NUI", "SystemSettings_TimeChanged is called\n");

            String name = Vconf.GetString("db/setting/device_name");
            mDevicenameItem.SubText = name;
        }

        protected override void OnCreate(string contentInfo, Window window)
        {
            base.OnCreate(contentInfo, window);
            Tizen.System.SystemSettings.DeviceNameChanged += SystemSettings_DeviceNameChanged;
        }
        protected override void OnTerminate(string contentInfo, TerminationType type)
        {
            Tizen.System.SystemSettings.DeviceNameChanged -= SystemSettings_DeviceNameChanged;

            base.OnTerminate(contentInfo, type);
        }
    }
}
