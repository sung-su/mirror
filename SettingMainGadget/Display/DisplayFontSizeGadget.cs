using SettingCore.TextResources;
using SettingCore;
using SettingCore.Views;
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

            foreach (SystemSettingsFontSize fontSize in Enum.GetValues(typeof(SystemSettingsFontSize)))
            {
                fontsizeList.Add(new FontsizeInfo(fontSize));
            }

            RadioButtonGroup radioButtonGroup = new RadioButtonGroup();

            for (int i = 0; i < fontsizeList.Count; i++)
            {
                RadioButtonListItem item = new RadioButtonListItem(fontsizeList[i].GetName());
                item.RadioButton.IsSelected = fontsizeList[i].GetValue() == SystemSettings.FontSize;

                radioButtonGroup.Add(item.RadioButton);
                content.Add(item);
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
