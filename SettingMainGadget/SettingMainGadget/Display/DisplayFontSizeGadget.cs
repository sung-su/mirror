using SettingMainGadget.TextResources;
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
        public override string ProvideTitle() => NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_MBODY_FONT_SIZE));

        internal class FontsizeInfo
        {
            static Dictionary<SystemSettingsFontSize, string> fontSizeResourceKeys = new Dictionary<SystemSettingsFontSize, string>
            {
                { SystemSettingsFontSize.Small, nameof(Resources.IDS_ST_MBODY_FONT_SIZE_SMALL)},
                { SystemSettingsFontSize.Normal, nameof(Resources.IDS_ST_MBODY_FONT_SIZE_NORMAL)},
                { SystemSettingsFontSize.Large, nameof(Resources.IDS_ST_MBODY_FONT_SIZE_LARGE)},
                { SystemSettingsFontSize.Huge, nameof(Resources.IDS_ST_MBODY_FONT_SIZE_HUGE)},
                { SystemSettingsFontSize.Giant, nameof(Resources.IDS_ST_MBODY_FONT_SIZE_GIANT)}
            };

            private readonly SystemSettingsFontSize Value;

            public FontsizeInfo(SystemSettingsFontSize value)
            {
                Value = value;
            }

            public string GetName(MenuGadget gadget)
            {
                return GetName(gadget, Value);
            }

            public static string GetName(MenuGadget gadget, SystemSettingsFontSize fontSize)
            {
                if (fontSizeResourceKeys.TryGetValue(fontSize, out string fontResourceKey))
                {
                    return gadget.NUIGadgetResourceManager.GetString(fontResourceKey);
                }
                return fontSize.ToString();
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
                RadioButtonListItem item = new RadioButtonListItem(fontsizeList[i].GetName(this));
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
