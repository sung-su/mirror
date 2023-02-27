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
        }

        protected override void OnTerminate()
        {
            Tizen.System.SystemSettings.LocaleLanguageChanged -= SystemSettings_LocaleLanguageChanged;
            base.OnTerminate();
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
            return new AppBar()
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

            var mainGadgetInfos = GadgetManager.GetMainWithDefaultOrder();

            foreach (var gadgetInfo in mainGadgetInfos.Where(i => i.Order > 0))
            {
                Logger.Debug($"{gadgetInfo}");

                var info = MainMenuInfo.Create(gadgetInfo);
                if (info != null)
                {
                    var row = new MainMenuRowView(info.IconPath, info.IconColor, info.Title);

                    // TODO: replace TouchEvent with Clicked for Accessibility
                    row.TouchEvent += (object source, Tizen.NUI.BaseComponents.View.TouchEventArgs e) =>
                    {
                        if (e.Touch.GetState(0) == PointStateType.Down)
                        {
                            info.Action?.Invoke();
                        }
                        return true;
                    };

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
