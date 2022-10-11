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
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.Applications;

using SettingAppTextResopurces.TextResources;

namespace SettingView
{
    public class Program : NUIApplication
    {

        private static readonly string resPath = Tizen.Applications.Application.Current.DirectoryInfo.Resource;
        protected const string SETTING_LIST_ICON_PATH_CFG = "/icons/list_icon/";

        private ContentPage mMainPage;

        public Program(string styleSheet, Size2D windowSize, Position2D windowPosition, IBorderInterface borderInterface)
            : base(styleSheet, windowSize, windowPosition, borderInterface)
        {
            mMainPage = null;
        }
        protected override void OnCreate()
        {
            base.OnCreate();

            Window window = GetDefaultWindow();
            window.KeyEvent += OnKeyEvent;
            window.TouchEvent += OnTouchEvent;

            // Page with AppBar and Content.
            var appBar = new AppBar()
            {
                Title = Resources.IDS_ST_OPT_SETTINGS,
            };
            var appBarStyle = ThemeManager.GetStyle("Tizen.NUI.Components.AppBar");
            var navigationContent = new Button(((AppBarStyle)appBarStyle).BackButton);
            navigationContent.Clicked += (o, e) =>
            {
                Exit();
            };
            appBar.NavigationContent = navigationContent;
            //appBarStyle.Dispose();

            mMainPage = new ContentPage()
            {
                AppBar = appBar,

                Content = CreateMainMenuContent(),
            };

            // Push the page to the default navigator.
            window.GetDefaultNavigator().Push(mMainPage);


            Tizen.System.SystemSettings.LocaleLanguageChanged += SystemSettings_LocaleLanguageChanged;
        }

        protected override void OnTerminate()
        {

            Window window = GetDefaultWindow();

            window.KeyEvent -= OnKeyEvent;
            window.TouchEvent -= OnTouchEvent;


            Tizen.System.SystemSettings.LocaleLanguageChanged -= SystemSettings_LocaleLanguageChanged;


            // Create items and add them to the content of the page.
            SettingMenuManager.ClearMenuList();
            base.OnTerminate();
        }

        private void SystemSettings_LocaleLanguageChanged(object sender, Tizen.System.LocaleLanguageChangedEventArgs e)
        {
            if (mMainPage != null) {
                mMainPage.AppBar.Title = Resources.IDS_ST_OPT_SETTINGS;
                mMainPage.Content = CreateMainMenuContent();
            }
        }


        public void OnKeyEvent(object sender, Window.KeyEventArgs e)
        {
            if (e.Key.State == Key.StateType.Down && (e.Key.KeyPressedName == "XF86Back" || e.Key.KeyPressedName == "Escape"))
            {
                Exit();
            }
        }
        private void OnTouchEvent(object source, Window.TouchEventArgs e)
        {
        }

        private SettingMenuInfo[] MakeMenuList()
        {
            SettingMenuInfo[] menulist = new SettingMenuInfo[13];

            // menulist[0] = new SettingMenuInfo(Resources.IDS_ST_BODY_WI_FI, SettingLaunchType.Widget, "wifi@org.tizen.cssetting-wifi", resPath + SETTING_LIST_ICON_PATH_CFG + "settings_wifi.png");
            menulist[0] = new SettingMenuInfo(Resources.IDS_ST_BODY_WI_FI, SettingLaunchType.Application, "wifi-efl-ug", resPath + SETTING_LIST_ICON_PATH_CFG + "settings_wifi.png");
            // menulist[1] = new SettingMenuInfo(Resources.IDS_TPLATFORM_OPT_BLUETOOTH, SettingLaunchType.Widget, "bluetooth@org.tizen.cssetting-bluetooth", resPath + SETTING_LIST_ICON_PATH_CFG + "settings_bluetooth.png");
            menulist[1] = new SettingMenuInfo(Resources.IDS_TPLATFORM_OPT_BLUETOOTH, SettingLaunchType.Application, "ug-bluetooth-efl", resPath + SETTING_LIST_ICON_PATH_CFG + "settings_bluetooth.png");

            menulist[2] = new SettingMenuInfo(Resources.IDS_ST_HEADER_SOUND, SettingLaunchType.Widget, "sound@org.tizen.cssettings", resPath + SETTING_LIST_ICON_PATH_CFG + "settings_sound_and_notifications.png");
            menulist[3] = new SettingMenuInfo(Resources.IDS_ST_HEADER_DISPLAY, SettingLaunchType.Widget, "display@org.tizen.cssettings", resPath + SETTING_LIST_ICON_PATH_CFG + "settings_display.png");
            menulist[4] = new SettingMenuInfo(Resources.IDS_LCKSCN_BODY_WALLPAPERS, SettingLaunchType.Widget, "wallpaper@org.tizen.cssetting-wallpaper", resPath + SETTING_LIST_ICON_PATH_CFG + "settings_wallpapers.png");
            menulist[5] = new SettingMenuInfo(Resources.IDS_ST_BODY_ACCOUNTS, SettingLaunchType.Widget, "account@org.tizen.cssetting-account", resPath + SETTING_LIST_ICON_PATH_CFG + "settings_account.png");
            menulist[6] = new SettingMenuInfo(Resources.IDS_ST_HEADER_PRIVACY, SettingLaunchType.Widget, "privacy@org.tizen.cssetting-privacy", resPath + SETTING_LIST_ICON_PATH_CFG + "settings_privacy_and_safety.png");
            menulist[7] = new SettingMenuInfo(Resources.IDS_ST_BODY_APPLICATIONS, SettingLaunchType.Widget, "apps@org.tizen.cssettings", resPath + SETTING_LIST_ICON_PATH_CFG + "settings_applications.png");
            menulist[8] = new SettingMenuInfo(Resources.IDS_ST_BODY_STORAGE, SettingLaunchType.Widget, "storage@org.tizen.cssettings", resPath + SETTING_LIST_ICON_PATH_CFG + "settings_storage.png");
            menulist[9] = new SettingMenuInfo(Resources.IDS_ST_HEADER_LANGUAGE_AND_INPUT, SettingLaunchType.Widget, "languageinput@org.tizen.cssettings", resPath + SETTING_LIST_ICON_PATH_CFG + "settings_language_and_input.png");
            menulist[10] = new SettingMenuInfo(Resources.IDS_ST_BODY_DATE_AND_TIME, SettingLaunchType.Widget, "datetime@org.tizen.cssettings", resPath + SETTING_LIST_ICON_PATH_CFG + "settings_date_and_time.png");
            menulist[11] = new SettingMenuInfo(Resources.IDS_ST_BODY_ACCESSIBILITY, SettingLaunchType.Widget, "accessibility@org.tizen.cssetting-accessibility", resPath + SETTING_LIST_ICON_PATH_CFG + "settings_accessibility.png");
            menulist[12] = new SettingMenuInfo(Resources.IDS_ST_BODY_ABOUT_DEVICE, SettingLaunchType.Widget, "aboutdevice@org.tizen.cssettings", resPath + SETTING_LIST_ICON_PATH_CFG + "settings_about_device.png");

            return menulist;
        }

        // Create a page with scrollable content
        private View CreateMainMenuContent()
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

            // Create items and add them to the content of the page.
#if false
            SettingMenuInfo[] menulist = MakeMenuList();

#if false
            // codes to generate .menulist file
            string datapath = Tizen.Applications.Application.Current.DirectoryInfo.Data;
            SettingMenuManager.WriteMenuList(menulist, datapath, "settingmain");
#endif

#else
            SettingMenuInfo[] menulist = SettingMenuManager.ReadMenuList(resPath + "/menu", "settingmain");
#endif
            if (menulist != null)
                SettingMenuManager.BuildMenuList(content, menulist, GetDefaultWindow());

            return content;
        }

        static void Main(string[] args)
        {
            var appCustomBorder = new SettingViewBorder();
            var app = new Program("", new Size2D(800, 500), new Position2D(300, 50), appCustomBorder);

            app.Run(args);
        }
    }
}
