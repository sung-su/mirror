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
using System.Collections.Generic; // for Dictionary
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.Applications;
using Tizen.System;

using SettingAppTextResopurces.TextResources;

namespace SettingMain
{
    class SettingContent_Display : SettingContent_Base
    {
        private static readonly string[] mIconPath  = {
            "brightness_icon/settings_ic_brightness_00.png",
            "brightness_icon/settings_ic_brightness_01.png",
        };
        static void GetBrightnessSliderIcon(int brightness, out string iconpath) 
        {
            int iconlevel = mIconPath.Length;

            int mapped_level = 0;
            if (iconlevel > 1)
            {
                int minbrightness = 1;
                int maxbrightness = Display.Displays[0].MaxBrightness;
                if (brightness > minbrightness)
                {
                    int levelcount = maxbrightness - minbrightness;
                    int level = brightness - (minbrightness + 1);
                    mapped_level = (level * (iconlevel - 1) / levelcount) + 1;
                }
            }
            Tizen.Log.Debug("NUI", "mapped_level:" + mapped_level.ToString());

            iconpath = resPath + SETTING_ICON_PATH_CFG + mIconPath[mapped_level];
        }


        private SliderItem mBrightnessItem;
        private DefaultLinearItem mFontItem;
        private DefaultLinearItem mScreenTimeoutItem;

        public SettingContent_Display()
            : base()
        {
            mTitle = Resources.IDS_ST_HEADER_DISPLAY;

            mBrightnessItem = null;
            mFontItem = null;
            mScreenTimeoutItem = null;
        }

        protected override View CreateContent(Window window)
        {
            // Content of the page which scrolls items vertically.
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


            DefaultLinearItem item = null;

            if (Display.NumberOfDisplays > 0)
            {
                int brightness = 0;
                try
                {
                    brightness = Display.Displays[0].Brightness;
                }
                catch (Exception e)
                {
                    Tizen.Log.Debug("NUI", string.Format("error :({0}) {1} ", e.GetType().ToString(), e.Message));
                }

                int maxbrightness = Display.Displays[0].MaxBrightness;

                GetBrightnessSliderIcon(brightness, out string iconpath);

                Tizen.Log.Debug("NUI", "GET brightness : " + brightness.ToString());

                item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_BRIGHTNESS_M_POWER_SAVING);
                var slideritem = SettingItemCreator.CreateSliderItem("BRIGHTNESS", iconpath, (brightness*1.0f)/ maxbrightness);
                mBrightnessItem = slideritem;
                if (slideritem != null)
                {
                    slideritem.mSlider.ValueChanged += OnValueChanged;
                    slideritem.mSlider.SlidingStarted += OnSlidingStarted;
                    slideritem.mSlider.SlidingFinished += OnSlidingFinished;

                    content.Add(slideritem);
                }
            }
            
            SystemSettingsFontSize fontSize = SystemSettings.FontSize;
            string fontType = SystemSettings.FontType;

            item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_FONT, fontSize.ToString() + ", " + fontType);
            mFontItem = item;
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    RequestWidgetPush("font@org.tizen.cssettings");
                };
                content.Add(item);
            }


            item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_SCREEN_TIMEOUT_ABB2, SettingContent_ScreenTimeout.GetScreenTimeoutName());
            mScreenTimeoutItem = item;
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    RequestWidgetPush("timeout@org.tizen.cssettings");
                };
                content.Add(item);
            }


            item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_THEME, SettingContent_Theme.GetThemeName());
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    RequestWidgetPush("theme@org.tizen.cssettings");
                };
                content.Add(item);
            }

            return content;
        }

        protected override void OnCreate(string contentInfo, Window window)
        {
            base.OnCreate(contentInfo, window);

            Tizen.System.SystemSettings.FontSizeChanged += SystemSettings_FontSizeChanged;
            Tizen.System.SystemSettings.FontTypeChanged += SystemSettings_FontTypeChanged;

            Tizen.System.SystemSettings.ScreenBacklightTimeChanged += SystemSettings_ScreenBacklightTimeChanged;
        }

        protected override void OnTerminate(string contentInfo, TerminationType type)
        {
            Tizen.System.SystemSettings.FontSizeChanged -= SystemSettings_FontSizeChanged;
            Tizen.System.SystemSettings.FontTypeChanged -= SystemSettings_FontTypeChanged;

            Tizen.System.SystemSettings.ScreenBacklightTimeChanged -= SystemSettings_ScreenBacklightTimeChanged;

            base.OnTerminate(contentInfo, type);
        }

        private void SystemSettings_FontSizeChanged(object sender, FontSizeChangedEventArgs e)
        {
            SystemSettingsFontSize fontSize = SystemSettings.FontSize;
            string fontType = SystemSettings.FontType;

            mFontItem.SubText = fontSize.ToString() + ", " + fontType;
        }

        private void SystemSettings_FontTypeChanged(object sender, FontTypeChangedEventArgs e)
        {
            SystemSettingsFontSize fontSize = SystemSettings.FontSize;
            string fontType = SystemSettings.FontType;

            mFontItem.SubText = fontSize.ToString() + ", " + fontType;
        }
        private void SystemSettings_ScreenBacklightTimeChanged(object sender, ScreenBacklightTimeChangedEventArgs e)
        {
            mScreenTimeoutItem.SubText = SettingContent_ScreenTimeout.GetScreenTimeoutName();
            
        }


        private void OnValueChanged(object sender, SliderValueChangedEventArgs args)
        {
        }

        private void OnSlidingStarted(object sender, SliderSlidingStartedEventArgs args)
        {
        }

        private void OnSlidingFinished(object sender, SliderSlidingFinishedEventArgs args)
        {
            var slider = sender as Slider;

            int minbrightness = 1;
            int maxbrightness = Display.Displays[0].MaxBrightness;
                        
            int brightness = (int)(slider.CurrentValue * (maxbrightness- minbrightness)) + minbrightness;
            if (brightness >= maxbrightness) brightness = maxbrightness;

            Tizen.Log.Debug("NUI", string.Format("maxbrightness : {0}, brightness : {1}", maxbrightness, brightness));

            if (mBrightnessItem != null)
            {
                GetBrightnessSliderIcon(brightness, out string iconpath);
                mBrightnessItem.mIcon.SetImage(iconpath);
           }

            try
            {
                Display.Displays[0].Brightness = brightness;
            }
            catch (Exception e)
            {
                Tizen.Log.Debug("NUI", string.Format("error :({0}) {1} ", e.GetType().ToString(), e.Message));
            }
        }

    }
}
