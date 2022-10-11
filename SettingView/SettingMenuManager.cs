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

using System.Collections.ObjectModel;
using System.IO;

using static SettingView.Vconf;

using System.Text.Json;

namespace SettingView
{
    public class WidgetViewInfo
    {
        private string Id;
        private WidgetView View;


        public WidgetViewInfo(string id, WidgetView view)
        {
            Id = id;
            View = view;
        }


        public string GetId()
        {
            return Id;
        }

        public WidgetView GetView()
        {
            return View;
        }
    };




    /// //////////////////////////////////////////////////////////////////////////

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
#if false
        private static Collection<WidgetViewInfo> mWidgetViewPool;
#endif

        private static string LastestPushWidgetId = "";

        /////////////////////////////////////////////////////////////
        /// Build MenuList with Table

        public static void BuildMenuList(View content, SettingMenuInfo[] menulist, Window window)
        {
#if false
            mWidgetViewPool = new Collection<WidgetViewInfo>();
#endif


            DefaultLinearItem item;

            foreach (var menu in menulist)
            {
                item = SettingItemCreator.CreateItemWithIcon(menu.Name, menu.IconPath);
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
                    content.Add(item);
                }
            }
        }

        public static void ClearMenuList()
        {
#if false
            mWidgetViewPool.Clear();
#endif
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

#if false
            WidgetView widgetview = null;
            // find widgetview in mWidgetViewPool
            foreach (var info in mWidgetViewPool)
            {
                if (info.GetId().Equals(widgetid)) {
                        widgetview = info.GetView();
                }
            }
            if (widgetview == null)
                widgetview = WidgetViewManager.Instance.AddWidget(widgetid, encodedBundle, window.Size.Width, window.Size.Height, 0.0f);
#else
            WidgetView widgetview = WidgetViewManager.Instance.AddWidget(widgetid, encodedBundle, window.Size.Width, window.Size.Height, 0.0f);
#endif

            if (widgetview != null)
            {
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

                    var page = new ContentPage
                    {
                        Content = widgetview
                    };
                    navigator.Push(page);

                    LastestPushWidgetId = widgetid;
#if false
                    mWidgetViewPool.Add(new WidgetViewInfo(widgetid, widgetview));
#endif
                }
            }
        }

        public static void PopWidget(Window window)
        {
            window.GetDefaultNavigator().Pop();

            LastestPushWidgetId = "";
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
                    Tizen.Log.Debug("NUI", "WIDGET_ACTION : PUSH!\n");

                    if (Int32.TryParse(widgetWidth, out int width) && Int32.TryParse(widgetHeight, out int height))
                    {
                        Bundle bundle2 = new Bundle();
                        bundle2.AddItem(" ", " ");
                        String encodedBundle2 = bundle2.Encode();

                        WidgetView widgetview = WidgetViewManager.Instance.AddWidget(widgetID, encodedBundle2, width, height, 0.0f);
                        if (widgetview != null)
                        {
                            widgetview.WidgetContentUpdated += OnWidgetContentUpdatedCB;
                            widgetview.Preview = false;
                            NUIApplication.GetDefaultWindow().GetDefaultNavigator().Push(new ContentPage() { Content = widgetview });
                        }
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
                    PopWidget(NUIApplication.GetDefaultWindow());
                }
                else if (widgetAction.Equals("LAUNCH"))
                {
                    Tizen.Log.Debug("NUI", "WIDGET_ACTION : LAUNCH!\n");
                    LaunchApplication(appID);
                }
            }
        }

        public static bool WriteMenuList(SettingMenuInfo[] menulist, string folderpath, string name)
        {
            string locale = Vconf.GetString("db/menu_widget/language");
            String[] qStrings = locale.Split('.');
            locale = qStrings[0];

            string filename = name + "_" + locale + ".menulist";


            
            try
            {
                // Use Combine again to add the file name to the path.
                string pathString = System.IO.Path.Combine(folderpath, filename);

                Tizen.Log.Debug("NUI", string.Format("Path to menu list file: {0}", pathString));

                // Check that the file doesn't already exist. If it doesn't exist, create
                // the file and write integers 0 - 99 to it.
                // DANGER: System.IO.File.Create will overwrite the file if it already exists.
                // This could happen even with random file names, although it is unlikely.
                if (System.IO.File.Exists(pathString))
                {
                    Tizen.Log.Debug("NUI", string.Format("File {0} already exists.", pathString));
                    return false;
                }


                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize(menulist, options);
                Tizen.Log.Debug("NUI", "JSON : " + jsonString);

                File.WriteAllText(pathString, jsonString);

            }
            catch (System.IO.IOException e)
            {
                Tizen.Log.Debug("NUI", "IO Error : " + e.Message);
            }

            return true;
        }


#if true

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
#endif

    }
}
