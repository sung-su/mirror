using SettingAppTextResopurces.TextResources;
using SettingCore;
using SettingMainGadget.Sound;
using Tizen.Multimedia;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;

namespace Setting.Menu
{
    public class SoundGadget : SettingCore.MainMenuGadget
    {
        private DefaultLinearItem soundMode;
        private DefaultLinearItem notificationSound;

        public override Color ProvideIconColor() => new Color("#DB3069");

        public override string ProvideIconPath() => "main-menu-icons/sound.svg";

        public override string ProvideTitle() => Resources.IDS_ST_HEADER_SOUND;

        protected override View OnCreate()
        {
            base.OnCreate();

            var content = new ScrollableBase
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

            string soundModeName = "TBU"; //SettingContent_Soundmode.GetSoundmodeName()
            soundMode = SettingMain.SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_HEADER_SOUND_MODE, soundModeName);
            if (soundMode != null)
            {
                soundMode.Clicked += (o, e) =>
                {
                    //RequestWidgetPush("soundmode@org.tizen.cssettings");
                };
                content.Add(soundMode);
            }

            string notificationSoundName = "TBU"; //SettingContent_NotificationSound.GetNotificationSoundName()
            notificationSound = SettingMain.SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_NOTIFICATIONS, notificationSoundName);
            if (notificationSound != null)
            {
                notificationSound.Clicked += (o, e) =>
                {
                    //RequestWidgetPush("notificationsound@org.tizen.cssettings");
                };
                content.Add(notificationSound);
            }

            var otherSounds = SettingMain.SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_MBODY_OTHER_SOUNDS);
            if (otherSounds != null)
            {
                otherSounds.Clicked += (o, e) =>
                {
                    //RequestWidgetPush("othersounds@org.tizen.cssettings");
                };
                content.Add(otherSounds);
            }

            Logger.Debug($"GET {AudioVolumeType.Media} Volume : {SettingAudioManager.GetVolumeLevel(AudioVolumeType.Media)}");
            Logger.Debug($"GET {AudioVolumeType.Notification} Volume : {SettingAudioManager.GetVolumeLevel(AudioVolumeType.Notification)}");
            Logger.Debug($"GET {AudioVolumeType.System} Volume : {SettingAudioManager.GetVolumeLevel(AudioVolumeType.System)}");

            string soundSliderIconPath = System.IO.Path.Combine(Tizen.Applications.Application.Current.DirectoryInfo.Resource, "sound/sound_slider_icon_default.png");

            var bodyMedia = SettingMain.SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_MEDIA);
            content.Add(bodyMedia);

            var slideritem = SettingMain.SettingItemCreator.CreateSliderItem("MEDIA", soundSliderIconPath, SettingAudioManager.GetPercentageVolumeLevel(AudioVolumeType.Media));
            if (slideritem != null)
            {
                slideritem.mSlider.SlidingFinished += OnMediaSlidingFinished;
                content.Add(slideritem);
            }

            var notifications = SettingMain.SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_NOTIFICATIONS);
            content.Add(notifications);

            slideritem = SettingMain.SettingItemCreator.CreateSliderItem("NOTI", soundSliderIconPath, SettingAudioManager.GetPercentageVolumeLevel(AudioVolumeType.Notification));
            if (slideritem != null)
            {
                slideritem.mSlider.ValueChanged += OnNofificationSlider_ValueChanged;
                content.Add(slideritem);
            }

            var bodySystem = SettingMain.SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_SYSTEM);
            content.Add(bodySystem);

            slideritem = SettingMain.SettingItemCreator.CreateSliderItem("SYSTEM", soundSliderIconPath, SettingAudioManager.GetPercentageVolumeLevel(AudioVolumeType.System));
            if (slideritem != null)
            {
                slideritem.mSlider.ValueChanged += OnSystemSlider_ValueChanged;
                content.Add(slideritem);
            }

            return content;
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
    }
}
