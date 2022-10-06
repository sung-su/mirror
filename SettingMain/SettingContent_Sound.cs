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

        private DefaultLinearItem mSoundModeItem;
        private DefaultLinearItem mNotificationSoundItem;

        public SettingContent_Sound()
            : base()
        {
            mTitle = Resources.IDS_ST_HEADER_SOUND;

            mSoundModeItem = null;
            mNotificationSoundItem = null;
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


            item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_HEADER_SOUND_MODE, SettingContent_Soundmode.GetSoundmodeName());
            mSoundModeItem = item;
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    RequestWidgetPush("soundmode@org.tizen.cssettings");
                };
                content.Add(item);
            }


            item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_NOTIFICATIONS, SettingContent_NotificationSound.GetNotificationSoundName());
            mNotificationSoundItem = item;
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    RequestWidgetPush("notificationsound@org.tizen.cssettings");
                };
                content.Add(item);
            }


            item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_MBODY_OTHER_SOUNDS);
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    RequestWidgetPush("othersounds@org.tizen.cssettings");
                };
                content.Add(item);
            }




            item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_MEDIA);
            content.Add(item);
            var slideritem = SettingItemCreator.CreateSliderItem("MEDIA", resPath + SETTING_ICON_PATH_CFG + "sound_slider_icon_default.png", 100);
            content.Add(slideritem);

            item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_NOTIFICATIONS);
            content.Add(item);
            slideritem = SettingItemCreator.CreateSliderItem("MEDIA", resPath + SETTING_ICON_PATH_CFG + "sound_slider_icon_default.png", 100);
            content.Add(slideritem);

            item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_SYSTEM);
            content.Add(item);
            slideritem = SettingItemCreator.CreateSliderItem("MEDIA", resPath + SETTING_ICON_PATH_CFG + "sound_slider_icon_default.png", 100);
            content.Add(slideritem);

            return content;
        }


        protected override void OnCreate(string contentInfo, Window window)
        {
            base.OnCreate(contentInfo, window);

            Tizen.System.SystemSettings.SoundSilentModeSettingChanged += SystemSettings_SoundSilentModeSettingChanged;
            Tizen.System.SystemSettings.VibrationChanged += SystemSettings_VibrationChanged;

            Tizen.System.SystemSettings.SoundNotificationChanged += SystemSettings_NotificationSoundChanged;
        }

        protected override void OnTerminate(string contentInfo, TerminationType type)
        {
            Tizen.System.SystemSettings.SoundSilentModeSettingChanged -= SystemSettings_SoundSilentModeSettingChanged;
            Tizen.System.SystemSettings.VibrationChanged -= SystemSettings_VibrationChanged;

            Tizen.System.SystemSettings.SoundNotificationChanged -= SystemSettings_NotificationSoundChanged;

            base.OnTerminate(contentInfo, type);
        }

        private void SystemSettings_SoundSilentModeSettingChanged(object sender, SoundSilentModeSettingChangedEventArgs e)
        {
            mSoundModeItem.SubText = SettingContent_Soundmode.GetSoundmodeName();
        }
        private void SystemSettings_VibrationChanged(object sender, VibrationChangedEventArgs e)
        {
            mSoundModeItem.SubText = SettingContent_Soundmode.GetSoundmodeName();
        }

        private void SystemSettings_NotificationSoundChanged(object sender, SoundNotificationChangedEventArgs e)
        {
            mNotificationSoundItem.SubText = SettingContent_NotificationSound.GetNotificationSoundName();
        }

    }
}
