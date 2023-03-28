using SettingCore.TextResources;
using SettingCore;
using System;
using System.Collections.Generic;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.System;

namespace Setting.Menu.Display
{
    public class DisplayFontSizeGadget : SettingCore.MenuGadget
    {
        public override string ProvideTitle() => Resources.IDS_ST_MBODY_FONT_SIZE;

        private class FontsizeInfo
        {
            private readonly SystemSettingsFontSize Value;

            public FontsizeInfo(SystemSettingsFontSize value)
            {
                Value = value;
            }

            public string GetName()
            {
                return Value.ToString();
            }

            public SystemSettingsFontSize GetValue()
            {
                return Value;
            }
        };

        private List<FontsizeInfo> fontsizeList = new List<FontsizeInfo>();

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

            foreach (SystemSettingsFontSize fontSize in Enum.GetValues(typeof(SystemSettingsFontSize)))
            {
                fontsizeList.Add(new FontsizeInfo(fontSize));
            }

            RadioButtonGroup radioButtonGroup = new RadioButtonGroup();

            for (int i = 0; i < fontsizeList.Count; i++)
            {
                RadioButton radioButton = new RadioButton() 
                {
                    ThemeChangeSensitive = true,
                    Text = fontsizeList[i].GetName(),
                    IsSelected = fontsizeList[i].GetValue() == SystemSettings.FontSize,
                    Margin = new Extents(24, 0, 0, 0).SpToPx(),
                };

                radioButtonGroup.Add(radioButton);
                content.Add(radioButton);
            }

            radioButtonGroup.SelectedChanged += (o, e) =>
            {
                SetFontsize(fontsizeList[radioButtonGroup.SelectedIndex].GetValue());
            };

            return content;
        }

        private void SetFontsize(SystemSettingsFontSize fontsize)
        {
            SystemSettings.FontSize = fontsize;
            Logger.Debug($"FontSize value changed to: {fontsize}");
        }
    }
}
