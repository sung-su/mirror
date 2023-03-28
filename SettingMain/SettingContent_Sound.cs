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
using Tizen.Multimedia;

using SettingCore.TextResources;

namespace SettingMain
{
    class SettingContent_Sound : SettingContent_Base
    {

        private static DefaultLinearItem mSoundModeItem;
        private static DefaultLinearItem mNotificationSoundItem;


        Vconf.NotificationCallback mNotiSoundCallback;

        public SettingContent_Sound()
            : base()
        {
            mTitle = Resources.IDS_ST_HEADER_SOUND;

            mSoundModeItem = null;
            mNotificationSoundItem = null;
            mNotiSoundCallback = VconfChanged_NotificationSound;
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


            Tizen.Log.Debug("NUI", $"GET {AudioVolumeType.Media} Volume : {SettingAudioManager.GetVolumeLevel(AudioVolumeType.Media)}");

            item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_MEDIA);
            content.Add(item);
            var slideritem = SettingItemCreator.CreateSliderItem("MEDIA", resPath + SETTING_ICON_PATH_CFG + "sound_slider_icon_default.png", SettingAudioManager.GetPercentageVolumeLevel(AudioVolumeType.Media));
            if (slideritem != null)
            {
                slideritem.mSlider.SlidingFinished += OnMediaSlidingFinished;

                content.Add(slideritem);
            }

            Tizen.Log.Debug("NUI", $"GET {AudioVolumeType.Notification} Volume : {SettingAudioManager.GetVolumeLevel(AudioVolumeType.Notification)}");

            item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_NOTIFICATIONS);
            content.Add(item);
            slideritem = SettingItemCreator.CreateSliderItem("NOTI", resPath + SETTING_ICON_PATH_CFG + "sound_slider_icon_default.png", SettingAudioManager.GetPercentageVolumeLevel(AudioVolumeType.Notification));
            if (slideritem != null)
            {
                slideritem.mSlider.ValueChanged += OnNofificationSlider_ValueChanged;

                content.Add(slideritem);
            }

            Tizen.Log.Debug("NUI", $"GET {AudioVolumeType.System} Volume : {SettingAudioManager.GetVolumeLevel(AudioVolumeType.System)}");

            item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_SYSTEM);
            content.Add(item);
            slideritem = SettingItemCreator.CreateSliderItem("SYSTEM", resPath + SETTING_ICON_PATH_CFG + "sound_slider_icon_default.png", SettingAudioManager.GetPercentageVolumeLevel(AudioVolumeType.System));
            if (slideritem != null)
            {
                slideritem.mSlider.ValueChanged += OnSystemSlider_ValueChanged;

                content.Add(slideritem);
            }

            return content;
        }


        protected override void OnCreate(string contentInfo, Window window)
        {
            base.OnCreate(contentInfo, window);

            Tizen.System.SystemSettings.SoundSilentModeSettingChanged += SystemSettings_SoundSilentModeSettingChanged;
            Tizen.System.SystemSettings.VibrationChanged += SystemSettings_VibrationChanged;

#if false
            Tizen.System.SystemSettings.SoundNotificationChanged += SystemSettings_NotificationSoundChanged;
#else
            Vconf.NotifyKeyChanged("db/setting/sound/noti/msg_ringtone_path", mNotiSoundCallback);
#endif
        }

        protected override void OnTerminate(string contentInfo, TerminationType type)
        {
            Tizen.System.SystemSettings.SoundSilentModeSettingChanged -= SystemSettings_SoundSilentModeSettingChanged;
            Tizen.System.SystemSettings.VibrationChanged -= SystemSettings_VibrationChanged;

#if false
            Tizen.System.SystemSettings.SoundNotificationChanged -= SystemSettings_NotificationSoundChanged;
#else
            Vconf.IgnoreKeyChanged("db/setting/sound/noti/msg_ringtone_path", mNotiSoundCallback);
#endif
            base.OnTerminate(contentInfo, type);
        }

        private static void SystemSettings_SoundSilentModeSettingChanged(object sender, SoundSilentModeSettingChangedEventArgs e)
        {
            if (mSoundModeItem != null)
                mSoundModeItem.SubText = SettingContent_Soundmode.GetSoundmodeName();
        }
        private static void SystemSettings_VibrationChanged(object sender, VibrationChangedEventArgs e)
        {
            if (mSoundModeItem != null)
                mSoundModeItem.SubText = SettingContent_Soundmode.GetSoundmodeName();
        }

#if false
        private static void SystemSettings_NotificationSoundChanged(object sender, SoundNotificationChangedEventArgs e)
        {
            if (mNotificationSoundItem != null)
                mNotificationSoundItem.SubText = SettingContent_NotificationSound.GetNotificationSoundName();
        }
#else
        public static void VconfChanged_NotificationSound(IntPtr node, IntPtr userData)
        {
            if (mNotificationSoundItem != null)
                mNotificationSoundItem.SubText = SettingContent_NotificationSound.GetNotificationSoundName();
        }
#endif

        /// ///////////////////////////

        private static void OnMediaSlidingFinished(object sender, SliderSlidingFinishedEventArgs e)
        {
            int volume = (int)(e.CurrentValue * SettingAudioManager.GetMaxVolumeLevel(AudioVolumeType.Media));

            if (volume != SettingAudioManager.GetVolumeLevel(AudioVolumeType.Media))
            {
                SettingAudioManager.SetVolumeLevel(AudioVolumeType.Media, volume);
                SettingAudioManager.PlayAudio(AudioStreamType.Media);
            }
        }
        private static void OnNofificationSlider_ValueChanged(object sender, SliderValueChangedEventArgs e)
        {
            int volume = (int)(e.CurrentValue * SettingAudioManager.GetMaxVolumeLevel(AudioVolumeType.Notification));

            if (volume != SettingAudioManager.GetVolumeLevel(AudioVolumeType.Notification))
            {
                SettingAudioManager.SetVolumeLevel(AudioVolumeType.Notification, volume);
                SettingAudioManager.PlayAudio(AudioStreamType.Notification);
            }
        }
        private static void OnSystemSlider_ValueChanged(object sender, SliderValueChangedEventArgs e)
        {
            int volume = (int)(e.CurrentValue * SettingAudioManager.GetMaxVolumeLevel(AudioVolumeType.System));

            if (volume != SettingAudioManager.GetVolumeLevel(AudioVolumeType.System))
            {
                SettingAudioManager.SetVolumeLevel(AudioVolumeType.System, volume);
                SettingAudioManager.PlayAudio(AudioStreamType.System);
            }
        }
    }
}
