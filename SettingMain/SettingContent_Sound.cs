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




            item = CreateItemWithCheck(Resources.IDS_ST_HEADER_SOUND_MODE, SettingContent_Soundmode.GetSoundmodeName());
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    // Update Widget Content by sending message to add the third page in advance.
                    Bundle nextBundle = new Bundle();
                    nextBundle.AddItem("WIDGET_ID", "soundmode@org.tizen.cssettings");
                    nextBundle.AddItem("WIDGET_WIDTH", window.Size.Width.ToString());
                    nextBundle.AddItem("WIDGET_HEIGHT", window.Size.Height.ToString());
                    nextBundle.AddItem("WIDGET_PAGE", "CONTENT_PAGE");
                    nextBundle.AddItem("WIDGET_ACTION", "PUSH");
                    String encodedBundle = nextBundle.Encode();
                    SetContentInfo(encodedBundle);
                };
                content.Add(item);
            }


            item = CreateItemWithCheck(Resources.IDS_ST_BODY_NOTIFICATIONS, SettingContent_NotificationSound.GetNotificationSoundName());
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    // Update Widget Content by sending message to add the third page in advance.
                    Bundle nextBundle = new Bundle();
                    nextBundle.AddItem("WIDGET_ID", "notificationsound@org.tizen.cssettings");
                    nextBundle.AddItem("WIDGET_WIDTH", window.Size.Width.ToString());
                    nextBundle.AddItem("WIDGET_HEIGHT", window.Size.Height.ToString());
                    nextBundle.AddItem("WIDGET_PAGE", "CONTENT_PAGE");
                    nextBundle.AddItem("WIDGET_ACTION", "PUSH");
                    String encodedBundle = nextBundle.Encode();
                    SetContentInfo(encodedBundle);
                };
                content.Add(item);
            }


            //item = CreateItemWithCheck(Resources.IDS_ST_MBODY_OTHER_SOUNDS);
            //content.Add(item);



            item = CreateItemWithCheck(Resources.IDS_ST_BODY_MEDIA);
            content.Add(item);
            var slideritem = CreateSliderItem("MEDIA", resPath + SETTING_ICON_PATH_CFG + "sound_slider_icon_default.png", 100);
            content.Add(slideritem);

            item = CreateItemWithCheck(Resources.IDS_ST_BODY_NOTIFICATIONS);
            content.Add(item);
            slideritem = CreateSliderItem("MEDIA", resPath + SETTING_ICON_PATH_CFG + "sound_slider_icon_default.png", 100);
            content.Add(slideritem);

            item = CreateItemWithCheck(Resources.IDS_ST_BODY_SYSTEM);
            content.Add(item);
            slideritem = CreateSliderItem("MEDIA", resPath + SETTING_ICON_PATH_CFG + "sound_slider_icon_default.png", 100);
            content.Add(slideritem);

            return content;
        }

    }
}
