using SettingAppTextResopurces.TextResources;
using SettingCore.Views;
using SettingMainGadget;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.System;

namespace Setting.Menu.Display
{
    public class DisplayFontGadget : SettingCore.MenuGadget
    {
        public override string ProvideTitle() => Resources.IDS_ST_BODY_FONT;
        private TextListItem fontSizeItem;
        private TextListItem fontTypeItem;
        private View content;

        protected override void OnDestroy()
        {
            SystemSettings.FontSizeChanged -= SystemSettings_FontSizeChanged;
            SystemSettings.FontTypeChanged -= SystemSettings_FontTypeChanged;

            base.OnDestroy();
        }

        private void SystemSettings_FontSizeChanged(object sender, FontSizeChangedEventArgs e)
        {
            CreateItems();
        }

        private void SystemSettings_FontTypeChanged(object sender, FontTypeChangedEventArgs e)
        {
            if (fontTypeItem != null)
                fontTypeItem.Secondary = SystemSettings.FontType.ToString();
        }

        private void CreateItems()
        {
            if(fontSizeItem != null)
            {
                content.Remove(fontSizeItem);
            }

            fontSizeItem = TextListItem.CreatePrimaryTextItemWithSecondaryText(Resources.IDS_ST_MBODY_FONT_SIZE, SystemSettings.FontSize.ToString());
            fontSizeItem.Clicked += (o, e) =>
            {
                NavigateTo(MainMenuProvider.Display_FontSize);
            };
            content.Add(fontSizeItem);

            if (fontTypeItem != null)
            {
                content.Remove(fontTypeItem);
            }

            fontTypeItem = TextListItem.CreatePrimaryTextItemWithSecondaryText(Resources.IDS_ST_BODY_FONT_TYPE, SystemSettings.FontType.ToString());
            fontTypeItem.Clicked += (o, e) =>
            {
                NavigateTo(MainMenuProvider.Display_FontType);
            };
            content.Add(fontTypeItem);
        }

        protected override View OnCreate()
        {
            base.OnCreate();

            content = new View()
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
                Layout = new LinearLayout()
                {
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                },
            };

            CreateItems();

            SystemSettings.FontSizeChanged += SystemSettings_FontSizeChanged;
            SystemSettings.FontTypeChanged += SystemSettings_FontTypeChanged;

            return content;
        }
    }
}
