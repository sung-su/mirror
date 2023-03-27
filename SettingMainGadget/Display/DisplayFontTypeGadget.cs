using SettingAppTextResopurces.TextResources;
using SettingCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.System;

namespace Setting.Menu.Display
{
    public class DisplayFontTypeGadget : SettingCore.MenuGadget
    {
        public override string ProvideTitle() => Resources.IDS_ST_BODY_FONT_TYPE;

        protected override View OnCreate()
        {
            base.OnCreate();

            var content = new View()
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
                Layout = new LinearLayout()
                {
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                },
            };

            var fontTypeList = MakeFontTypeList();

            RadioButtonGroup radioButtonGroup = new RadioButtonGroup();

            for (int i = 0; i < fontTypeList.Count; i++)
            {
                RadioButton radioButton = new RadioButton()
                {
                    ThemeChangeSensitive = true,
                    Text = fontTypeList[i],
                    IsSelected = fontTypeList[i] == SystemSettings.FontType,
                    Margin = new Extents(24, 0, 0, 0).SpToPx(),
                };
                radioButton.TextLabel.FontFamily = fontTypeList[i];

                radioButtonGroup.Add(radioButton);
                content.Add(radioButton);
            }

            radioButtonGroup.SelectedChanged += (o, e) =>
            {
                SetFonttype(fontTypeList[radioButtonGroup.SelectedIndex]);
            };

            return content;
        }

        private List<string> MakeFontTypeList()
        {
            var systemFonts = FontClient.Instance.GetSystemFonts();

            // only fonts from /usr/share/fonts/ folder are works properly.
            // GetSystemFonts() also returns fonts from /usr/share/fallback_fonts/ folder, but with wrong family name.
            var fontTypeList = systemFonts.Where(x => x.Path.Contains("/usr/share/fonts/")).Select(a => a.Family).Distinct().ToList();

            if (SystemSettings.FontType.Equals("Default"))
            {
                SetFonttype(SystemSettings.DefaultFontType);
            }

            Logger.Debug($"SystemSettings.DefaultFontType: {SystemSettings.DefaultFontType}");
            Logger.Debug($"SystemSettings.FontType: {SystemSettings.FontType}");

            return fontTypeList;
        }

        private void SetFonttype(string fonttype)
        {
            try
            {
                SystemSettings.FontType = fonttype;
                Logger.Debug($"FontType value changed to: {fonttype}");
            }
            catch (Exception ex)
            {
                Logger.Error($"{ex.GetType()}, {ex.Message}");
            }
        }
    }
}
