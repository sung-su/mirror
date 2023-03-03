using SettingAppTextResopurces.TextResources;
using SettingMain;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.System;

namespace Setting.Menu.Display
{
    public class DisplayFontGadget : SettingCore.MenuGadget
    {
        public override string ProvideTitle() => Resources.IDS_ST_BODY_FONT;
        private DefaultLinearItem fontSizeItem;
        private DefaultLinearItem fontTypeItem;

        protected override void OnDestroy()
        {
            SystemSettings.FontSizeChanged -= SystemSettings_FontSizeChanged;
            SystemSettings.FontTypeChanged -= SystemSettings_FontTypeChanged;

            base.OnDestroy();
        }

        private void SystemSettings_FontSizeChanged(object sender, FontSizeChangedEventArgs e)
        {
            if (fontSizeItem != null)
                fontSizeItem.SubText = SystemSettings.FontSize.ToString();
        }

        private void SystemSettings_FontTypeChanged(object sender, FontTypeChangedEventArgs e)
        {
            if (fontTypeItem != null)
                fontTypeItem.SubText = SystemSettings.FontType.ToString();
        }

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

            fontSizeItem = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_MBODY_FONT_SIZE, SystemSettings.FontSize.ToString());
            if (fontSizeItem != null)
            {
                fontSizeItem.Clicked += (o, e) =>
                {
                    NavigateTo("Setting.Menu.Display.Font.FontSize");
                };
                content.Add(fontSizeItem);
            }

            fontTypeItem = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_FONT_TYPE, SystemSettings.FontType.ToString());
            if (fontTypeItem != null)
            {
                fontTypeItem.Clicked += (o, e) =>
                {
                    NavigateTo("Setting.Menu.Display.Font.FontType");
                };
                content.Add(fontTypeItem);
            }

            SystemSettings.FontSizeChanged += SystemSettings_FontSizeChanged;
            SystemSettings.FontTypeChanged += SystemSettings_FontTypeChanged;

            return content;
        }
    }
}
