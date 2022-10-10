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

using System.Collections.ObjectModel;

using SettingAppTextResopurces.TextResources;

namespace SettingView
{
    public class WidgetViewInfo
    {
        private string Id;
        private WidgetView View;


        public WidgetViewInfo(string id, WidgetView view)
        {
            Id = id;
            View = view;
        }


        public string GetId()
        {
            return Id;
        }

        public WidgetView GetView()
        {
            return View;
        }
    };

    public class Program : NUIApplication
    {
        private string LastestPushWidgetId = "";

#if false
        private Collection<WidgetViewInfo> mWidgetViewPool;
#endif

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

#if false
            mWidgetViewPool = new Collection<WidgetViewInfo>();
#endif


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


#if false
            mWidgetViewPool.Clear();
#endif
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

        private void OnWidgetContentUpdatedCB(object sender, WidgetView.WidgetViewEventArgs e)
        {
            String encodedBundle = e.WidgetView.ContentInfo;
            Bundle bundle = Bundle.Decode(encodedBundle);

            if (bundle.TryGetItem("WIDGET_ID", out string widgetID))
            {
                Tizen.Log.Debug("NUI", "WIDGET_ID!\n");
            }

            if (bundle.TryGetItem("WIDGET_WIDTH", out string widgetWidth))
            {
                Tizen.Log.Debug("NUI", "WIDGET_WIDTH!\n");
            }

            if (bundle.TryGetItem("WIDGET_HEIGHT", out string widgetHeight))
            {
                Tizen.Log.Debug("NUI", "WIDGET_HEIGHT!\n");
            }

            if (bundle.TryGetItem("WIDGET_PAGE", out string widgetPage))
            {
                Tizen.Log.Debug("NUI", "WIDGET_PAGE!\n");
            }

            if (bundle.TryGetItem("APP_ID", out string appID))
            {
                Tizen.Log.Debug("NUI", "APP_ID!\n");
            }

            if (bundle.TryGetItem("WIDGET_ACTION", out string widgetAction))
            {
                if (widgetAction.Equals("ADD"))
                {
                    Tizen.Log.Debug("NUI", "WIDGET_ACTION : ADD!\n");
#if false
                    if (Int32.TryParse(widgetWidth, out int width) && Int32.TryParse(widgetHeight, out int height))
                    {
                        Bundle bundle2 = new Bundle();
                        bundle2.AddItem(" ", " ");
                        String encodedBundle2 = bundle2.Encode();

                        if (widgetID.Equals("secondPage@NUISettingsReset"))
                        {
                            secondPageWidgetView = WidgetViewManager.Instance.AddWidget(widgetID, encodedBundle2, width, height, 0.0f);
                            secondPageWidgetView.WidgetContentUpdated += OnWidgetContentUpdatedCB;
                        }
                        else if (widgetID.Equals("thirdPage@NUISettingsReset"))
                        {
                            thirdPageWidgetView = WidgetViewManager.Instance.AddWidget(widgetID, encodedBundle2, width, height, 0.0f);
                            thirdPageWidgetView.WidgetContentUpdated += OnWidgetContentUpdatedCB;
                        }
                        else if (widgetID.Equals("fourthPage@NUISettingsReset"))
                        {
                            fourthPageWidgetView = WidgetViewManager.Instance.AddWidget(widgetID, encodedBundle2, width, height, 0.0f);
                            fourthPageWidgetView.WidgetContentUpdated += OnWidgetContentUpdatedCB;
                        }
                    }
#endif
                }
                else if (widgetAction.Equals("PUSH"))
                {
                    Tizen.Log.Debug("NUI", "WIDGET_ACTION : PUSH!\n");

                    if (Int32.TryParse(widgetWidth, out int width) && Int32.TryParse(widgetHeight, out int height))
                    {
                        Bundle bundle2 = new Bundle();
                        bundle2.AddItem(" ", " ");
                        String encodedBundle2 = bundle2.Encode();

                        WidgetView widgetview = WidgetViewManager.Instance.AddWidget(widgetID, encodedBundle2, width, height, 0.0f);
                        if (widgetview != null)
                        {
                            widgetview.WidgetContentUpdated += OnWidgetContentUpdatedCB;
                            widgetview.Preview = false;
                            NUIApplication.GetDefaultWindow().GetDefaultNavigator().Push(new ContentPage() { Content = widgetview });
                        }
                    }
#if false
                    if (widgetPage.Equals("CONTENT_PAGE"))
                    {
                        if (widgetID.Equals("secondPage@NUISettingsReset"))
                        {
                            NUIApplication.GetDefaultWindow().GetDefaultNavigator().Push(new ContentPage() { Content = secondPageWidgetView });
                        }
                        else if (widgetID.Equals("thirdPage@NUISettingsReset"))
                        {
                            NUIApplication.GetDefaultWindow().GetDefaultNavigator().Push(new ContentPage() { Content = thirdPageWidgetView });
                        }
                        else if (widgetID.Equals("fourthPage@NUISettingsReset"))
                        {
                            NUIApplication.GetDefaultWindow().GetDefaultNavigator().Push(new ContentPage() { Content = fourthPageWidgetView });
                        }
                    }
                    else if (widgetPage.Equals("DIALOG_PAGE"))
                    {
                        if (widgetID.Equals("secondPage@NUISettingsReset"))
                        {
                            NUIApplication.GetDefaultWindow().GetDefaultNavigator().Push(new DialogPage() { Content = secondPageWidgetView });
                        }
                        else if (widgetID.Equals("thirdPage@NUISettingsReset"))
                        {
                            NUIApplication.GetDefaultWindow().GetDefaultNavigator().Push(new DialogPage() { Content = thirdPageWidgetView });
                        }
                        else if (widgetID.Equals("fourthPage@NUISettingsReset"))
                        {
                            NUIApplication.GetDefaultWindow().GetDefaultNavigator().Push(new DialogPage() { Content = fourthPageWidgetView });
                        }
                    }
#endif
                }
                else if (widgetAction.Equals("POP"))
                {
                    Tizen.Log.Debug("NUI", "WIDGET_ACTION : POP!\n");
                    NUIApplication.GetDefaultWindow().GetDefaultNavigator().Pop();

                    LastestPushWidgetId = "";
                }
                else if (widgetAction.Equals("LAUNCH"))
                {
                    Tizen.Log.Debug("NUI", "WIDGET_ACTION : LAUNCH!\n");
                    LaunchApplication(appID);

                    LastestPushWidgetId = "";
                }
        }
    }



        public class SettingMenuInfo
        {
            private string Name;
            private string WidgetId;
            private string IconPath;


            public SettingMenuInfo(string name, string widgetid, string iconpath)
            {
                Name = name;
                WidgetId = widgetid;
                IconPath = iconpath;
            }

            public string GetName()
            {
                return Name;
            }
            public string GetWidgetId()
            {
                return WidgetId;
            }
            public string GetIconPath()
            {
                return IconPath;
            }
        };

        private static SettingMenuInfo[] SettingMenuList =
        {
            new SettingMenuInfo(Resources.IDS_ST_HEADER_SOUND, "sound@org.tizen.cssettings", resPath + SETTING_LIST_ICON_PATH_CFG + "settings_sound_and_notifications.png"),
            new SettingMenuInfo(Resources.IDS_ST_HEADER_DISPLAY, "display@org.tizen.cssettings", resPath + SETTING_LIST_ICON_PATH_CFG + "settings_display.png"),
            new SettingMenuInfo(Resources.IDS_LCKSCN_BODY_WALLPAPERS, "wallpaper@org.tizen.cssetting-wallpaper", resPath + SETTING_LIST_ICON_PATH_CFG + "settings_wallpapers.png"),
            new SettingMenuInfo(Resources.IDS_ST_BODY_ACCOUNTS, "account@org.tizen.cssetting-account", resPath + SETTING_LIST_ICON_PATH_CFG + "settings_account.png"),
            new SettingMenuInfo(Resources.IDS_ST_HEADER_PRIVACY, "privacy@org.tizen.cssetting-privacy", resPath + SETTING_LIST_ICON_PATH_CFG + "settings_privacy_and_safety.png"),
            new SettingMenuInfo(Resources.IDS_ST_BODY_APPLICATIONS, "apps@org.tizen.cssettings", resPath + SETTING_LIST_ICON_PATH_CFG + "settings_applications.png"),
            new SettingMenuInfo(Resources.IDS_ST_BODY_STORAGE, "storage@org.tizen.cssettings", resPath + SETTING_LIST_ICON_PATH_CFG + "settings_storage.png"),
            new SettingMenuInfo(Resources.IDS_ST_HEADER_LANGUAGE_AND_INPUT, "languageinput@org.tizen.cssettings", resPath + SETTING_LIST_ICON_PATH_CFG + "settings_language_and_input.png"),
            new SettingMenuInfo(Resources.IDS_ST_BODY_DATE_AND_TIME, "datetime@org.tizen.cssettings", resPath + SETTING_LIST_ICON_PATH_CFG + "settings_date_and_time.png"),
            new SettingMenuInfo(Resources.IDS_ST_BODY_ACCESSIBILITY, "accessibility@org.tizen.cssetting-accessibility", resPath + SETTING_LIST_ICON_PATH_CFG + "settings_accessibility.png"),
            new SettingMenuInfo(Resources.IDS_ST_BODY_ABOUT_DEVICE, "aboutdevice@org.tizen.cssettings", resPath + SETTING_LIST_ICON_PATH_CFG + "settings_about_device.png")
        };

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
            DefaultLinearItem item;
            item = SettingItemCreator.CreateItemWithIcon(Resources.IDS_ST_BODY_WI_FI, resPath + SETTING_LIST_ICON_PATH_CFG + "settings_wifi.png");
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
#if false
                Window window = GetDefaultWindow();
                LaunchWidget(window, "wifi@org.tizen.cssetting-wifi");
#else
                    LaunchApplication("wifi-efl-ug");
#endif
                };
                content.Add(item);
            }
            item = SettingItemCreator.CreateItemWithIcon(Resources.IDS_TPLATFORM_OPT_BLUETOOTH, resPath + SETTING_LIST_ICON_PATH_CFG + "settings_bluetooth.png");
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
#if false
                Window window = GetDefaultWindow();
                LaunchWidget(window, "bluetooth@org.tizen.cssetting-bluetooth");
#else
                    LaunchApplication("ug-bluetooth-efl");
#endif
                };
                content.Add(item);
            }



            BuildMenuList(content, SettingMenuList);


            return content;

        }





        /////////////////////////////////////////////////////////////
        /// Build MenuList with Table

        void BuildMenuList(View content, SettingMenuInfo[] menulist)
        {
            DefaultLinearItem item;

            foreach (var menu in menulist) 
            {
                item = SettingItemCreator.CreateItemWithIcon(menu.GetName(), menu.GetIconPath());
                if (item != null)
                {
                    item.Clicked += (o, e) =>
                    {
                        Window window = GetDefaultWindow();
                        LaunchWidget(window, menu.GetWidgetId());
                    };
                    content.Add(item);
                }
            }
        }


        /////////////////////////////////////////////////////////////
        /// Widget Operations

        void LaunchWidget(Window window, string widgetid)
        {
            if (LastestPushWidgetId.Equals(widgetid))
            {
                    Tizen.Log.Debug("NUI", "LastestPushWidgetId : "+ LastestPushWidgetId);
                    return;
            }

            Navigator navigator = window.GetDefaultNavigator();

            Bundle bundle = new Bundle();
            bundle.AddItem(" ", " ");
            String encodedBundle = bundle.Encode();

#if false
            WidgetView widgetview = null;
            // find widgetview in mWidgetViewPool
            foreach (var info in mWidgetViewPool)
            {
                if (info.GetId().Equals(widgetid)) {
                        widgetview = info.GetView();
                }
            }
            if (widgetview == null)
                widgetview = WidgetViewManager.Instance.AddWidget(widgetid, encodedBundle, window.Size.Width, window.Size.Height, 0.0f);
#else
            WidgetView widgetview = WidgetViewManager.Instance.AddWidget(widgetid, encodedBundle, window.Size.Width, window.Size.Height, 0.0f);
#endif

            if (widgetview != null)
            {
                if (string.IsNullOrEmpty(widgetview.InstanceID))
                {
                    Tizen.Log.Debug("NUI", widgetid+" is not installed!!");

                    widgetview.Dispose();
                }
                else
                {
                    Tizen.Log.Debug("NUI", String.Format($"widget launch : {0}\n", widgetid));

                    widgetview.Position = new Position(0, 0);
                    widgetview.WidgetContentUpdated += OnWidgetContentUpdatedCB;
                    widgetview.Preview = false;

                    var page = new ContentPage
                    {
                        Content = widgetview
                    };
                    navigator.Push(page);

                    LastestPushWidgetId = widgetid;
#if false
                    mWidgetViewPool.Add(new WidgetViewInfo(widgetid, widgetview));
#endif
                }
            }
        }

        void LaunchApplication(string appid)
        {
            AppControl appcontrol = new AppControl()
            {
                Operation = AppControlOperations.Default,
                ApplicationId = appid,
                LaunchMode = AppControlLaunchMode.Group,
            };
            AppControl.SendLaunchRequest(appcontrol);
        }

        static void Main(string[] args)
        {
            var appCustomBorder = new SettingViewBorder();
            var app = new Program("", new Size2D(800, 500), new Position2D(300, 50), appCustomBorder);

            app.Run(args);
        }
    }
}
