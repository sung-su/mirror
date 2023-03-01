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

using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.System;

using SettingAppTextResopurces.TextResources;
using SettingCore;
using System.Linq;
using Tizen.Applications;
using SettingCore.Customization;

namespace SettingView
{
    public class Program : NUIApplication
    {
        private ContentPage mMainPage;

        public Program(string styleSheet, Size2D windowSize, Position2D windowPosition, IBorderInterface borderInterface)
            : base(styleSheet, windowSize, windowPosition, borderInterface)
        {
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            mMainPage = new ContentPage()
            {
                AppBar = CreateAppBar(),
                Content = CreateContent(),
            };
            GetDefaultWindow().GetDefaultNavigator().Push(mMainPage);

            Tizen.System.SystemSettings.LocaleLanguageChanged += SystemSettings_LocaleLanguageChanged;
            GadgetManager.CustomizationChanged += CustomizationChanged;
        }

        protected override void OnTerminate()
        {
            Tizen.System.SystemSettings.LocaleLanguageChanged -= SystemSettings_LocaleLanguageChanged;
            GadgetManager.CustomizationChanged -= CustomizationChanged;
            base.OnTerminate();
        }

        private void CustomizationChanged(object sender, CustomizationChangedEventArgs e)
        {
            if (mMainPage != null && GadgetManager.IsMainMenuPath(e.MenuPath))
            {
                mMainPage.Content = CreateContent();
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

                    GadgetManager.UpdateCustomization(menupath, orderValue);
                }
            }
        }

        private void SystemSettings_LocaleLanguageChanged(object sender, Tizen.System.LocaleLanguageChangedEventArgs e)
        {
            if (mMainPage != null)
            {
                mMainPage.AppBar.Title = Resources.IDS_ST_OPT_SETTINGS;
                mMainPage.Content = CreateContent();
            }
        }

        private static AppBar CreateAppBar()
        {
            // TODO: remove style customization with scalable unit, when merged to TizenFX
            var appBarStyle = ThemeManager.GetStyle("Tizen.NUI.Components.AppBar") as AppBarStyle;
            appBarStyle.Size = new Size(-1, 64).SpToPx();
            appBarStyle.TitleTextLabel.PointSize = 18f.SpToPt();

            return new AppBar(appBarStyle)
            {
                Title = Resources.IDS_ST_OPT_SETTINGS,
                AutoNavigationContent = false,
                NavigationContent = new View(), // FIXME: must be set with empty View to hide default back button
            };
        }

        private static View CreateContent()
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

            var mainGadgetInfos = GadgetManager.GetMainWithCurrentOrder();

            foreach (var gadgetInfo in mainGadgetInfos.Where(i => i.IsVisible))
            {
                Logger.Debug($"{gadgetInfo}");

                var info = MainMenuInfo.Create(gadgetInfo);
                if (info != null)
                {
                    var row = new MainMenuRowView(info.IconPath, info.IconColor, info.Title, info.Action);
                    content.Add(row);
                }
            }

            return content;
        }

        static void Main(string[] args)
        {
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
            var appCustomBorder = new SettingViewBorder();

            var app = new Program("", size, position, appCustomBorder);

            app.Run(args);
        }
    }
}
