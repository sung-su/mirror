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
        private static Task rowsCreated;
        private static bool noMainMenus;
        private static bool noVisibleMainMenus;
        private static List<MainMenuInfo> mainMenuItems;
        private static Task itemsLoaded;
        private static Task contentLoaded;
        private bool isLightTheme => ThemeManager.PlatformThemeId == "org.tizen.default-light-theme";

        public Program(Size2D windowSize, Position2D windowPosition, ThemeOptions themeOptions, IBorderInterface borderInterface)
            : base(windowSize, windowPosition, themeOptions, borderInterface)
        {
        }

        private Task InitResourcesManager()
        {
            return Task.Run(() =>
            {
                // initialize ResourcesManager instance
                var title = Resources.IDS_ST_OPT_SETTINGS;
                return true;
            });
        }        
        
        private Task CreateTitleAndScroll()
        {
            return Task.Run(async () =>
            {
                await Post(() =>
                {
                    Logger.Performance($"CreateTitleAndScroll start");
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
                    page.Title.Relayout += (s, e) =>
                    {
                        Logger.Performance($"CreateTitleAndScroll label");
                    };

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

                    page.Add(page.Content);
                    page.Content.Relayout += (s, e) =>
                    {
                        Logger.Performance($"CreateTitleAndScroll scroll");
                    };
                    return true;
                });
            });
        }

        protected override void OnPreCreate()
        {
            Logger.Performance($"OnPreCreate");
            _ = InitResourcesManager();
            itemsLoaded = LoadMainMenuItems();
            base.OnPreCreate();
        }

        protected override void OnCreate()
        {
            Logger.Performance($"ONCREATE start");

            base.OnCreate();

            Logger.Performance($"ONCREATE base");

            page = new CustomPage()
            {
                BackgroundColor = isLightTheme ? new Color("#FAFAFA") : new Color("#16131A"),
            };

            page.Relayout += (s, e) =>
            {
                Logger.Performance($"Page relayout");
            };

            Logger.Performance($"ONCREATE main page");

            contentLoaded = CreateTitleAndScroll();
            rowsCreated = CreateContentRows();

            _ = CheckCustomization();

            var navigator = new SettingNavigation();

            GetDefaultWindow().Remove(GetDefaultWindow().GetDefaultNavigator());
            GetDefaultWindow().SetDefaultNavigator(navigator);
            GetDefaultWindow().GetDefaultNavigator().Push(page);

            RegisterEvents();

            GetDefaultWindow().AddAvailableOrientation(Window.WindowOrientation.Portrait);
            GetDefaultWindow().AddAvailableOrientation(Window.WindowOrientation.Landscape);
            GetDefaultWindow().AddAvailableOrientation(Window.WindowOrientation.PortraitInverse);
            GetDefaultWindow().AddAvailableOrientation(Window.WindowOrientation.LandscapeInverse);

            LogScalableInfoAsync();

            Logger.Performance($"ONCREATE end");
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

                if (!customizationMainMenusStr.SequenceEqual(cacheMainMenuStr))
                {
                    Logger.Verbose($"customization has changed. Reload main view.");
                    if (page != null)
                    {
                        await Post(() =>
                        {
                            CreateContent();
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

        private static Task LogScalableInfoAsync()
        {
            return Task.Run(() =>
            {
                var scalable = new string[]
                {
                    $"ScalingFactor = {GraphicsTypeManager.Instance.ScalingFactor}",
                    $"Dpi = {GraphicsTypeManager.Instance.Dpi}",
                    $"ScaledDpi = {GraphicsTypeManager.Instance.ScaledDpi}",
                    $"BaselineDpi = {GraphicsTypeManager.Instance.BaselineDpi}",
                    $"Density = {GraphicsTypeManager.Instance.Density}",
                    $"ScaledDensity = {GraphicsTypeManager.Instance.ScaledDensity}",
                    $"100dp => {GraphicsTypeManager.Instance.ConvertScriptToPixel("100dp")}px",
                    $"100sp => {GraphicsTypeManager.Instance.ConvertScriptToPixel("100sp")}px",
                };
                foreach (var s in scalable)
                {
                    Logger.Debug($"Scalable Info: {s}");
                }
            });
        }

        private void RegisterEvents()
        {
            SystemSettings.LocaleLanguageChanged += SystemSettings_LocaleLanguageChanged;
            ThemeManager.ThemeChanged += ThemeManager_ThemeChanged;
            GadgetManager.Instance.CustomizationChanged += CustomizationChanged;
            GadgetNavigation.OnWindowModeChanged += WindowModeChanged;
            GetDefaultWindow().OrientationChanged += OnWindowOrientationChangedEvent;
        }

        private void UnregisterEvents()
        {
            SystemSettings.LocaleLanguageChanged -= SystemSettings_LocaleLanguageChanged;
            ThemeManager.ThemeChanged -= ThemeManager_ThemeChanged;
            GadgetManager.Instance.CustomizationChanged -= CustomizationChanged;
            GadgetNavigation.OnWindowModeChanged -= WindowModeChanged;
            GetDefaultWindow().OrientationChanged -= OnWindowOrientationChangedEvent;
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
        }

        private void SystemSettings_LocaleLanguageChanged(object sender, Tizen.System.LocaleLanguageChangedEventArgs e)
        {
            if (page != null)
            {
                MainMenuInfo.ClearCache();

                page.Title.Text = Resources.IDS_ST_OPT_SETTINGS;
                CreateContent();
            }
        }

        private void ThemeManager_ThemeChanged(object sender, ThemeChangedEventArgs e)
        {
            if (page != null && e.IsPlatformThemeChanged)
            {
                page.BackgroundColor = isLightTheme ? new Color("#FAFAFA") : new Color("#16131A");

                MainMenuInfo.ClearCache();
                CreateContent();
            }
        }

        private static Task LoadMainMenuItems(bool customizationChanged = false)
        {
            return Task.Run(() => {
                System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();
                noMainMenus = false;
                noVisibleMainMenus = false;
                mainMenuItems = MainMenuInfo.CacheMenu;
                if (mainMenuItems.Count == 0 || customizationChanged)
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

                    mainMenuItems = new List<MainMenuInfo>();
                    foreach (var gadgetInfo in visibleMenus)
                    {
                        if (MainMenuInfo.Create(gadgetInfo) is MainMenuInfo menu)
                        {
                            mainMenuItems.Add(menu);
                        }
                    }
                }

                stopwatch.Stop();
                Logger.Performance($"CONTENT get data: {stopwatch.Elapsed.TotalMilliseconds}");
                return Task.CompletedTask;
            });
        }

        private static async Task CreateContentRows()
        {
            await Task.WhenAll(new Task[] { itemsLoaded, contentLoaded });
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

                System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                foreach (var menu in mainMenuItems)
                {
                    await Post(() =>
                    {
                        if(menu == mainMenuItems.First())
                        {
                            Logger.Performance($"CreateContentRows start");
                            stopwatch.Start();
                        }
                        var row = new SettingCore.Views.MainMenuItem(menu.IconPath, new Color(menu.IconColorHex), menu.Title);
                        row.Clicked += (s, e) =>
                        {
                            Logger.Debug($"navigating to menupath {menu.Path}, title: {menu.Title}");
                            GadgetNavigation.NavigateTo(menu.Path);
                        };
                        if (mainMenuItems.Last() == menu)
                        {
                            row.Relayout += (s, e) =>
                            {
                                stopwatch?.Stop();
                                Logger.Performance($"UICompleted items: {stopwatch.Elapsed.TotalMilliseconds}");
                            };
                        }
                        page.Content.Add(row);
                        return true;
                    });
                }

                MainMenuInfo.UpdateCache(mainMenuItems);
            });
        }

        private static void CreateContent(bool customizationChanged = false)
        {
            page.Content.RemoveAllChildren(true);
            itemsLoaded = LoadMainMenuItems(customizationChanged);
            _ = CreateContentRows();
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
            Logger.Performance($"MAIN start");

            // window size adjustments
            float bottomMargin = 0.1f;
            float widthRatio = 0.45f;
            float heightRatio = 0.5f;

            var screenSize = GetScreenSize();

            int width = (int)(screenSize.Width * widthRatio);
            int height = (int)(screenSize.Height * (1 - bottomMargin) * heightRatio);

            // INFO: it looks like size of custom border is not included in total window size
            Size2D size = new Size2D(width, height);
            Position2D position = new Position2D(((int)screenSize.Width - width) / 2, ((int)screenSize.Height - height) / 2 - (int)(bottomMargin * screenSize.Height));

            appCustomBorder = new SettingViewBorder();

            Logger.Performance($"MAIN border");

            var app = new Program(size, position, ThemeOptions.PlatformThemeEnabled, appCustomBorder);

            app.Run(args);
        }
    }
}
