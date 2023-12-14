using SettingMainGadget.TextResources;
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

        public override string ProvideTitle() => NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_HEADER_DISPLAY));

        private ScrollableBase content;

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

            CreateContent();

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

        private void CreateContent()
        {
            content.RemoveAllChildren(true);
            sections.Clear();

            // brightness section
            sections.Add(MainMenuProvider.Display_Brightness, () =>
            {
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

                    brightnessItem = new SliderListItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_BRIGHTNESS_M_POWER_SAVING)), iconpath, (brightness * 1.0f) / maxbrightness);
                    if (brightnessItem != null)
                    {
                        if (!isBrightnessSupported)
                        {
                            brightnessItem.Slider.CurrentValue = brightnessItem.Slider.MaxValue;
                        }

                        brightnessItem.Margin = new Extents(0, 0, 16, 0).SpToPx();
                        brightnessItem.Slider.ValueChanged += MSlider_ValueChanged;
                        brightnessItem.IsEnabled = isBrightnessSupported;
                        content.Add(brightnessItem);
                    }
                }
                else
                {
                    Logger.Warn($"There are no available displays. The Brightness section has not been created.");
                }
            });

            // font section
            sections.Add(MainMenuProvider.Display_Font, () =>
            {
                Logger.Performance($"CreateItems Display_Font");
                fontItem = TextListItem.CreatePrimaryTextItemWithSecondaryText(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_FONT)), $"{SystemSettings.FontSize}, {SystemSettings.FontType}");
                if (fontItem != null)
                {
                    fontItem.Clicked += (o, e) =>
                    {
                        NavigateTo(MainMenuProvider.Display_Font);
                    };
                    content.Add(fontItem);
                }
                Logger.Performance($"CreateItems Display_Font end");
            });

            // timeout section
            sections.Add(MainMenuProvider.Display_Timeout, () =>
            {
                screenTimeOutItem = TextListItem.CreatePrimaryTextItemWithSecondaryText(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_SCREEN_TIMEOUT_ABB2)), DisplayTimeOutManager.GetScreenTimeoutName(this));
                if (screenTimeOutItem != null)
                {
                    screenTimeOutItem.Clicked += (o, e) =>
                    {
                        NavigateTo(MainMenuProvider.Display_Timeout);
                    };
                    content.Add(screenTimeOutItem);
                }
            });

            // theme section
            sections.Add(MainMenuProvider.Display_Theme, () =>
            {
                themeItem = TextListItem.CreatePrimaryTextItemWithSecondaryText(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_THEME)), DisplayThemeManager.GetThemeName(this));
                if (themeItem != null)
                {
                    themeItem.Clicked += (o, e) =>
                    {
                        NavigateTo(MainMenuProvider.Display_Theme);
                    };
                    content.Add(themeItem);
                }
            });

            CreateItems();
        }

        protected override void OnCustomizationUpdate(IEnumerable<MenuCustomizationItem> items)
        {
            Logger.Verbose($"{nameof(DisplayGadget)} got customization with {items.Count()} items. Recreating view.");
            CreateContent();
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
                screenTimeOutItem.Secondary = DisplayTimeOutManager.GetScreenTimeoutName(this);
        }

        private void SystemSettings_FontSizeChanged(object sender, FontSizeChangedEventArgs e)
        {
            CreateContent(); // TODO : change only secondary text instead of re-create all view
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
                themeItem.Secondary = DisplayThemeManager.GetThemeName(this);

                // reassign CurrentValue to trigger slider icon path update
                if (brightnessItem != null)
                {
                    brightnessItem.Slider.CurrentValue = brightnessItem.Slider.CurrentValue;
                }
            }
        }
    }
}
