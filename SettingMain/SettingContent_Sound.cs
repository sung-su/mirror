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
    class SettingContent_Sound : SettingContent_Base
    {
        public SettingContent_Sound()
            : base()
        {
            mTitle = Resources.IDS_ST_HEADER_SOUND;
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




            item = CreateItemWithCheck(Resources.IDS_ST_HEADER_SOUND_MODE, Resources.IDS_ST_HEADER_UNAVAILABLE);
            content.Add(item);

            item = CreateItemWithCheck(Resources.IDS_ST_BODY_NOTIFICATIONS, Resources.IDS_ST_HEADER_UNAVAILABLE);
            content.Add(item);

            item = CreateItemWithCheck(Resources.IDS_ST_MBODY_OTHER_SOUNDS);
            content.Add(item);

            
            
            item = CreateItemWithCheck(Resources.IDS_ST_BODY_MEDIA);
            content.Add(item);
            var slideritem = CreateSliderItem("MEDIA", resPath + SETTING_ICON_PATH_CFG + "sound_slider_icon_default.png");
            content.Add(slideritem);

            item = CreateItemWithCheck(Resources.IDS_ST_BODY_NOTIFICATIONS);
            content.Add(item);
            slideritem = CreateSliderItem("MEDIA", resPath + SETTING_ICON_PATH_CFG + "sound_slider_icon_default.png");
            content.Add(slideritem);

            item = CreateItemWithCheck(Resources.IDS_ST_BODY_SYSTEM);
            content.Add(item);
            slideritem = CreateSliderItem("MEDIA", resPath + SETTING_ICON_PATH_CFG + "sound_slider_icon_default.png");
            content.Add(slideritem);

            return content;
        }

    }
}
