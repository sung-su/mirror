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
using System.Linq;
using System.Collections.Generic;
using Tizen.Applications;
using Tizen.NUI;
using SettingCore;
using SettingView.Core;
using SettingView.Views;
using System.Globalization;
using System.Threading.Tasks;

namespace SettingView
{
    public class Program : NUIApplication
    {
        private static SettingViewBorder appCustomBorder;
        private static Window window;

        private ViewManager viewManager;
        private bool resumedBefore;
        private static Task initTask;

        public Program() : base(new Size2D(1, 1), new Position2D(0, 0), ThemeOptions.PlatformThemeEnabled, appCustomBorder)
        {
            resumedBefore = false;
        }

        protected override void OnPreCreate()
        {
            base.OnPreCreate();
            initTask = Init();
        }

        protected override void OnCreate()
        {
            Logger.Debug("OnCreate start");
            base.OnCreate();

            window = GetDefaultWindow();
            WindowManager.UpdateWindowPositionSize();

            viewManager.SetSplashScreen();

            Post(async () =>
            {
                await initTask;
                viewManager.SetupMainView();
            });

            Logger.Debug("OnCreate end");
        }

        protected override void OnResume()
        {
            Logger.Debug("OnResume");
            base.OnResume();

            if (!resumedBefore)
            {
                resumedBefore = true;

                viewManager.UpdateMainViewModel();

                GadgetManager.Instance.Init();

                List<Window.WindowOrientation> list = new List<Window.WindowOrientation>
                {
                    Window.WindowOrientation.Portrait,
                    Window.WindowOrientation.Landscape,
                    Window.WindowOrientation.PortraitInverse,
                    Window.WindowOrientation.LandscapeInverse,
                };
                window.SetAvailableOrientations(list);

                RegisterEvents();
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

        protected override void OnTerminate()
        {
            UnRegisterEvents();

            base.OnTerminate();
        }

        private Task Init()
        {
            Logger.Debug("Init start");
            return Task.Run(() =>
            {
                SetupLanguage();
                viewManager = new ViewManager();
            });
        }

        private static void SetupLanguage()
        {
            MultilingualResourceManager = TextResources.Resources.ResourceManager;
            Tizen.System.SystemSettings.LocaleLanguageChanged += (s, e) =>
            {
                try
                {
                    string language = Tizen.System.SystemSettings.LocaleLanguage.Replace("_", "-");
                    var culture = CultureInfo.CreateSpecificCulture(language);
                    CultureInfo.CurrentCulture = culture;
                    TextResources.Resources.Culture = culture;
                }
                catch (Exception ex)
                {
                    Logger.Error("Setting Language failed" + ex.Message);
                }
            };
        }

        private void RegisterEvents()
        {
            window.KeyEvent += OnKeyEvent;
            window.OrientationChanged += WindowOrientationChanged;
        }

        private void UnRegisterEvents()
        {
            window.KeyEvent -= OnKeyEvent;
            window.OrientationChanged -= WindowOrientationChanged;
        }

        public void OnKeyEvent(object sender, Window.KeyEventArgs e)
        {
            if (e.Key.State == Key.StateType.Down && (e.Key.KeyPressedName == "XF86Back" || e.Key.KeyPressedName == "Escape"))
            {
                Exit();
            }
        }

        private void WindowOrientationChanged(object sender, WindowOrientationChangedEventArgs e)
        {
            Window.WindowOrientation orientation = e.WindowOrientation;
            Logger.Debug($"OnWindowOrientationChangedEvent() called!, orientation:{orientation}");
        }

        static void Main(string[] args)
        {
            Logger.Debug("SettingView Main() Started");

            appCustomBorder = new SettingViewBorder();
            Program app = new Program();
            app.Run(args);
        }
    }
}
