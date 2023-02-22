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
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

using Tizen.Applications;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;

using SettingAppTextResopurces.TextResources;

namespace SettingView
{
    public enum SettingLaunchType
    {
        Widget = 0,
        Application
    };

    public class SettingMenuInfo
    {
        public string Name { get; set; }
        public SettingLaunchType LaunchType { get; set; }
        public string Id { get; set; }
        public string IconPath { get; set; }


        public SettingMenuInfo(string name, SettingLaunchType launchType, string id, string iconpath)
        {
            Name = name;
            LaunchType = launchType;
            Id = id;
            IconPath = iconpath;
        }
    };

    public class SettingMenuManager
    {
        private static string LastestPushWidgetId = "";

        public static IEnumerable<DefaultLinearItem> CreateMenuItems(SettingMenuInfo[] menulist, Window window)
        {
            foreach (var menu in menulist)
            {
                string iconPath = System.IO.Path.Combine(Tizen.Applications.Application.Current.DirectoryInfo.Resource, menu.IconPath.TrimStart('/'));
                var item = SettingItemCreator.CreateItemWithIcon(menu.Name, iconPath);
                if (item != null)
                {
                    item.Clicked += (o, e) =>
                    {
                        if (menu.LaunchType == SettingLaunchType.Widget)
                        {
                            PushWidget(window, menu.Id);
                        }
                        else if (menu.LaunchType == SettingLaunchType.Application)
                        {
                            LaunchApplication(menu.Id);
                        }
                    };
                };
                yield return item;
            }
        }

        /////////////////////////////////////////////////////////////
        /// Widget Operations

        public static void PushWidget(Window window, string widgetid)
        {
            if (LastestPushWidgetId.Equals(widgetid))
            {
                Tizen.Log.Debug("NUI", "LastestPushWidgetId : " + LastestPushWidgetId);
                return;
            }

            Navigator navigator = window.GetDefaultNavigator();

            Bundle bundle = new Bundle();
            bundle.AddItem(" ", " ");
            String encodedBundle = bundle.Encode();

            Tizen.Log.Debug("NUI", string.Format("[SettingView]  window.Size.Width : {0}, window.Size.Height : {1}", window.Size.Width, window.Size.Height));
            WidgetView widgetview = WidgetViewManager.Instance.AddWidget(widgetid, encodedBundle, window.Size.Width, window.Size.Height, 0.0f);
            if (widgetview != null)
            {
                widgetview.WidthSpecification = LayoutParamPolicies.MatchParent;
                widgetview.HeightSpecification = LayoutParamPolicies.MatchParent;

                if (string.IsNullOrEmpty(widgetview.InstanceID))
                {
                    Tizen.Log.Debug("NUI", widgetid + " is not installed!!");

                    widgetview.Dispose();
                }
                else
                {
                    Tizen.Log.Debug("NUI", String.Format($"widget launch : {0}\n", widgetid));

                    widgetview.Position = new Position(0, 0);
                    widgetview.WidgetContentUpdated += OnWidgetContentUpdatedCB;
                    widgetview.Preview = false;

                    // support H/W key input (only English)
                    widgetview.Focusable = true;
                    widgetview.FocusableInTouch = true;

                    var page = new ContentPage
                    {
                        Content = widgetview
                    };
                    navigator.Push(page);

                    LastestPushWidgetId = widgetid;
                }
            }
        }

        public static void PopWidget(Window window)
        {
            window.GetDefaultNavigator().Pop();
            LastestPushWidgetId = "";
        }

        public static void DisposePoppedPage(Page page)
        {
            ContentPage contentpage = page as ContentPage;
            if (contentpage != null)
            {
                var view = contentpage.Content;
                WidgetView widgetview = view as WidgetView;
                if (widgetview != null)
                    WidgetViewManager.Instance.RemoveWidget(widgetview);
                else
                    Tizen.Log.Debug("NUI", "This View is Not a WidgetView");
                contentpage.Dispose();
            }
            else
            {
                DialogPage dialogpage = page as DialogPage;
                if (dialogpage != null)
                {
                    var view = dialogpage.Content;
                    WidgetView widgetview = view as WidgetView;
                    if (widgetview != null)
                        WidgetViewManager.Instance.RemoveWidget(widgetview);
                    else
                        Tizen.Log.Debug("NUI", "This View is Not a WidgetView");
                    dialogpage.Dispose();
                }
                else
                    Tizen.Log.Debug("NUI", "This Page is Not a ContentPage or a DialogPage");
                
            }
            
        }
        public static void LaunchApplication(string appid)
        {
            AppControl appcontrol = new AppControl()
            {
                Operation = AppControlOperations.Default,
                ApplicationId = appid,
                LaunchMode = AppControlLaunchMode.Group,
            };
            AppControl.SendLaunchRequest(appcontrol);

            LastestPushWidgetId = "";
        }
        private static void OnWidgetContentUpdatedCB(object sender, WidgetView.WidgetViewEventArgs e)
        {
            String encodedBundle = e.WidgetView.ContentInfo;
            Bundle bundle = Bundle.Decode(encodedBundle);

            if (bundle.TryGetItem("WIDGET_ID", out string widgetID))
            {
                Tizen.Log.Debug("NUI", "WIDGET_ID!\n");
            }

            if (bundle.TryGetItem("WIDGET_WIDTH", out string widgetWidth))
            {
                Tizen.Log.Debug("NUI", "WIDGET_WIDTH!\n");
            }

            if (bundle.TryGetItem("WIDGET_HEIGHT", out string widgetHeight))
            {
                Tizen.Log.Debug("NUI", "WIDGET_HEIGHT!\n");
            }

            if (bundle.TryGetItem("WIDGET_PAGE", out string widgetPage))
            {
                Tizen.Log.Debug("NUI", "WIDGET_PAGE!\n");
            }

            if (bundle.TryGetItem("APP_ID", out string appID))
            {
                Tizen.Log.Debug("NUI", "APP_ID!\n");
            }

            if (bundle.TryGetItem("WIDGET_ACTION", out string widgetAction))
            {
                if (widgetAction.Equals("PUSH"))
                {
                    if (widgetID.Equals("renamedevice@org.tizen.cssettings"))
                    {
                        Tizen.Log.Debug("NUI", "WIDGET_ACTION : RENAMEDEVICE!\n");
                        PushRenameContents(NUIApplication.GetDefaultWindow());
                    }
                    else
                    {
                        Tizen.Log.Debug("NUI", "WIDGET_ACTION : PUSH!\n");
                        PushWidget(NUIApplication.GetDefaultWindow(), widgetID);
                    }
                }
                else if (widgetAction.Equals("POP"))
                {
                    Tizen.Log.Debug("NUI", "WIDGET_ACTION : POP!\n");
                    PopWidget(NUIApplication.GetDefaultWindow());
                }
                else if (widgetAction.Equals("LAUNCH"))
                {
                    Tizen.Log.Debug("NUI", "WIDGET_ACTION : LAUNCH!\n");
                    LaunchApplication(appID);
                }
            }
        }

        public static SettingMenuInfo[] ReadMenuList(string folderpath, string name)
        {
            string locale = Vconf.GetString("db/menu_widget/language");
            String[] qStrings = locale.Split('.');
            locale = qStrings[0];

            string filename = name + "_" + locale + ".menulist";
            string pathString = System.IO.Path.Combine(folderpath, filename);

            if (!System.IO.File.Exists(pathString))
            {
                locale = "en_US";
                filename = name + "_" + locale + ".menulist";
                pathString = System.IO.Path.Combine(folderpath, filename);
            }


            SettingMenuInfo[] menulist = null;
            try
            {
                string jsonString = File.ReadAllText(pathString);
                Tizen.Log.Debug("NUI", "JSON : " + jsonString);

                menulist = JsonSerializer.Deserialize<SettingMenuInfo[]>(jsonString);
            }
            catch (System.IO.IOException e)
            {
                Tizen.Log.Debug("NUI", "IO Error : " + e.Message);
            }


            return menulist;
        }

        private static void PushRenameContents(Window window)
        {
            Navigator navigator = window.GetDefaultNavigator();

            ContentPage renamepage = new SettingRenameDevicePage(window) {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,

                Position = new Position(0, 0),
            };

            Tizen.Log.Debug("NUI", String.Format($"RenameContent Push"));


            navigator.Push(renamepage);

        }
    }
}
