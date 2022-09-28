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

        public Program(string styleSheet, Size2D windowSize, Position2D windowPosition, IBorderInterface borderInterface)
            : base(styleSheet, windowSize, windowPosition, borderInterface)
        {
        }
        protected override void OnCreate()
        {
            base.OnCreate();
            Initialize();
        }

        void Initialize()
        {

            Window window = GetDefaultWindow();
            window.BackgroundColor = Color.Blue;
            window.KeyEvent += OnKeyEvent;
            window.TouchEvent += OnTouchEvent;

            Bundle bundle = new Bundle();
            bundle.AddItem(" ", " ");
            String encodedBundle = bundle.Encode();

            Tizen.Log.Error("SettingWidget", "REQUEST \n");
#if false
            // Add Widget in advance to avoid loading pending.
            mWidgetView = WidgetViewManager.Instance.AddWidget("main@org.tizen.cssettings", encodedBundle, window.Size.Width, window.Size.Height, 0.0f);
            mWidgetView.Position = new Position(0, 0);
            mWidgetView.WidgetContentUpdated += OnWidgetContentUpdatedCB;

            //window.GetDefaultLayer().Add(mWidgetView);





            var button = new Button()
            {
                Text = "Click to Second Page",
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
            };
            button.Clicked += (o, e) =>
            {
                //  var page = new ContentPage();
                //  page.Content = secondPageWidgetView;
                //navigator.Push(page);
            };

            // Push the first page.
            //PushContentPage("First Page", button);
#endif

            CreateSettingsMainMenu();


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
                        widgetview.WidgetContentUpdated += OnWidgetContentUpdatedCB;
                        widgetview.Preview = false;
                        NUIApplication.GetDefaultWindow().GetDefaultNavigator().Push(new ContentPage() { Content = widgetview });
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






        // Create an list item with checkbox.
        private DefaultLinearItem CreateItemWithCheck(string text, string subText = null, bool icon = false, bool extra = false)
    {
        var item = new DefaultLinearItem()
        {
            WidthSpecification = LayoutParamPolicies.MatchParent,
            Text = text,
            IsSelectable = false, // Item should not be remained as selected state.
        };

        if (subText != null)
        {
            item.SubText = subText;
        }

        CheckBox check = null;
        if (icon)
        {
            check = new CheckBox();
            check.SelectedChanged += (o, e) =>
            {
                if (e.IsSelected)
                {
                    Tizen.Log.Debug("NUI", "check is selected!\n");
                }
                else
                {
                    Tizen.Log.Debug("NUI", "check is unselected!\n");
                }
            };
            // Icon is placed at the beginning(left end) of the item.
            item.Icon = check;

            // Do not propagate Pressed/Selected states from item to item.Icon.
            // When item is pressed/clicked/selected, item.Icon is not pressed/clicked/selected.
            item.Icon.PropagatableControlStates = ControlState.Disabled;
        }

        Switch toggle = null;
        if (extra)
        {
            toggle = new Switch();
            toggle.SelectedChanged += (o, e) =>
            {
                if (e.IsSelected)
                {
                    Tizen.Log.Debug("NUI", "toggle is selected!\n");
                }
                else
                {
                    Tizen.Log.Debug("NUI", "toggle is unselected!\n");
                }
            };
            // Extra is placed at the end(right end) of the item.
            item.Extra = toggle;

            // Do not propagate Pressed/Selected states from item to item.Extra.
            // When item is pressed/clicked/selected, item.Extra is not pressed/clicked/selected.
            item.Extra.PropagatableControlStates = ControlState.Disabled;
        }

        item.Clicked += (o, e) =>
        {
            if (check != null)
            {
                check.IsSelected = !check.IsSelected;
            }

            if (toggle != null)
            {
                toggle.IsSelected = !toggle.IsSelected;
            }

            Tizen.Log.Debug("NUI", "item is clicked!\n");
        };



        return item;
    }

    // Create an list item  with icon
    private DefaultLinearItem CreateItemWithIcon(string text, string iconpath, string subText = null, bool extra = false)
    {
        var item = new DefaultLinearItem()
        {
            WidthSpecification = LayoutParamPolicies.MatchParent,
            Text = text,
            IsSelectable = false, // Item should not be remained as selected state.
        };

        if (subText != null)
        {
            item.SubText = subText;
        }

        ImageView icon = null;

        if (iconpath.Length > 0)
        {
            icon = new ImageView(iconpath)
            {
                Size2D = new Size2D(32, 32),
                //Name = Program.ItemContentNameIcon,
            };
            // Icon is placed at the beginning(left end) of the item.
            item.Icon = icon;
        }

        Switch toggle = null;
        if (extra)
        {
            toggle = new Switch();
            toggle.SelectedChanged += (o, e) =>
            {
                if (e.IsSelected)
                {
                    Tizen.Log.Debug("NUI", "toggle is selected!\n");
                }
                else
                {
                    Tizen.Log.Debug("NUI", "toggle is unselected!\n");
                }
            };
            // Extra is placed at the end(right end) of the item.
            item.Extra = toggle;

            // Do not propagate Pressed/Selected states from item to item.Extra.
            // When item is pressed/clicked/selected, item.Extra is not pressed/clicked/selected.
            item.Extra.PropagatableControlStates = ControlState.Disabled;
        }

        item.Clicked += (o, e) =>
        {
            if (toggle != null)
            {
                toggle.IsSelected = !toggle.IsSelected;
            }

            Tizen.Log.Debug("NUI", "item is clicked!\n");
        };



        return item;
    }
    // Create a page with scrollable content
    private void CreateSettingsMainMenu()
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
        item = CreateItemWithIcon(Resources.IDS_ST_BODY_WI_FI, resPath + SETTING_LIST_ICON_PATH_CFG + "settings_wifi.png");
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    Window window = GetDefaultWindow();
                    LaunchWidget(window, "wifi@org.tizen.cssetting-wifi");
                };
                content.Add(item);
            }
        item = CreateItemWithIcon(Resources.IDS_TPLATFORM_OPT_BLUETOOTH, resPath + SETTING_LIST_ICON_PATH_CFG + "settings_bluetooth.png");
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    Window window = GetDefaultWindow();
                    LaunchWidget(window, "bluetooth@org.tizen.cssetting-bluetooth");
                };
                content.Add(item);
            }

        item = CreateItemWithIcon(Resources.IDS_ST_HEADER_SOUND, resPath + SETTING_LIST_ICON_PATH_CFG + "settings_sound_and_notifications.png");
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
        item = CreateItemWithIcon(Resources.IDS_ST_BODY_NOTIFICATIONS, resPath + SETTING_LIST_ICON_PATH_CFG + "settings_notifications.png");
        if (item != null)
        {
            content.Add(item);
        }
#endif


        item = CreateItemWithIcon(Resources.IDS_ST_HEADER_DISPLAY, resPath + SETTING_LIST_ICON_PATH_CFG + "settings_display.png");
        if (item != null)
        {
            item.Clicked += (o, e) =>
            {
                Window window = GetDefaultWindow();
                LaunchWidget(window, "display@org.tizen.cssettings");
            };
            content.Add(item);
        }

        item = CreateItemWithIcon(Resources.IDS_LCKSCN_BODY_WALLPAPERS, resPath + SETTING_LIST_ICON_PATH_CFG + "settings_wallpapers.png");
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
        item = CreateItemWithIcon("Tray", resPath + SETTING_LIST_ICON_PATH_CFG + "settings_softkey.png");
        content.Add(item);
        item = CreateItemWithIcon("Screen Mirroring", resPath + SETTING_LIST_ICON_PATH_CFG + "settings_softkey.png");
        content.Add(item);
#endif
            item = CreateItemWithIcon(Resources.IDS_ST_BODY_ACCOUNTS, resPath + SETTING_LIST_ICON_PATH_CFG + "settings_account.png");
        if (item != null)
        {
            item.Clicked += (o, e) =>
            {
                Window window = GetDefaultWindow();
                LaunchWidget(window, "account@org.tizen.cssetting-account");
            };
            content.Add(item);
        }

        item = CreateItemWithIcon(Resources.IDS_ST_HEADER_PRIVACY, resPath + SETTING_LIST_ICON_PATH_CFG + "settings_privacy_and_safety.png");
        if (item != null)
        {
            item.Clicked += (o, e) =>
            {
                Window window = GetDefaultWindow();
                LaunchWidget(window, "privacy@org.tizen.cssetting-privacy");
            };
            content.Add(item);
        }

        item = CreateItemWithIcon(Resources.IDS_ST_BODY_APPLICATIONS, resPath + SETTING_LIST_ICON_PATH_CFG + "settings_applications.png");
        if (item != null)
        {
            item.Clicked += (o, e) =>
            {
                Window window = GetDefaultWindow();
                LaunchWidget(window, "apps@org.tizen.cssettings");
            };
            content.Add(item);
        }

        item = CreateItemWithIcon(Resources.IDS_ST_BODY_STORAGE, resPath + SETTING_LIST_ICON_PATH_CFG + "settings_storage.png");
        if (item != null)
        {
            item.Clicked += (o, e) =>
            {
                Window window = GetDefaultWindow();
                LaunchWidget(window, "storage@org.tizen.cssettings");
            };
            content.Add(item);
        }

        item = CreateItemWithIcon(Resources.IDS_ST_HEADER_LANGUAGE_AND_INPUT, resPath + SETTING_LIST_ICON_PATH_CFG + "settings_language_and_input.png");
        if (item != null)
        {
            item.Clicked += (o, e) =>
            {
                Window window = GetDefaultWindow();
                LaunchWidget(window, "languageinput@org.tizen.cssettings");
            };
            content.Add(item);
        }

        item = CreateItemWithIcon(Resources.IDS_ST_BODY_DATE_AND_TIME, resPath + SETTING_LIST_ICON_PATH_CFG + "settings_date_and_time.png");
        if (item != null)
        {
            item.Clicked += (o, e) =>
            {
                Window window = GetDefaultWindow();
                LaunchWidget(window, "datetime@org.tizen.cssettings");
            };
            content.Add(item);
        }

        item = CreateItemWithIcon(Resources.IDS_ST_BODY_ACCESSIBILITY, resPath + SETTING_LIST_ICON_PATH_CFG + "settings_accessibility.png");
        if (item != null)
        {
            item.Clicked += (o, e) =>
            {
                Window window = GetDefaultWindow();
                LaunchWidget(window, "accessibility@org.tizen.cssetting-accessibility");
            };
            content.Add(item);
        }

        item = CreateItemWithIcon(Resources.IDS_ST_BODY_ABOUT_DEVICE, resPath + SETTING_LIST_ICON_PATH_CFG + "settings_about_device.png");
        if (item != null)
        {
            item.Clicked += (o, e) =>
            {
                Window window = GetDefaultWindow();
                LaunchWidget(window, "aboutdevice@org.tizen.cssettings");
            };
            content.Add(item);
        }


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

        var contentPage = new ContentPage()
        {
            AppBar = appBar,

            Content = content,
        };

        // Push the page to the default navigator.
        NUIApplication.GetDefaultWindow().GetDefaultNavigator().Push(contentPage);
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
            Tizen.Log.Debug("NUI", String.Format($"widget launch : {0}\n", widgetid));

            widgetview.Position = new Position(0, 0);
            widgetview.WidgetContentUpdated += OnWidgetContentUpdatedCB;
            widgetview.Preview = false;

            var page = new ContentPage();
            page.Content = widgetview;
            navigator.Push(page);
        }
    }

    static void Main(string[] args)
        {
            var appCustomBorder = new SettingViewBorder();
            var app = new Program("", new Size2D(800, 400), new Position2D(300, 100), appCustomBorder);

            app.Run(args);
        }

        WidgetView mWidgetView;
    }
}
