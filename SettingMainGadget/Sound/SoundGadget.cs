using SettingAppTextResopurces.TextResources;
using SettingCore;
using SettingCore.Customization;
using SettingMainGadget.Sound;
using System.Collections.Generic;
using System.Linq;
using Tizen.Multimedia;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.System;

namespace Setting.Menu
{
    public class SoundGadget : SettingCore.MainMenuGadget
    {
        private View content;
        private Dictionary<string, View> sections = new Dictionary<string, View>();

        private DefaultLinearItem soundMode;
        private DefaultLinearItem notificationSound;

        public override Color ProvideIconColor() => new Color("#DB3069");

        public override string ProvideIconPath() => GetResourcePath("sound.svg");

        public override string ProvideTitle() => Resources.IDS_ST_HEADER_SOUND;

        protected override View OnCreate()
        {
            base.OnCreate();

            Tizen.System.SystemSettings.SoundSilentModeSettingChanged += SystemSettings_SoundSilentModeSettingChanged;
            Tizen.System.SystemSettings.VibrationChanged += SystemSettings_VibrationChanged;
            Tizen.System.SystemSettings.SoundNotificationChanged += SystemSettings_NotificationSoundChanged;

            content = new ScrollableBase
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
            CreateView();

            return content;
        }

        protected override void OnDestroy()
        {
            Tizen.System.SystemSettings.SoundSilentModeSettingChanged -= SystemSettings_SoundSilentModeSettingChanged;
            Tizen.System.SystemSettings.VibrationChanged -= SystemSettings_VibrationChanged;
            Tizen.System.SystemSettings.SoundNotificationChanged -= SystemSettings_NotificationSoundChanged;

            base.OnDestroy();
        }

        private void CreateView()
        {
            // remove all sections from content view
            foreach (var section in sections)
            {
                content.Remove(section.Value);
            }
            sections.Clear();

            // section: sound mode

            string soundModeName = SoundmodeManager.GetSoundmodeName(SoundmodeManager.GetSoundmode());
            soundMode = SettingMain.SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_HEADER_SOUND_MODE, soundModeName);
            if (soundMode != null)
            {
                soundMode.Clicked += (o, e) =>
                {
                    NavigateTo("Setting.Menu.Sound.SoundMode");
                };
            }
            sections.Add("Setting.Menu.Sound.SoundMode", soundMode);

            // section: notification sound

            string notificationSoundName = SoundNotificationManager.GetNotificationSoundName();
            notificationSound = SettingMain.SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_NOTIFICATIONS, notificationSoundName);
            if (notificationSound != null)
            {
                notificationSound.Clicked += (o, e) =>
                {
                    NavigateTo("Setting.Menu.Sound.SoundNotification");
                };
            }
            sections.Add("Setting.Menu.Sound.SoundNotification", notificationSound);

            // section: other sounds

            var otherSounds = SettingMain.SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_MBODY_OTHER_SOUNDS);
            if (otherSounds != null)
            {
                otherSounds.Clicked += (o, e) =>
                {
                    NavigateTo("Setting.Menu.Sound.SoundOther");
                };
            }
            sections.Add("Setting.Menu.Sound.SoundOther", otherSounds);

            Logger.Debug($"GET {AudioVolumeType.Media} Volume : {SettingAudioManager.GetVolumeLevel(AudioVolumeType.Media)}");
            Logger.Debug($"GET {AudioVolumeType.Notification} Volume : {SettingAudioManager.GetVolumeLevel(AudioVolumeType.Notification)}");
            Logger.Debug($"GET {AudioVolumeType.System} Volume : {SettingAudioManager.GetVolumeLevel(AudioVolumeType.System)}");

            string soundSliderIconPath = GetResourcePath("sound/sound_slider_icon_default.png");

            // section: media

            var bodyMediaSection = new View
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                Layout = new LinearLayout
                {
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                },
            };

            var bodyMedia = SettingMain.SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_MEDIA);
            bodyMediaSection.Add(bodyMedia);

            var slideritem = SettingMain.SettingItemCreator.CreateSliderItem("MEDIA", soundSliderIconPath, SettingAudioManager.GetPercentageVolumeLevel(AudioVolumeType.Media));
            if (slideritem != null)
            {
                slideritem.mSlider.SlidingFinished += OnMediaSlidingFinished;
                bodyMediaSection.Add(slideritem);
            }
            sections.Add("Setting.Menu.Sound.Media", bodyMediaSection);

            // section: notification

            var notificationSection = new View
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                Layout = new LinearLayout
                {
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                },
            };

            var notifications = SettingMain.SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_NOTIFICATIONS);
            notificationSection.Add(notifications);

            slideritem = SettingMain.SettingItemCreator.CreateSliderItem("NOTI", soundSliderIconPath, SettingAudioManager.GetPercentageVolumeLevel(AudioVolumeType.Notification));
            if (slideritem != null)
            {
                slideritem.mSlider.ValueChanged += OnNofificationSlider_ValueChanged;
                notificationSection.Add(slideritem);
            }
            sections.Add("Setting.Menu.Sound.Notification", notificationSection);

            // section: system

            var systemSection = new View
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                Layout = new LinearLayout
                {
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                },
            };

            var bodySystem = SettingMain.SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_SYSTEM);
            systemSection.Add(bodySystem);

            slideritem = SettingMain.SettingItemCreator.CreateSliderItem("SYSTEM", soundSliderIconPath, SettingAudioManager.GetPercentageVolumeLevel(AudioVolumeType.System));
            if (slideritem != null)
            {
                slideritem.mSlider.ValueChanged += OnSystemSlider_ValueChanged;
                systemSection.Add(slideritem);
            }
            sections.Add("Setting.Menu.Sound.System", systemSection);

            // add only visible sections to content view in required order
            var customization = GetCustomization().OrderBy(c => c.Order);
            foreach (var cust in customization)
            {
                string visibility = cust.IsVisible ? "visible" : "hidden";
                Logger.Verbose($"Customization: {cust.MenuPath} - {visibility} - {cust.Order}");
                if (cust.IsVisible && sections.TryGetValue(cust.MenuPath, out View row))
                {
                    content.Add(row);
                }
            }
        }

        protected override void OnCustomizationUpdate(IEnumerable<MenuCustomizationItem> items)
        {
            Logger.Verbose($"{nameof(SoundGadget)} got customization with {items.Count()} items. Recreating view.");
            CreateView();
        }

        private void SystemSettings_SoundSilentModeSettingChanged(object sender, SoundSilentModeSettingChangedEventArgs e)
        {
            if (soundMode != null)
                soundMode.SubText = SoundmodeManager.GetSoundmodeName(SoundmodeManager.GetSoundmode());
        }

        private void SystemSettings_VibrationChanged(object sender, VibrationChangedEventArgs e)
        {
            if (soundMode != null)
                soundMode.SubText = SoundmodeManager.GetSoundmodeName(SoundmodeManager.GetSoundmode());
        }

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

        private void SystemSettings_NotificationSoundChanged(object sender, SoundNotificationChangedEventArgs e)
        {
            if (notificationSound != null)
                notificationSound.SubText = SoundNotificationManager.GetNotificationSoundName();
        }
    }
}
