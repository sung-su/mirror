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

            base.OnTerminate();
        }

        private void SystemSettings_LocaleLanguageChanged(object sender, Tizen.System.LocaleLanguageChangedEventArgs e)
        {
            if (mMainPage != null) {
                mMainPage.AppBar.Title = Resources.IDS_ST_OPT_SETTINGS;
                mMainPage.Content = CreateMainMenuContent();
            }
        }


        void PushContentPage(string title, View content)
        {
            Window window = GetDefaultWindow();
            Navigator navigator = window.GetDefaultNavigator();

            window.KeyEvent += OnKeyEvent;
            window.TouchEvent += OnTouchEvent;

            var page = new ContentPage()
            {
                AppBar = new AppBar()
                {
                    Title = title,
                },
            };

            page.Content = content;
            navigator.Push(page);
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

            string widgetID;
            if (bundle.TryGetItem("WIDGET_ID", out widgetID))
            {
                Tizen.Log.Debug("NUI", "WIDGET_ID!\n");
            }

            string widgetWidth;
            if (bundle.TryGetItem("WIDGET_WIDTH", out widgetWidth))
            {
                Tizen.Log.Debug("NUI", "WIDGET_WIDTH!\n");
            }

            string widgetHeight;
            if (bundle.TryGetItem("WIDGET_HEIGHT", out widgetHeight))
            {
                Tizen.Log.Debug("NUI", "WIDGET_HEIGHT!\n");
            }

            string widgetPage;
            if (bundle.TryGetItem("WIDGET_PAGE", out widgetPage))
            {
                Tizen.Log.Debug("NUI", "WIDGET_PAGE!\n");
            }

            string widgetAction;
            if (bundle.TryGetItem("WIDGET_ACTION", out widgetAction))
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
                }
            }
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
            DefaultLinearItem item = null;
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

            item = SettingItemCreator.CreateItemWithIcon(Resources.IDS_ST_HEADER_SOUND, resPath + SETTING_LIST_ICON_PATH_CFG + "settings_sound_and_notifications.png");
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    Window window = GetDefaultWindow();
                    LaunchWidget(window, "sound@org.tizen.cssettings");
                };
                content.Add(item);
            }

#if false
        item = SettingItemCreator.CreateItemWithIcon(Resources.IDS_ST_BODY_NOTIFICATIONS, resPath + SETTING_LIST_ICON_PATH_CFG + "settings_notifications.png");
        if (item != null)
        {
            content.Add(item);
        }
#endif


            item = SettingItemCreator.CreateItemWithIcon(Resources.IDS_ST_HEADER_DISPLAY, resPath + SETTING_LIST_ICON_PATH_CFG + "settings_display.png");
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    Window window = GetDefaultWindow();
                    LaunchWidget(window, "display@org.tizen.cssettings");
                };
                content.Add(item);
            }

            item = SettingItemCreator.CreateItemWithIcon(Resources.IDS_LCKSCN_BODY_WALLPAPERS, resPath + SETTING_LIST_ICON_PATH_CFG + "settings_wallpapers.png");
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    Window window = GetDefaultWindow();
                    LaunchWidget(window, "wallpaper@org.tizen.cssetting-wallpaper");
                };
                content.Add(item);
            }

#if false
        item = SettingItemCreator.CreateItemWithIcon("Tray", resPath + SETTING_LIST_ICON_PATH_CFG + "settings_softkey.png");
        content.Add(item);
        item = SettingItemCreator.CreateItemWithIcon("Screen Mirroring", resPath + SETTING_LIST_ICON_PATH_CFG + "settings_softkey.png");
        content.Add(item);
#endif
            item = SettingItemCreator.CreateItemWithIcon(Resources.IDS_ST_BODY_ACCOUNTS, resPath + SETTING_LIST_ICON_PATH_CFG + "settings_account.png");
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    Window window = GetDefaultWindow();
                    LaunchWidget(window, "account@org.tizen.cssetting-account");
                };
                content.Add(item);
            }

            item = SettingItemCreator.CreateItemWithIcon(Resources.IDS_ST_HEADER_PRIVACY, resPath + SETTING_LIST_ICON_PATH_CFG + "settings_privacy_and_safety.png");
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    Window window = GetDefaultWindow();
                    LaunchWidget(window, "privacy@org.tizen.cssetting-privacy");
                };
                content.Add(item);
            }

            item = SettingItemCreator.CreateItemWithIcon(Resources.IDS_ST_BODY_APPLICATIONS, resPath + SETTING_LIST_ICON_PATH_CFG + "settings_applications.png");
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    Window window = GetDefaultWindow();
                    LaunchWidget(window, "apps@org.tizen.cssettings");
                };
                content.Add(item);
            }

            item = SettingItemCreator.CreateItemWithIcon(Resources.IDS_ST_BODY_STORAGE, resPath + SETTING_LIST_ICON_PATH_CFG + "settings_storage.png");
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    Window window = GetDefaultWindow();
                    LaunchWidget(window, "storage@org.tizen.cssettings");
                };
                content.Add(item);
            }

            item = SettingItemCreator.CreateItemWithIcon(Resources.IDS_ST_HEADER_LANGUAGE_AND_INPUT, resPath + SETTING_LIST_ICON_PATH_CFG + "settings_language_and_input.png");
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    Window window = GetDefaultWindow();
                    LaunchWidget(window, "languageinput@org.tizen.cssettings");
                };
                content.Add(item);
            }

            item = SettingItemCreator.CreateItemWithIcon(Resources.IDS_ST_BODY_DATE_AND_TIME, resPath + SETTING_LIST_ICON_PATH_CFG + "settings_date_and_time.png");
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    Window window = GetDefaultWindow();
                    LaunchWidget(window, "datetime@org.tizen.cssettings");
                };
                content.Add(item);
            }

            item = SettingItemCreator.CreateItemWithIcon(Resources.IDS_ST_BODY_ACCESSIBILITY, resPath + SETTING_LIST_ICON_PATH_CFG + "settings_accessibility.png");
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    Window window = GetDefaultWindow();
                    LaunchWidget(window, "accessibility@org.tizen.cssetting-accessibility");
                };
                content.Add(item);
            }

            item = SettingItemCreator.CreateItemWithIcon(Resources.IDS_ST_BODY_ABOUT_DEVICE, resPath + SETTING_LIST_ICON_PATH_CFG + "settings_about_device.png");
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    Window window = GetDefaultWindow();
                    LaunchWidget(window, "aboutdevice@org.tizen.cssettings");
                };
                content.Add(item);
            }

            return content;

        }

    void LaunchWidget(Window window, string widgetid)
    {
        Navigator navigator = window.GetDefaultNavigator();

        Bundle bundle = new Bundle();
        bundle.AddItem(" ", " ");
        String encodedBundle = bundle.Encode();

        WidgetView widgetview = WidgetViewManager.Instance.AddWidget(widgetid, encodedBundle, window.Size.Width, window.Size.Height, 0.0f);
        if (widgetview != null)
        {
                if (string.IsNullOrEmpty(widgetview.InstanceID))
                {
                    Tizen.Log.Debug("NUI", widgetid+" is not installed!!");
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
            var app = new Program("", new Size2D(800, 400), new Position2D(300, 100), appCustomBorder);

            app.Run(args);
        }
    }
}
