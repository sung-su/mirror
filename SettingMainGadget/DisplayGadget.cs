using SettingAppTextResopurces.TextResources;
using SettingCore;
using SettingMain;
using SettingMainGadget.Display;
using System;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.System;

namespace Setting.Menu
{
    public class DisplayGadget : SettingCore.MainMenuGadget
    {
        public override Color ProvideIconColor() => new Color("#0075FF");

        public override string ProvideIconPath() => "main-menu-icons/display.svg";

        public override string ProvideTitle() => "Display";

        private SliderItem brightnessItem;
        private DefaultLinearItem fontItem;
        private DefaultLinearItem screenTimeOutItem;
        private DefaultLinearItem themeItem;

        private static readonly string[] iconPath = {
            "display/Brightness_0.svg",
            "display/Brightness_50.svg",
            "display/Brightness_100.svg",
        };

        protected override View OnCreate()
        {
            SystemSettings.ScreenBacklightTimeChanged += SystemSettings_ScreenBacklightTimeChanged;
            SystemSettings.FontSizeChanged += SystemSettings_FontSizeChanged;
            SystemSettings.FontTypeChanged += SystemSettings_FontTypeChanged;

            return CreateView();
        }

        protected override void OnDestroy()
        {
            SystemSettings.ScreenBacklightTimeChanged -= SystemSettings_ScreenBacklightTimeChanged;
            SystemSettings.FontSizeChanged -= SystemSettings_FontSizeChanged;
            SystemSettings.FontTypeChanged -= SystemSettings_FontTypeChanged;

            base.OnDestroy();
        }

        private View CreateView()
        {
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

            if (Tizen.System.Display.NumberOfDisplays > 0)
            {
                int brightness = 0;
                int maxbrightness = 1;
                try
                {
                    brightness = Tizen.System.Display.Displays[0].Brightness;
                    maxbrightness = Tizen.System.Display.Displays[0].MaxBrightness;

                    Logger.Debug($"Current-Max display brightness: {brightness}-{maxbrightness}");
                }
                catch (Exception ex)
                {
                    Logger.Error($"Error while getting the display brightness: {ex.GetType()}, {ex.Message}");
                }

                GetBrightnessSliderIcon(brightness, out string iconpath);

                content.Add(new TextLabel 
                { 
                    Text = Resources.IDS_ST_BODY_BRIGHTNESS_M_POWER_SAVING,
                    Margin = new Extents(20, 20, 5, 5),
                });

                brightnessItem = SettingItemCreator.CreateSliderItem("BRIGHTNESS", iconpath, (brightness * 1.0f) / maxbrightness);
                if (brightnessItem != null)
                {
                    brightnessItem.mSlider.ValueChanged += MSlider_ValueChanged;
                    content.Add(brightnessItem);
                }
            }

            fontItem = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_FONT, $"{SystemSettings.FontSize}, {SystemSettings.FontType}");
            if (fontItem != null)
            {
                fontItem.Clicked += (o, e) =>
                {
                    NavigateTo("Setting.Menu.Display.Font");
                };
                content.Add(fontItem);
            }

            screenTimeOutItem = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_SCREEN_TIMEOUT_ABB2, DisplayTimeOutManager.GetScreenTimeoutName());
            if (screenTimeOutItem != null)
            {
                screenTimeOutItem.Clicked += (o, e) =>
                {
                    NavigateTo("Setting.Menu.Display.TimeOut");
                };
                content.Add(screenTimeOutItem);
            }

            themeItem = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_THEME);
            if (themeItem != null)
            {
                themeItem.Clicked += (o, e) =>
                {
                    NavigateTo("Setting.Menu.Display.Theme");
                };
                content.Add(themeItem);
            }

            return content;
        }

        private void GetBrightnessSliderIcon(int brightness, out string iconpath)
        {
            int iconlevel = iconPath.Length;

            int mapped_level = 0;
            if (iconlevel > 1)
            {
                int minbrightness = 1;
                int maxbrightness = Tizen.System.Display.Displays[0].MaxBrightness;
                if (brightness > minbrightness)
                {
                    int levelcount = maxbrightness - minbrightness;
                    int level = brightness - (minbrightness + 1);
                    mapped_level = (level * (iconlevel - 1) / levelcount) + 1;
                }
            }
            Logger.Debug($"mapped_level: {mapped_level} {System.IO.Path.Combine(Tizen.Applications.Application.Current.DirectoryInfo.Resource, iconPath[mapped_level])}");

            iconpath = System.IO.Path.Combine(Tizen.Applications.Application.Current.DirectoryInfo.Resource, iconPath[mapped_level]);
        }

        private void MSlider_ValueChanged(object sender, SliderValueChangedEventArgs args)
        {
            var slider = sender as Slider;

            int minbrightness = 1;
            int maxbrightness = Tizen.System.Display.Displays[0].MaxBrightness;

            int brightness = (int)(slider.CurrentValue * (maxbrightness - minbrightness)) + minbrightness;
            if (brightness >= maxbrightness) brightness = maxbrightness;

            Logger.Debug($"maxbrightness : {maxbrightness}, brightness : {brightness}");

            if (brightnessItem != null)
            {
                GetBrightnessSliderIcon(brightness, out string iconpath);
                brightnessItem.mIcon.SetImage(iconpath);
            }

            try
            {
                Tizen.System.Display.Displays[0].Brightness = brightness;
            }
            catch (Exception e)
            {
                Logger.Error(string.Format("error :({0}) {1} ", e.GetType().ToString(), e.Message));
            }
        }

        private void SystemSettings_ScreenBacklightTimeChanged(object sender, ScreenBacklightTimeChangedEventArgs e)
        {
            if (screenTimeOutItem != null)
                screenTimeOutItem.SubText = DisplayTimeOutManager.GetScreenTimeoutName();
        }

        private void SystemSettings_FontSizeChanged(object sender, FontSizeChangedEventArgs e)
        {
            if (fontItem != null)
                fontItem.SubText = $"{SystemSettings.FontSize}, {SystemSettings.FontType}";
        }

        private void SystemSettings_FontTypeChanged(object sender, FontTypeChangedEventArgs e)
        {
            if (fontItem != null)
                fontItem.SubText = $"{SystemSettings.FontSize}, {SystemSettings.FontType}";
        }
    }
}
