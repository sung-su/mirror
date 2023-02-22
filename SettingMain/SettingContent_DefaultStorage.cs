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

﻿using System;
using System.Collections.Generic; // for Dictionary
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.Applications;

using Tizen.System;

using SettingAppTextResopurces.TextResources;

namespace SettingMain
{
    class SettingContent_DefaultStorage : SettingContent_Base
    {

        private DefaultLinearItem mShaedContentItem = null;
        private DefaultLinearItem mAppInstallItem = null;

        public SettingContent_DefaultStorage()
            : base()
        {
            mTitle = Resources.IDS_SM_TMBODY_DEFAULT_STORAGE_LOCATIONS;
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




            // VCONFKEY_SYSMAN_MMC_STATUS
            // 0 : VCONFKEY_SYSMAN_MMC_REMOVED
            // 1 : VCONFKEY_SYSMAN_MMC_MOUNTED
            // 2 : VCONFKEY_SYSMAN_MMC_INSERTED_NOT_MOUNTED
             int MMCStatus = Vconf.GetInt("memory/sysman/mmc"); 


            // Share Content : VCONFKEY_SETAPPL_DEFAULT_MEM_WIFI_DIRECT_INT => "db/setting/default_memory/wifi_direct"
            // App Installation : VCONFKEY_SETAPPL_DEFAULT_MEM_INSTALL_APPLICATIONS_INT => "db/setting/default_memory/install_applications" 

            const string key_SharedContent = "db/setting/default_memory/wifi_direct";
            const string key_AppInstall = "db/setting/default_memory/install_applications";

            int defstorage_SharedContent = Vconf.GetInt(key_SharedContent);
            int defstorage_AppInstall = Vconf.GetInt(key_AppInstall);


            DefaultLinearItem item = null;


            //content.Add(SettingItemCreator.CreateItemStatic(Resources.IDS_ST_HEADER_SHARED_CONTENT));
            content.Add(SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_HEADER_SHARED_CONTENT));
            item = SettingItemCreator.CreateItemWithCheck("",
                Resources.IDS_SM_BODY_SELECT_THE_DEFAULT_STORAGE_LOCATION_FOR_CONTENT_SHARED_VIA_BLUETOOTH_OR_WI_FI_DIRECT);
            if (item != null)
            {
                item.SubLabel.MultiLine = true;
                content.Add(item);
            }
            item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_STORAGE, null, false, true);
            mShaedContentItem = item;
            if (item != null)
            {
                item.Padding = new Extents(50,0,0,0);

                var toggle = item.Extra as Switch;
                if (defstorage_SharedContent == 0)
                {
                    toggle.IsSelected = true;
                    item.Text = Resources.IDS_ST_BODY_STORAGE;
                }
                else {
                    toggle.IsSelected = false;
                    item.Text = Resources.IDS_ST_BODY_SD_CARD;
                }
                

                toggle.SelectedChanged += (o, e) =>
                {
                    if (e.IsSelected)
                    {
                        Vconf.SetInt(key_SharedContent, 0);
                        Tizen.Log.Debug("NUI", "Def. storage of Shared Content is Storage!");

                        if(mShaedContentItem)
                            mShaedContentItem.Text = Resources.IDS_ST_BODY_STORAGE;
                    }
                    else
                    {
                        Vconf.SetInt(key_SharedContent, 1);
                        Tizen.Log.Debug("NUI", "Def. storage of Shared Content is SD Card");

                        if (mShaedContentItem)
                            mShaedContentItem.Text = Resources.IDS_ST_BODY_SD_CARD;
                    }
                };

                if (MMCStatus != 1)
                    item.IsEnabled = false;

                content.Add(item);
            }


            content.Add(SettingItemCreator.CreateItemStatic(""));

            //content.Add(SettingItemCreator.CreateItemStatic(Resources.IDS_ST_HEADER_APP_INSTALLATION));
            content.Add(SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_HEADER_APP_INSTALLATION));
            item = SettingItemCreator.CreateItemWithCheck("",
                Resources.IDS_SM_BODY_SELECT_THE_DEFAULT_LOCATION_FOR_INSTALLING_APPS_WHERE_APPS_CAN_BE_SAVED_DEPENDS_ON_THE_TYPE_OF_APP_AND_THE_AVAILABILITY_OF_THE_LOCATION);
            if (item != null)
            {
                item.SubLabel.MultiLine = true;
                content.Add(item);
            }
            item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_STORAGE, null, false, true);
            mAppInstallItem = item;
            if (item != null)
            {
                item.Padding = new Extents(50, 0, 0, 0);

                var toggle = item.Extra as Switch;
                if (defstorage_AppInstall == 0)
                {
                    toggle.IsSelected = true;
                    item.Text = Resources.IDS_ST_BODY_STORAGE;
                }
                else
                {
                    toggle.IsSelected = false;
                    item.Text = Resources.IDS_ST_BODY_SD_CARD;
                }


                toggle.SelectedChanged += (o, e) =>
                {
                    if (e.IsSelected)
                    {
                        Vconf.SetInt(key_AppInstall, 0);
                        Tizen.Log.Debug("NUI", "Def. storage of Shared Content is Storage!");

                        if (mAppInstallItem)
                            mAppInstallItem.Text = Resources.IDS_ST_BODY_STORAGE;
                    }
                    else
                    {
                        Vconf.SetInt(key_AppInstall, 1);
                        Tizen.Log.Debug("NUI", "Def. storage of Shared Content is SD Card");

                        if (mAppInstallItem)
                            mAppInstallItem.Text = Resources.IDS_ST_BODY_SD_CARD;
                    }
                };

                if (MMCStatus != 1)
                    item.IsEnabled = false;

                content.Add(item);
            }

            return content;
        }
    }
}
