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
    class SettingRenameDevicePage : ContentPage
    {

        protected Window mWindow;
        protected string mTitle;

        private const int MAX_DEVICE_NAME_LEN = 32;
        TextField mTextField;
        public SettingRenameDevicePage(Window window)
            : base()
        {
            mWindow = window;

            mTitle = Resources.IDS_ST_BUTTON_BACK;

            mTextField = null;

            Content = CreateContent(window);
            AppBar = CreateAppBar(mTitle);
        }

        protected AppBar CreateAppBar(string title)
        {
            var appBar = new AppBar()
            {
                Title = title,
                AutoNavigationContent = false,
            };

            var appBarStyle = ThemeManager.GetStyle("Tizen.NUI.Components.AppBar");

            var navigationContent = new Button(((AppBarStyle)appBarStyle).BackButton);
            navigationContent.Clicked += (o, e) =>
            {
                if (mWindow != null)
                {
                    Navigator navigator = mWindow.GetDefaultNavigator();
                    navigator.Pop();
                }
            };
            appBar.NavigationContent = navigationContent;
            //appBarStyle.Dispose();

            return appBar;
        }

        protected View CreateContent(Window window)
        {
            var content = new View()
            {

                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
                Layout = new LinearLayout()
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                },
            };

            var textTitle = SettingItemCreator.CreateItemTitle(Resources.IDS_ST_HEADER_RENAME_DEVICE);
            content.Add(textTitle);

            var textSubTitle = new TextLabel(Resources.IDS_ST_BODY_DEVICE_NAMES_ARE_DISPLAYED)
            {
                MultiLine = true,
                LineWrapMode = LineWrapMode.Character,
                Size2D = new Size2D(window.WindowSize.Width-20*2, 100),
            };
            content.Add(textSubTitle);

            String name = Vconf.GetString("db/setting/device_name");

            PropertyMap placeholder = new PropertyMap();
            placeholder.Add("color", new PropertyValue(Color.CadetBlue));
            placeholder.Add("fontFamily", new PropertyValue("Serif"));
            placeholder.Add("pointSize", new PropertyValue(25.0f));

            mTextField = new TextField
            {
                BackgroundColor = Color.White,

                Placeholder = placeholder,

                MaxLength = MAX_DEVICE_NAME_LEN,
                EnableCursorBlink = true,
                Text = name,
            };
            content.Add(mTextField);


            var button = new Button()
            {
                // WidthSpecification = LayoutParamPolicies.MatchParent,
                // HeightSpecification = LayoutParamPolicies.MatchParent,
                Text = Resources.IDS_ST_BUTTON_OK
            };
            button.Clicked += (o, e) =>
            {
                // Change Device Name
                Vconf.SetString("db/setting/device_name", mTextField.Text);

                Navigator navigator = mWindow.GetDefaultNavigator();
                navigator.Pop();
                //RequestWidgetPop();
            };
            content.Add(button);

            return content;
        }
    }
}
