using SettingCore.TextResources;
using SettingCore;
using SettingCore.Views;
using SettingMainGadget;
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
        private Sections sections = new Sections();

        private TextListItem soundMode;
        private TextListItem notificationSound;

        public override Color ProvideIconColor() => new Color(IsLightTheme  ? "#DB3069" : "#DF4679");

        public override string ProvideIconPath() => GetResourcePath("sound.svg");

        public override string ProvideTitle() => Resources.IDS_ST_HEADER_SOUND;

        protected override View OnCreate()
        {
            base.OnCreate();
            AttachToEvents();

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
            DetachFromEvents();
            base.OnDestroy();
        }

        private void AttachToEvents()
        {
            try
            {
                Tizen.System.SystemSettings.SoundSilentModeSettingChanged += SystemSettings_SoundSilentModeSettingChanged;
            }
            catch (System.Exception e)
            {
                Logger.Warn($"Cannot attach to SystemSettings.SoundSilentModeSettingChanged ({e.GetType()})");
            }

            try
            {
                Tizen.System.SystemSettings.VibrationChanged += SystemSettings_VibrationChanged;
            }
            catch (System.Exception e)
            {
                Logger.Warn($"Cannot attach to SystemSettings.VibrationChanged ({e.GetType()})");
            }

            try
            {
                Tizen.System.SystemSettings.SoundNotificationChanged += SystemSettings_NotificationSoundChanged;
            }
            catch (System.Exception e)
            {
                Logger.Warn($"Cannot attach to SystemSettings.SoundNotificationChanged ({e.GetType()})");
            }
        }

        private void DetachFromEvents()
        {
            try
            {
                Tizen.System.SystemSettings.SoundSilentModeSettingChanged -= SystemSettings_SoundSilentModeSettingChanged;
            }
            catch (System.Exception e)
            {
                Logger.Warn($"Cannot detach from SystemSettings.SoundSilentModeSettingChanged ({e.GetType()})");
            }

            try
            {
                Tizen.System.SystemSettings.VibrationChanged -= SystemSettings_VibrationChanged;
            }
            catch (System.Exception e)
            {
                Logger.Warn($"Cannot detach from SystemSettings.VibrationChanged ({e.GetType()})");
            }

            try
            {
                Tizen.System.SystemSettings.SoundNotificationChanged -= SystemSettings_NotificationSoundChanged;
            }
            catch (System.Exception e)
            {
                Logger.Warn($"Cannot detach from SystemSettings.SoundNotificationChanged ({e.GetType()})");
            }
        }

        private void CreateView()
        {
            // remove all sections from content view
            sections.RemoveAllSectionsFromView(content);

            // section: sound mode

            string soundModeName = SoundmodeManager.GetSoundmodeName(SoundmodeManager.GetSoundmode());
            soundMode = TextListItem.CreatePrimaryTextItemWithSecondaryText(Resources.IDS_ST_HEADER_SOUND_MODE, soundModeName);
            if (soundMode != null)
            {
                soundMode.Clicked += (o, e) =>
                {
                    NavigateTo(MainMenuProvider.Sound_Mode);
                };
            }
            sections.Add(MainMenuProvider.Sound_Mode, soundMode);

            // section: notification sound

            string notificationSoundName = SoundNotificationManager.GetNotificationSoundName();
            notificationSound = TextListItem.CreatePrimaryTextItemWithSecondaryText(Resources.IDS_ST_BODY_NOTIFICATIONS, notificationSoundName);
            if (notificationSound != null)
            {
                notificationSound.Clicked += (o, e) =>
                {
                    NavigateTo(MainMenuProvider.Sound_Notification);
                };
            }
            sections.Add(MainMenuProvider.Sound_Notification, notificationSound);

            // section: other sounds

            var otherSounds = TextListItem.CreatePrimaryTextItem(Resources.IDS_ST_MBODY_OTHER_SOUNDS);
            if (otherSounds != null)
            {
                otherSounds.Clicked += (o, e) =>
                {
                    NavigateTo(MainMenuProvider.Sound_Other);
                };
            }
            sections.Add(MainMenuProvider.Sound_Other, otherSounds);

            Logger.Debug($"GET {AudioVolumeType.Media} Volume : {SettingAudioManager.GetVolumeLevel(AudioVolumeType.Media)}");
            Logger.Debug($"GET {AudioVolumeType.Notification} Volume : {SettingAudioManager.GetVolumeLevel(AudioVolumeType.Notification)}");
            Logger.Debug($"GET {AudioVolumeType.System} Volume : {SettingAudioManager.GetVolumeLevel(AudioVolumeType.System)}");

            string soundSliderIconPath = GetResourcePath("sound/sound_slider_icon_default.png");

            // section: media

            var mediaSlider = new SliderListItem(Resources.IDS_ST_BODY_MEDIA, soundSliderIconPath, SettingAudioManager.GetPercentageVolumeLevel(AudioVolumeType.Media));
            mediaSlider.Slider.SlidingFinished += OnMediaSlidingFinished;
            mediaSlider.Margin = new Extents(0, 0, 16, 0).SpToPx();
            sections.Add(MainMenuProvider.Sound_MediaSlider, mediaSlider);

            // section: notification

            var notificationSlider = new SliderListItem(Resources.IDS_ST_BODY_NOTIFICATIONS, soundSliderIconPath, SettingAudioManager.GetPercentageVolumeLevel(AudioVolumeType.Notification));
            notificationSlider.Slider.ValueChanged += OnNofificationSlider_ValueChanged;
            notificationSlider.Margin = new Extents(0, 0, 16, 0).SpToPx();
            sections.Add(MainMenuProvider.Sound_NotificationSlider, notificationSlider);

            // section: system

            var systemSlider = new SliderListItem(Resources.IDS_ST_BODY_SYSTEM, soundSliderIconPath, SettingAudioManager.GetPercentageVolumeLevel(AudioVolumeType.System));
            systemSlider.Slider.ValueChanged += OnSystemSlider_ValueChanged;
            systemSlider.Margin = new Extents(0, 0, 16, 0).SpToPx();
            sections.Add(MainMenuProvider.Sound_SystemSlider, systemSlider);

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
                soundMode.Secondary = SoundmodeManager.GetSoundmodeName(SoundmodeManager.GetSoundmode());
        }

        private void SystemSettings_VibrationChanged(object sender, VibrationChangedEventArgs e)
        {
            if (soundMode != null)
                soundMode.Secondary = SoundmodeManager.GetSoundmodeName(SoundmodeManager.GetSoundmode());
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
                notificationSound.Secondary = SoundNotificationManager.GetNotificationSoundName();
        }
    }
}
