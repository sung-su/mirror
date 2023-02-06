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

using System.Linq;

using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.System;

using SettingAppTextResopurces.TextResources;

namespace SettingView
{
    public class Program : NUIApplication
    {
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

            mMainPage = new ContentPage()
            {
                AppBar = CreateAppBar(),
                Content = CreateMainMenuContent(),
            };

            window.GetDefaultNavigator().Push(mMainPage);
            window.GetDefaultNavigator().Popped += Program_Popped;

            Tizen.System.SystemSettings.LocaleLanguageChanged += SystemSettings_LocaleLanguageChanged;
        }

        protected override void OnTerminate()
        {
            Tizen.System.SystemSettings.LocaleLanguageChanged -= SystemSettings_LocaleLanguageChanged;

            Window window = GetDefaultWindow();
            window.KeyEvent -= OnKeyEvent;

            base.OnTerminate();
        }

        private void Program_Popped(object sender, PoppedEventArgs e)
        {
            SettingMenuManager.DisposePoppedPage(e.Page);
        }

        private void SystemSettings_LocaleLanguageChanged(object sender, Tizen.System.LocaleLanguageChangedEventArgs e)
        {
            if (mMainPage != null)
            {
                mMainPage.AppBar.Title = Resources.IDS_ST_OPT_SETTINGS;
                mMainPage.Content = CreateMainMenuContent();
            }
        }

        public void OnKeyEvent(object sender, Window.KeyEventArgs e)
        {
            if (e.Key.State == Key.StateType.Down && (e.Key.KeyPressedName == "XF86Back" || e.Key.KeyPressedName == "Escape"))
            {
                var window = sender as Window;
                Navigator navigator = window.GetDefaultNavigator();
                if (navigator.PageCount > 1)
                    SettingMenuManager.PopWidget(window);
                else
                    Exit();
            }
        }

        private AppBar CreateAppBar()
        {
            var appBarStyle = ThemeManager.GetStyle("Tizen.NUI.Components.AppBar");

            var navigationContent = new Button(((AppBarStyle)appBarStyle).BackButton);
            navigationContent.Clicked += (o, e) =>
            {
                Exit();
            };

            var appBar = new AppBar()
            {
                Title = Resources.IDS_ST_OPT_SETTINGS,
                NavigationContent = navigationContent,
            };

            return appBar;
        }

        private View CreateMainMenuContent()
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

            string path = System.IO.Path.Combine(Tizen.Applications.Application.Current.DirectoryInfo.Resource, "menu/");
            SettingMenuInfo[] menulist = SettingMenuManager.ReadMenuList(path, "settingmain");
            if (menulist != null)
            {
                var items = SettingMenuManager.CreateMenuItems(menulist.ToArray(), GetDefaultWindow());
                foreach (var item in items)
                {
                    content.Add(item);
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
