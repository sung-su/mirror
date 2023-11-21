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
        private ContentPage mMainPage;
        private static Task rowsCreated;

        public Program(Size2D windowSize, Position2D windowPosition, ThemeOptions themeOptions, IBorderInterface borderInterface)
            : base(windowSize, windowPosition, themeOptions, borderInterface)
        {
        }

        protected override void OnCreate()
        {
            Logger.Performance($"ONCREATE start");

            base.OnCreate();

            Logger.Performance($"ONCREATE base");

            mMainPage = CreateMainPage();

            Logger.Performance($"ONCREATE main page");

            mMainPage.Content = CreateScrollableBase();

            Logger.Performance($"ONCREATE scrolable base");

            rowsCreated = CreateContentRows(mMainPage.Content);
            _ = CheckCustomization();

            var navigator = new SettingNavigation();

            GetDefaultWindow().Remove(GetDefaultWindow().GetDefaultNavigator());
            GetDefaultWindow().SetDefaultNavigator(navigator);
            GetDefaultWindow().GetDefaultNavigator().Push(mMainPage);

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
                    if (mMainPage != null)
                    {
                        await Post(() =>
                        {
                            mMainPage.Content = CreateContent(true);
                            return true;
                        });
                    }
                }
                return true;
            });
        }

        private ContentPage CreateMainPage()
        {
            var mainPage = new BaseContentPage()
            {
                CornerRadius = (SettingViewBorder.WindowCornerRadius - SettingViewBorder.WindowPadding).SpToPx(),
                ThemeChangeSensitive = true,
            };

            Logger.Performance($"ONCREATE base page");

            CreateAppBar(mainPage);
            return mainPage;
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

            if (mMainPage != null && items.Any())
            {
                mMainPage.Content = CreateContent(true);
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
            if (mMainPage != null && mMainPage.AppBar != null)
            {
                MainMenuInfo.ClearCache();

                mMainPage.AppBar.Title = Resources.IDS_ST_OPT_SETTINGS;
                mMainPage.Content = CreateContent();
            }
        }

        private void ThemeManager_ThemeChanged(object sender, ThemeChangedEventArgs e)
        {
            if (mMainPage != null && e.IsPlatformThemeChanged)
            {
                MainMenuInfo.ClearCache();

                // recreate main page content just to apply new colors from gadgets
                mMainPage.Content = CreateContent();
            }
        }

        private static System.Threading.Tasks.Task CreateAppBar(BaseContentPage mainPage)
        {
            return System.Threading.Tasks.Task.Run(async () =>
            {
                await Post(() =>
                {
                    System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                    stopwatch.Start();

                    // TODO: remove style customization with scalable unit, when merged to TizenFX
                    var appBarStyle = ThemeManager.GetStyle("Tizen.NUI.Components.AppBar") as AppBarStyle;
                    appBarStyle.TitleTextLabel.PixelSize = 24.SpToPx();

                    AppBar appBar = new AppBar(appBarStyle)
                    {
                        Size = new Size(-1, 64).SpToPx(),
                        Padding = new Extents(16, 16, 0, 0).SpToPx(),
                        Title = Resources.IDS_ST_OPT_SETTINGS,
                        AutoNavigationContent = false,
                        NavigationContent = new View(), // FIXME: must be set with empty View to hide default back button
                        ThemeChangeSensitive = true,
                    };
                    appBar.Relayout += (s, e) =>
                    {
                        stopwatch?.Stop();
                        Logger.Performance($"UICompleted appbar: {stopwatch.Elapsed.TotalMilliseconds}");
                    };
                    mainPage.AppBar = appBar;
                    return true;
                });
            });
        }

        private static View CreateScrollableBase()
        {
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

            return content;
        }

        private static System.Threading.Tasks.Task CreateContentRows(View content, bool customizationChanged = false)
        {
            return System.Threading.Tasks.Task.Run(async () =>
            {
                System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();
                var menus = MainMenuInfo.CacheMenu;
                if (menus.Count == 0 || customizationChanged)
                {
                    var mainMenus = GadgetManager.Instance.GetMainWithCurrentOrder();
                    if (!mainMenus.Any())
                    {
                        await Post(() =>
                        {
                            var textLabel = GetTextNotice("There is no setting menus installed.", Color.Orange);
                            content.Add(textLabel);
                            return true;
                        });
                        return;
                    }

                    var visibleMenus = mainMenus.Where(i => i.IsVisible);
                    if (!visibleMenus.Any())
                    {
                        await Post(() =>
                        {
                            var textLabel = GetTextNotice("There is no setting menus visible.", Color.Gray);
                            content.Add(textLabel);
                            return true;
                        });
                        return;
                    }

                    menus = new List<MainMenuInfo>();
                    foreach (var gadgetInfo in visibleMenus)
                    {
                        if (MainMenuInfo.Create(gadgetInfo) is MainMenuInfo menu)
                        {
                            menus.Add(menu);
                        }
                    }
                }

                stopwatch.Stop();
                Logger.Performance($"CONTENT get data: {stopwatch.Elapsed.TotalMilliseconds}");

                stopwatch.Restart();
                foreach (var menu in menus)
                {
                    await Post(() =>
                    {
                        var row = new SettingCore.Views.MainMenuItem(menu.IconPath, new Color(menu.IconColorHex), menu.Title);
                        row.Clicked += (s, e) =>
                        {
                            Logger.Debug($"navigating to menupath {menu.Path}, title: {menu.Title}");
                            GadgetNavigation.NavigateTo(menu.Path);
                        };
                        if (menus.Last() == menu)
                        {
                            row.Relayout += (s, e) =>
                            {
                                stopwatch?.Stop();
                                Logger.Performance($"UICompleted items: {stopwatch.Elapsed.TotalMilliseconds}");
                            };
                        }
                        content.Add(row);
                        return true;
                    });
                }

                MainMenuInfo.UpdateCache(menus);
            });
        }

        private static View CreateContent(bool customizationChanged = false)
        {
            var content = CreateScrollableBase();
            CreateContentRows(content, customizationChanged);
            return content;
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
            float widthRatio = 0.7f;
            float heightRatio = 0.7f;

            _ = Information.TryGetValue("http://tizen.org/feature/screen.width", out int screenWidth);
            _ = Information.TryGetValue("http://tizen.org/feature/screen.height", out int screenHeight);

            int width = (int)(screenWidth * widthRatio);
            int height = (int)(screenHeight * (1 - bottomMargin) * heightRatio);

            // INFO: it looks like size of custom border is not included in total window size
            Size2D size = new Size2D(width, height);
            Position2D position = new Position2D((screenWidth - width) / 2, (screenHeight - height) / 2 - (int)(bottomMargin * screenHeight));

            appCustomBorder = new SettingViewBorder();

            Logger.Performance($"MAIN border");

            var app = new Program(size, position, ThemeOptions.PlatformThemeEnabled, appCustomBorder);

            app.Run(args);
        }
    }
}
