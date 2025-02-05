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

using SettingCore;
using SettingCore.Views;
using SettingView.Common;
using SettingView.Core;
using SettingView.TextResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tizen.Applications;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.System;

namespace SettingView
{
    public class Program : NUIApplication
    {
        private static SettingViewBorder appCustomBorder;
        private static CustomPage page;
        private static Window window;
        private static Task rowsCreated;
        private static bool noMainMenus;
        private static bool noVisibleMainMenus;
        private static List<MainMenuInfo> mainMenuInfos;
        private static List<MainMenuItem> mainMenuItems = new List<MainMenuItem>();
        private static Task itemsLoaded;
        private static bool isFirstResumed = false;

        public Program() : base(new Size2D(1, 1), new Position2D(0, 0), ThemeOptions.PlatformThemeEnabled, appCustomBorder)
        {
        }

        protected override void OnPreCreate()
        {
            itemsLoaded = LoadMainMenuItems();
            base.OnPreCreate();
        }

        private void CreateTitleAndScroll()
        {
            page.Title = new TextLabel()
            {
                Size = new Size(-1, 64).SpToPx(),
                Margin = new Extents(16, 0, 0, 0).SpToPx(),
                Text = Resources.IDS_ST_OPT_SETTINGS,
                VerticalAlignment = VerticalAlignment.Center,
                PixelSize = 24.SpToPx(),
                ThemeChangeSensitive = true,
            };

            page.Add(page.Title);

            page.Content = new ScrollableBase()
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

            SetScrollbar();

            page.Add(page.Content);

            Logger.Debug("Title and scroller added");
        }

        protected override void OnCreate()
        {
            Logger.Debug("OnCreate start");
            base.OnCreate();

            window = GetDefaultWindow();

            var navigator = new SettingNavigation();
            window.Remove(window.GetDefaultNavigator());
            window.SetDefaultNavigator(navigator);

            page = new CustomPage()
            {
                BackgroundColor = AppConstants.BackgroundColor,
            };
            // Navigator().Push() disables border's accessibility. So, using Navigator().Add()
            window.GetDefaultNavigator().Add(page);

            CreateTitleAndScroll();
            WindowManager.UpdateWindowPositionSize();

            Logger.Debug("OnCreate end");
        }

        protected override void OnResume()
        {
            Logger.Debug("OnResume");
            base.OnResume();

            if (!isFirstResumed)
            {
                isFirstResumed = true;
                rowsCreated = CreateContentRows();

                RegisterEvents();
                _ = CheckCustomization();

                List<Window.WindowOrientation> list = new List<Window.WindowOrientation>
                {
                    Window.WindowOrientation.Portrait,
                    Window.WindowOrientation.Landscape,
                    Window.WindowOrientation.PortraitInverse,
                    Window.WindowOrientation.LandscapeInverse,
                };
                window.SetAvailableOrientations(list);
            }
        }

        private void WindowModeChanged(object ob, bool fullScreenMode)
        {
            if (fullScreenMode)
            {
                if (appCustomBorder.BorderWindow is null || appCustomBorder.BorderWindow.IsMaximized())
                {
                    return;
                }

                appCustomBorder.OverlayMode = true;
                appCustomBorder.BorderWindow.Maximize(true);
            }
            else
            {
                if (appCustomBorder.BorderWindow is null || appCustomBorder.BorderWindow.IsMinimized())
                {
                    return;
                }

                appCustomBorder.BorderWindow.Maximize(false);
            }
        }

        private async Task CheckCustomization()
        {
            await rowsCreated;
            await Task.Run(async () =>
            {
                GadgetManager.Instance.Init();

                var customizationMainMenus = GadgetManager.Instance.GetMainWithCurrentOrder();

                var customizationMainMenusStr = customizationMainMenus.Where(i => i.IsVisible).Select(x => new string(x.Path));
                var cacheMainMenuStr = MainMenuInfo.CacheMenu.Select(x => new string(x.Path));
                var notCachedGadgets = customizationMainMenusStr.Except(cacheMainMenuStr);
                var removedGedgets = cacheMainMenuStr.Except(customizationMainMenusStr);

                if (!customizationMainMenusStr.SequenceEqual(cacheMainMenuStr))
                {
                    var newMainMenus = new List<SettingGadgetInfo>();

                    if (notCachedGadgets.Count() > 0)
                    {
                        foreach (var gadgetInfo in customizationMainMenus.Where(a => notCachedGadgets.Any(e => e.Equals(a.Path))))
                        {
                            if (MainMenuInfo.Create(gadgetInfo) is MainMenuInfo menu)
                            {
                                newMainMenus.Add(gadgetInfo);
                            }
                        }
                    }

                    if (removedGedgets.Count() > 0 || newMainMenus.Count() > 0)
                    {
                        Logger.Verbose($"customization has changed. Reload main view.");
                        await Post(() =>
                        {
                            CreateContent(true);
                            return true;
                        });
                    }
                }
                return true;
            });
        }

        private void OnWindowOrientationChangedEvent(object sender, WindowOrientationChangedEventArgs e)
        {
            Window.WindowOrientation orientation = e.WindowOrientation;
            Logger.Debug($"OnWindowOrientationChangedEvent() called!, orientation:{orientation}");
        }

        private void RegisterEvents()
        {
            SystemSettings.LocaleLanguageChanged += SystemSettings_LocaleLanguageChanged;
            ThemeManager.ThemeChanged += ThemeManager_ThemeChanged;
            GadgetManager.Instance.CustomizationChanged += CustomizationChanged;
            GadgetNavigation.OnWindowModeChanged += WindowModeChanged;
            window.OrientationChanged += OnWindowOrientationChangedEvent;
        }

        private void UnregisterEvents()
        {
            SystemSettings.LocaleLanguageChanged -= SystemSettings_LocaleLanguageChanged;
            ThemeManager.ThemeChanged -= ThemeManager_ThemeChanged;
            GadgetManager.Instance.CustomizationChanged -= CustomizationChanged;
            GadgetNavigation.OnWindowModeChanged -= WindowModeChanged;
            window.OrientationChanged -= OnWindowOrientationChangedEvent;
        }

        protected override void OnTerminate()
        {
            UnregisterEvents();
            base.OnTerminate();
        }

        private void CustomizationChanged(object sender, CustomizationChangedEventArgs e)
        {
            List<MenuCustomizationItem> items = new List<MenuCustomizationItem>();
            foreach (var c in e.CustomizationItems)
            {
                if (GadgetManager.Instance.IsMainMenuPath(c.MenuPath))
                {
                    items.Add(c);
                }
            }

            if (page != null && items.Any())
            {
                CreateContent(true);
            }
        }

        protected override void OnAppControlReceived(AppControlReceivedEventArgs e)
        {
            base.OnAppControlReceived(e);

            var keys = e.ReceivedAppControl.ExtraData.GetKeys();
            if (keys.Contains("cmd") && keys.Contains("menupath") && keys.Contains("order"))
            {
                var data = e.ReceivedAppControl.ExtraData;
                string cmd = data.Get<string>("cmd");
                string menupath = data.Get<string>("menupath");
                string order = data.Get<string>("order");
                Logger.Debug($"AppControl (cmd: {cmd}, menupath: {menupath}, order: {order})");

                if (cmd == "customize")
                {
                    bool negative = order.EndsWith('-');
                    order = order.Replace("-", "");

                    if (!int.TryParse(order, out int orderValue))
                    {
                        Logger.Warn($"order value '{order}' is not corrent type (integer value with minus sign after digits, e.g. 7-)");
                        return;
                    }

                    if (negative)
                        orderValue *= -1;

                    GadgetManager.Instance.ChangeMenuPathOrder(menupath, orderValue);
                }
            }
            Logger.Debug("End");
        }

        private void SystemSettings_LocaleLanguageChanged(object sender, Tizen.System.LocaleLanguageChangedEventArgs e)
        {
            if (page != null)
            {
                page.Title.Text = Resources.IDS_ST_OPT_SETTINGS;
                UpdateContent();
            }
        }

        private void ThemeManager_ThemeChanged(object sender, ThemeChangedEventArgs e)
        {
            if (page != null && e.IsPlatformThemeChanged)
            {
                page.BackgroundColor = AppConstants.BackgroundColor;
                SetScrollbar();
                UpdateContent();
            }
        }

        private static Task LoadMainMenuItems(bool customizationChanged = false)
        {
            return Task.Run(() =>
            {
                noMainMenus = false;
                noVisibleMainMenus = false;
                mainMenuInfos = MainMenuInfo.CacheMenu;
                if (mainMenuInfos.Count == 0 || customizationChanged)
                {
                    var mainMenus = GadgetManager.Instance.GetMainWithCurrentOrder();
                    if (!mainMenus.Any())
                    {
                        noMainMenus = true;
                        return Task.CompletedTask;
                    }

                    var visibleMenus = mainMenus.Where(i => i.IsVisible);
                    if (!visibleMenus.Any())
                    {
                        noVisibleMainMenus = true;
                        return Task.CompletedTask;
                    }

                    mainMenuInfos = new List<MainMenuInfo>();
                    foreach (var gadgetInfo in visibleMenus)
                    {
                        if (MainMenuInfo.Create(gadgetInfo) is MainMenuInfo menu)
                        {
                            mainMenuInfos.Add(menu);
                        }
                    }
                }
                else
                {
                    Logger.Debug("Loaded Main Menu from cache");
                }

                return Task.CompletedTask;
            });
        }

        private static async Task CreateContentRows()
        {
            await itemsLoaded;
            await Task.Run(async () =>
            {
                if (noMainMenus)
                {
                    await Post(() =>
                    {
                        var textLabel = GetTextNotice("There is no setting menus installed.", Color.Orange);
                        page.Content.Add(textLabel);
                        return true;
                    });

                    return;
                }

                if (noVisibleMainMenus)
                {
                    await Post(() =>
                    {
                        var textLabel = GetTextNotice("There is no setting menus visible.", Color.Gray);
                        page.Content.Add(textLabel);
                        return true;
                    });

                    return;
                }
                int count = 0;
                foreach (var menu in mainMenuInfos)
                {
                    count++;
                    await Post(() =>
                    {
                        var row = new MainMenuItem(menu.IconPath, new Color(menu.IconColorHex), menu.Title, menu.Path);
                        mainMenuItems.Add(row);
                        page.Content.Add(row);
                        return true;
                    });
                }
                MainMenuInfo.UpdateCache(mainMenuInfos);
            });
        }

        private static void CreateContent(bool customizationChanged = false)
        {
            mainMenuItems.Clear();
            page.Content.RemoveAllChildren(true);
            itemsLoaded = LoadMainMenuItems(customizationChanged);
            _ = CreateContentRows();
        }

        private void SetScrollbar()
        {
            var scrollbarStyle = ThemeManager.GetStyle("Tizen.NUI.Components.Scrollbar") as ScrollbarStyle;
            scrollbarStyle.ThumbColor = AppConstants.ThumbColor;
            scrollbarStyle.TrackPadding = 8;
            page.Content.Scrollbar = new Scrollbar(scrollbarStyle);

            // get Thumb ImageView component, since it is internal
            var thumb = page.Content.Scrollbar.Children.Skip(1).FirstOrDefault() as ImageView;

            if (thumb != null)
            {
                thumb.CornerRadius = 4;
                thumb.BoxShadow = AppConstants.ThumbBoxShadow;
            }
        }

        private static async void UpdateContent()
        {
            MainMenuInfo.ClearCache();
            await LoadMainMenuItems();

            foreach (var menu in mainMenuInfos)
            {
                var item = mainMenuItems.Where(a => a.MenuPath == menu.Path).FirstOrDefault();
                if (item != null)
                {
                    item.UpdateItem(menu.Title, new Color(menu.IconColorHex));
                }
            }
        }

        private static View GetTextNotice(string text, Color color)
        {
            return new TextLabel
            {
                MultiLine = true,
                Text = text,
                TextColor = color,
                PixelSize = 30.SpToPx(),
                Margin = new Extents(50, 50, 50, 50).SpToPx(),
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
        }

        static void Main(string[] args)
        {
            appCustomBorder = new SettingViewBorder();
            Program app = new Program();
            app.Run(args);
        }
    }
}
