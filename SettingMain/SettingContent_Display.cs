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
        private static string[] mIconPath  = {
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
        static void getBrightnessSliderIcon(int level, out string iconpath) 
        {

            int mapped_level = 0;

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

        public SettingContent_Display()
            : base()
        {
            mTitle = Resources.IDS_ST_HEADER_DISPLAY;
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

            string iconpath;
            getBrightnessSliderIcon(level, out iconpath);

            item = CreateItemWithCheck(Resources.IDS_ST_BODY_BRIGHTNESS_M_POWER_SAVING);
            content.Add(item);
            var slideritem = CreateSliderItem("BRIGHTNESS", iconpath, 100);
            content.Add(slideritem);



            SystemSettingsFontSize fontSize = SystemSettings.FontSize;
            string fontType = SystemSettings.FontType;

            item = CreateItemWithCheck(Resources.IDS_ST_BODY_FONT, fontSize.ToString() + ", " + fontType);
            if (item)
            {
                item.Clicked += (o, e) =>
                {
                    // Update Widget Content by sending message to add the third page in advance.
                    Bundle nextBundle = new Bundle();
                    nextBundle.AddItem("WIDGET_ID", "font@org.tizen.cssettings");
                    nextBundle.AddItem("WIDGET_WIDTH", window.Size.Width.ToString());
                    nextBundle.AddItem("WIDGET_HEIGHT", window.Size.Height.ToString());
                    nextBundle.AddItem("WIDGET_PAGE", "CONTENT_PAGE");
                    nextBundle.AddItem("WIDGET_ACTION", "PUSH");
                    String encodedBundle = nextBundle.Encode();
                    SetContentInfo(encodedBundle);

                };
                content.Add(item);
            }


            item = CreateItemWithCheck(Resources.IDS_ST_BODY_SCREEN_TIMEOUT_ABB2, SettingContent_ScreenTimeout.GetScreenTimeoutName());
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    // Update Widget Content by sending message to add the third page in advance.
                    Bundle nextBundle = new Bundle();
                    nextBundle.AddItem("WIDGET_ID", "timeout@org.tizen.cssettings");
                    nextBundle.AddItem("WIDGET_WIDTH", window.Size.Width.ToString());
                    nextBundle.AddItem("WIDGET_HEIGHT", window.Size.Height.ToString());
                    nextBundle.AddItem("WIDGET_PAGE", "CONTENT_PAGE");
                    nextBundle.AddItem("WIDGET_ACTION", "PUSH");
                    String encodedBundle = nextBundle.Encode();
                    SetContentInfo(encodedBundle);
                };
                content.Add(item);
            }


            item = CreateItemWithCheck(Resources.IDS_ST_BODY_THEME, SettingContent_Theme.GetThemeName());
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    // Update Widget Content by sending message to add the third page in advance.
                    Bundle nextBundle = new Bundle();
                    nextBundle.AddItem("WIDGET_ID", "theme@org.tizen.cssettings");
                    nextBundle.AddItem("WIDGET_WIDTH", window.Size.Width.ToString());
                    nextBundle.AddItem("WIDGET_HEIGHT", window.Size.Height.ToString());
                    nextBundle.AddItem("WIDGET_PAGE", "CONTENT_PAGE");
                    nextBundle.AddItem("WIDGET_ACTION", "PUSH");
                    String encodedBundle = nextBundle.Encode();
                    SetContentInfo(encodedBundle);
                };
                content.Add(item);
            }

            return content;
        }

    }
}
