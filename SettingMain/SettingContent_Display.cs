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
            "brightness_icon/settings_ic_brightness_02.png",
            "brightness_icon/settings_ic_brightness_03.png",
            "brightness_icon/settings_ic_brightness_04.png",
            "brightness_icon/settings_ic_brightness_05.png",
            "brightness_icon/settings_ic_brightness_06.png",
            "brightness_icon/settings_ic_brightness_07.png",
            "brightness_icon/settings_ic_brightness_08.png",
            "brightness_icon/settings_ic_brightness_09.png",
            "brightness_icon/settings_ic_brightness_10.png",
            "brightness_icon/settings_ic_brightness_11.png"
        };
        static void GetBrightnessSliderIcon(int level, out string iconpath) 
        {

            int mapped_level;
            if (level <= 1)
		        mapped_level = 0;
	        else if (level >= 100)
		        mapped_level = 11;
	        else if (level > 1 && level <= 9)
		        mapped_level = 1;
	        else
		        mapped_level = (level / 10);

	        Tizen.Log.Debug("NUI", "mapped_level:"+mapped_level.ToString());

            iconpath = resPath + SETTING_ICON_PATH_CFG + mIconPath[mapped_level];
        }


        private DefaultLinearItem mFontItem;
        private DefaultLinearItem mScreenTimeoutItem;

        public SettingContent_Display()
            : base()
        {
            mTitle = Resources.IDS_ST_HEADER_DISPLAY;

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

            int level = 50;

            GetBrightnessSliderIcon(level, out string iconpath);

            item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_BRIGHTNESS_M_POWER_SAVING);
            content.Add(item);
            var slideritem = SettingItemCreator.CreateSliderItem("BRIGHTNESS", iconpath, 100);
            content.Add(slideritem);



            SystemSettingsFontSize fontSize = SystemSettings.FontSize;
            string fontType = SystemSettings.FontType;

            item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_FONT, fontSize.ToString() + ", " + fontType);
            mFontItem = item;
            if (item)
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
    }
}
