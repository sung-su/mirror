using SettingCore.TextResources;
using SettingCore;
using SettingCore.Views;
using SettingMainGadget;
using SettingMainGadget.Display;
using System;
using System.Collections.Generic;
using System.Linq;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.System;

namespace Setting.Menu
{
    public class DisplayGadget : SettingCore.MainMenuGadget
    {
        public override Color ProvideIconColor() => new Color(IsLightTheme ? "#0075FF" : "#1A85FF");

        public override string ProvideIconPath() => GetResourcePath("display.svg");

        public override string ProvideTitle() => Resources.IDS_ST_HEADER_DISPLAY;

        private View content;
        private Sections sections = new Sections();

        private SliderListItem brightnessItem;
        private TextListItem fontItem;
        private TextListItem screenTimeOutItem;
        private TextListItem themeItem;

        private bool isBrightnessSupported;

        private static readonly string[] iconPath = {
            "display/Brightness_0.svg",
            "display/Brightness_50.svg",
            "display/Brightness_100.svg",
        };

        private static readonly string[] iconDisabledPath = {
            "display/Brightness_light.svg",
            "display/Brightness_dark.svg",
        };

        protected override View OnCreate()
        {
            base.OnCreate();

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

            SystemSettings.ScreenBacklightTimeChanged += SystemSettings_ScreenBacklightTimeChanged;
            SystemSettings.FontSizeChanged += SystemSettings_FontSizeChanged;
            SystemSettings.FontTypeChanged += SystemSettings_FontTypeChanged;
            ThemeManager.ThemeChanged += ThemeManager_ThemeChanged;

            CreateView();

            return content;
        }

        protected override void OnDestroy()
        {
            SystemSettings.ScreenBacklightTimeChanged -= SystemSettings_ScreenBacklightTimeChanged;
            SystemSettings.FontSizeChanged -= SystemSettings_FontSizeChanged;
            SystemSettings.FontTypeChanged -= SystemSettings_FontTypeChanged;
            ThemeManager.ThemeChanged -= ThemeManager_ThemeChanged;

            base.OnDestroy();
        }

        private void CreateView()
        {
            sections.RemoveAllSectionsFromView(content);

            // section: brightness

            if (Tizen.System.Display.NumberOfDisplays > 0)
            {
                int brightness = 0;
                int maxbrightness = 1;

                try
                {
                    Tizen.System.Display.Displays[0].Brightness = Tizen.System.Display.Displays[0].Brightness;
                    isBrightnessSupported = true;
                }
                catch (Exception)
                {
                    Logger.Warn($"Brightness is not supported on the device.");
                }

                
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

                brightnessItem = new SliderListItem(Resources.IDS_ST_BODY_BRIGHTNESS_M_POWER_SAVING, iconpath, (brightness * 1.0f) / maxbrightness);
                if (brightnessItem != null)
                {
                    if(!isBrightnessSupported)
                    {
                        brightnessItem.Slider.CurrentValue = brightnessItem.Slider.MaxValue;
                    }

                    brightnessItem.Margin = new Extents(0, 0, 16, 0).SpToPx();
                    brightnessItem.Slider.ValueChanged += MSlider_ValueChanged;
                    brightnessItem.IsEnabled = isBrightnessSupported;
                    sections.Add(MainMenuProvider.Display_Brightness, brightnessItem);
                }
            }

            // section: font

            fontItem = TextListItem.CreatePrimaryTextItemWithSecondaryText(Resources.IDS_ST_BODY_FONT, $"{SystemSettings.FontSize}, {SystemSettings.FontType}");
            if (fontItem != null)
            {
                fontItem.Clicked += (o, e) =>
                {
                    NavigateTo(MainMenuProvider.Display_Font);
                };
                sections.Add(MainMenuProvider.Display_Font, fontItem);
            }

            // section: TimeOut

            screenTimeOutItem = TextListItem.CreatePrimaryTextItemWithSecondaryText(Resources.IDS_ST_BODY_SCREEN_TIMEOUT_ABB2, DisplayTimeOutManager.GetScreenTimeoutName());
            if (screenTimeOutItem != null)
            {
                screenTimeOutItem.Clicked += (o, e) =>
                {
                    NavigateTo(MainMenuProvider.Display_Timeout);
                };
                sections.Add(MainMenuProvider.Display_Timeout, screenTimeOutItem);
            }

            // section: Theme

            themeItem = TextListItem.CreatePrimaryTextItemWithSecondaryText(Resources.IDS_ST_BODY_THEME, DisplayThemeManager.GetThemeName());
            if (themeItem != null)
            {
                themeItem.Clicked += (o, e) =>
                {
                    NavigateTo(MainMenuProvider.Display_Theme);
                };
                sections.Add(MainMenuProvider.Display_Theme, themeItem);
            }

            var customization = GetCustomization().OrderBy(c => c.Order);
            Logger.Debug($"customization: {customization.Count()}");
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
            Logger.Verbose($"{nameof(DisplayGadget)} got customization with {items.Count()} items. Recreating view.");
            CreateView();
        }

        private void GetBrightnessSliderIcon(int brightness, out string iconpath)
        {
            if(!isBrightnessSupported)
            {
                // TODO : create a theme helper that provides information about the current theme using an enum variable. 
                iconpath = System.IO.Path.Combine(Tizen.Applications.Application.Current.DirectoryInfo.Resource, iconDisabledPath[DisplayThemeManager.GetThemeIndex()]);
                return;
            }

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
                brightnessItem.IconPath = iconpath;
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
                screenTimeOutItem.Secondary = DisplayTimeOutManager.GetScreenTimeoutName();
        }

        private void SystemSettings_FontSizeChanged(object sender, FontSizeChangedEventArgs e)
        {
            CreateView(); // TODO : change only secondary text instead of re-create all view
        }

        private void SystemSettings_FontTypeChanged(object sender, FontTypeChangedEventArgs e)
        {
            if(fontItem != null)
                fontItem.Secondary = $"{SystemSettings.FontSize}, {SystemSettings.FontType}"; // TODO : show default BreezeSans if FontType = DefaultFontType
        }

        private void ThemeManager_ThemeChanged(object sender, ThemeChangedEventArgs e)
        {
            if (e.IsPlatformThemeChanged)
            {
                Logger.Debug($"theme changed to: {e.PlatformThemeId}");
                themeItem.Secondary = DisplayThemeManager.GetThemeName();

                // reassign CurrentValue to trigger slider icon path update
                brightnessItem.Slider.CurrentValue = brightnessItem.Slider.CurrentValue;
            }
        }
    }
}
