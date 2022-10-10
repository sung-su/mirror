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
using System.ComponentModel;
//using System.Diagnostics;
using System.Collections.Generic;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.Applications;

namespace SettingMain
{
    abstract class SettingContent_Base : Widget
    {


        protected static readonly string resPath = Tizen.Applications.Application.Current.DirectoryInfo.Resource;
        protected const string SETTING_ICON_PATH_CFG = "/icons/";
        protected const string SETTING_LIST_ICON_PATH_CFG = "/icons/list_icon/";

        /// ///////////////////////////////////////////////////////////////////////////
        /// 

        protected void RequestWidgetPush(string widgetid)
        {
            if (mWindow == null) return;

            // Update Widget Content by sending message to add the third page in advance.
            Bundle nextBundle = new Bundle();
            nextBundle.AddItem("WIDGET_ID", widgetid);
            nextBundle.AddItem("WIDGET_WIDTH", mWindow.Size.Width.ToString());
            nextBundle.AddItem("WIDGET_HEIGHT", mWindow.Size.Height.ToString());
            nextBundle.AddItem("WIDGET_PAGE", "CONTENT_PAGE");
            nextBundle.AddItem("WIDGET_ACTION", "PUSH");
            String encodedBundle = nextBundle.Encode();
            SetContentInfo(encodedBundle);
        }

        protected void RequestWidgetPop()
        {
            // Update Widget Content by sending message to pop the fourth page.
            Bundle nextBundle = new Bundle();
            nextBundle.AddItem("WIDGET_ACTION", "POP");
            String encodedBundle = nextBundle.Encode();
            SetContentInfo(encodedBundle);
        }

        ///////////////////////////////////////////////////////////
        ///

        protected Window mWindow;

        protected string mTitle;
        protected ContentPage mPage;

        public SettingContent_Base()
            : base()
        {
            mWindow = null;
            mPage = null;
        }

        protected override void OnCreate(string contentInfo, Window window)
        {
            Bundle bundle = Bundle.Decode(contentInfo);

            window.BackgroundColor = Color.Transparent;

            mWindow = window;

            mPage = new ContentPage()
            {
                Content = CreateContent(window),
                AppBar = CreateAppBar(mTitle),
            };

            window.Add(mPage);

        }

        protected virtual AppBar CreateAppBar(string title)
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
                RequestWidgetPop();
            };
            appBar.NavigationContent = navigationContent;
            //appBarStyle.Dispose();

            return appBar;
        }

        protected abstract View CreateContent(Window window);

        protected override void OnPause()
        {
            base.OnPause();
        }

        protected override void OnResume()
        {
            Tizen.Log.Error("widget", "OnResume \n");
            base.OnResume();
        }

        protected override void OnResize(Window window)
        {
            base.OnResize(window);
        }

        protected override void OnTerminate(string contentInfo, TerminationType type)
        {

            base.OnTerminate(contentInfo, type);
        }

        protected override void OnUpdate(string contentInfo, int force)
        {
            base.OnUpdate(contentInfo, force);
        }
    }
}

