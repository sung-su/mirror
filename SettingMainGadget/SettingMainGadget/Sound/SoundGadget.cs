using SettingCore;
using SettingCore.Views;
using SettingMainGadget;
using SettingMainGadget.Sound;
using SettingMainGadget.TextResources;
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
        private const string soundSliderIconDefault = "sound/sound_slider_icon_default.svg";
        private const string soundSliderIconMute = "sound/sound_slider_icon_mute.svg";

        private ScrollableBase content;
        private AudioVolume audioVolume = AudioManager.VolumeController;

        private TextListItem soundMode;
        private TextListItem notificationSound;
        private SliderListItem mediaSlider;
        private SliderListItem notificationSlider;
        private SliderListItem systemSlider;

        private bool soundsEnabled;
        private string soundSliderIconPath;

        public override Color ProvideIconColor() => new Color(IsLightTheme  ? "#DB3069" : "#DF4679");

        public override string ProvideIconPath() => GetResourcePath("sound.svg");

        public override string ProvideTitle() => NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_HEADER_SOUND));

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

            CreateContent();

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

            try
            {
                audioVolume.Changed += AudioVolume_Changed;
            }
            catch (System.Exception e)
            {
                Logger.Warn($"Cannot attach to AudioVolume changed event ({e.GetType()})");
            }
        }

        private void AudioVolume_Changed(object sender, VolumeChangedEventArgs e)
        {
            if (e.Type == AudioVolumeType.Media && mediaSlider != null)
            {
                mediaSlider.Slider.CurrentValue = SettingAudioManager.GetPercentageVolumeLevel(e.Type);
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

            try
            {
                audioVolume.Changed -= AudioVolume_Changed;
            }
            catch (System.Exception e)
            {
                Logger.Warn($"Cannot detach to AudioVolume changed event ({e.GetType()})");
            }
        }

        private void CreateContent()
        {
            content.RemoveAllChildren(true);
            sections.Clear();

            Logger.Debug($"GET {AudioVolumeType.Media} Volume : {SettingAudioManager.GetVolumeLevel(AudioVolumeType.Media)}");
            Logger.Debug($"GET {AudioVolumeType.Notification} Volume : {SettingAudioManager.GetVolumeLevel(AudioVolumeType.Notification)}");
            Logger.Debug($"GET {AudioVolumeType.System} Volume : {SettingAudioManager.GetVolumeLevel(AudioVolumeType.System)}");

            soundsEnabled = SoundmodeManager.GetSoundmode() == Soundmode.SOUND_MODE_SOUND;
            soundSliderIconPath = soundsEnabled ? GetResourcePath(soundSliderIconDefault) : GetResourcePath(soundSliderIconMute);

            // section: sound mode
            sections.Add(MainMenuProvider.Sound_Mode, () =>
            {
                string soundModeName = SoundmodeManager.GetSoundmodeName(this, SoundmodeManager.GetSoundmode());
                soundMode = TextListItem.CreatePrimaryTextItemWithSecondaryText(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_HEADER_SOUND_MODE)), soundModeName);
                if (soundMode != null)
                {
                    soundMode.Clicked += (o, e) =>
                    {
                        NavigateTo(MainMenuProvider.Sound_Mode);
                    };
                    content.Add(soundMode);
                }
            });

            // section: notification sound
            sections.Add(MainMenuProvider.Sound_Notification, () =>
            {
                string notificationSoundName = SoundNotificationManager.GetNotificationSoundName(this);
                notificationSound = TextListItem.CreatePrimaryTextItemWithSecondaryText(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_NOTIFICATIONS)), notificationSoundName);
                if (notificationSound != null)
                {
                    notificationSound.Clicked += (o, e) =>
                    {
                        NavigateTo(MainMenuProvider.Sound_Notification);
                    };
                    content.Add(notificationSound);
                }
            });

            // section: other sounds
            sections.Add(MainMenuProvider.Sound_Other, () =>
            {
                var otherSounds = TextListItem.CreatePrimaryTextItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_MBODY_OTHER_SOUNDS)));
                if (otherSounds != null)
                {
                    otherSounds.Clicked += (o, e) =>
                    {
                        NavigateTo(MainMenuProvider.Sound_Other);
                    };
                    content.Add(otherSounds);
                }
            });

            // section: media
            sections.Add(MainMenuProvider.Sound_MediaSlider, () =>
            {
                mediaSlider = new SliderListItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_MEDIA)), GetResourcePath(soundSliderIconDefault), SettingAudioManager.GetPercentageVolumeLevel(AudioVolumeType.Media));
                mediaSlider.Slider.SlidingFinished += OnMediaSlidingFinished;
                mediaSlider.Margin = new Extents(0, 0, 16, 0).SpToPx();
                content.Add(mediaSlider);
            });

            // section: notification
            sections.Add(MainMenuProvider.Sound_NotificationSlider, () =>
            {
                notificationSlider = new SliderListItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_NOTIFICATIONS)), soundSliderIconPath, SettingAudioManager.GetPercentageVolumeLevel(AudioVolumeType.Notification));
                notificationSlider.Slider.ValueChanged += OnNofificationSlider_ValueChanged;
                notificationSlider.Slider.IsEnabled = soundsEnabled;
                notificationSlider.Margin = new Extents(0, 0, 16, 0).SpToPx();
                content.Add(notificationSlider);
            });

            // section: system
            sections.Add(MainMenuProvider.Sound_SystemSlider, () =>
            {
                systemSlider = new SliderListItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_SYSTEM)), soundSliderIconPath, SettingAudioManager.GetPercentageVolumeLevel(AudioVolumeType.System));
                systemSlider.Slider.ValueChanged += OnSystemSlider_ValueChanged;
                systemSlider.Slider.IsEnabled = soundsEnabled;
                systemSlider.Margin = new Extents(0, 0, 16, 0).SpToPx();
                content.Add(systemSlider);
            });

            CreateItems();
        }

        protected override void OnCustomizationUpdate(IEnumerable<MenuCustomizationItem> items)
        {
            Logger.Verbose($"{nameof(SoundGadget)} got customization with {items.Count()} items. Recreating view.");
            CreateContent();
        }

        private void SystemSettings_SoundSilentModeSettingChanged(object sender, SoundSilentModeSettingChangedEventArgs e)
        {
            if (soundMode != null)
                soundMode.Secondary = SoundmodeManager.GetSoundmodeName(this, SoundmodeManager.GetSoundmode());

            bool soundsEnabled = SoundmodeManager.GetSoundmode() == Soundmode.SOUND_MODE_SOUND;
            string soundSliderIconPath = soundsEnabled ? GetResourcePath(soundSliderIconDefault) : GetResourcePath(soundSliderIconMute);

            if (notificationSlider != null)
            {
                notificationSlider.Slider.IsEnabled = soundsEnabled;
                notificationSlider.IconPath = soundSliderIconPath;
                notificationSlider.Slider.CurrentValue = SettingAudioManager.GetPercentageVolumeLevel(AudioVolumeType.Notification);
            }

            if (systemSlider != null)
            {
                systemSlider.Slider.IsEnabled = soundsEnabled;
                systemSlider.IconPath = soundSliderIconPath;
                systemSlider.Slider.CurrentValue = SettingAudioManager.GetPercentageVolumeLevel(AudioVolumeType.System);
            }

            Logger.Debug($"Sound silent mode: {e.Value}");
        }

        private void SystemSettings_VibrationChanged(object sender, VibrationChangedEventArgs e)
        {
            if (soundMode != null)
                soundMode.Secondary = SoundmodeManager.GetSoundmodeName(this, SoundmodeManager.GetSoundmode());
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
                notificationSound.Secondary = SoundNotificationManager.GetNotificationSoundName(this);
        }
    }
}
