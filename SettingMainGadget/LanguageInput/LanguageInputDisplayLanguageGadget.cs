using SettingAppTextResopurces.TextResources;
using SettingMainGadget.LanguageInput;
using System.Collections.ObjectModel;
using System.Linq;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;

namespace Setting.Menu.LanguageInput
{
    public class LanguageInputDisplayLanguageGadget : SettingCore.MenuGadget
    {
        public override string ProvideTitle() => Resources.IDS_ST_HEADER_DISPLAY_LANGUAGE;

        protected override View OnCreate()
        {
            base.OnCreate();

            var content = new View()
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
                Layout = new LinearLayout()
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                },
            };

            var picker = new Picker()
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
            };

            picker.DisplayedValues = new ReadOnlyCollection<string>(LanguageInputDisplayLanguageManager.LanguageList.Select(s => s.GetName()).ToList());
            picker.MinValue = 0;
            picker.MaxValue = LanguageInputDisplayLanguageManager.LanguageList.Count - 1;
            picker.CurrentValue = LanguageInputDisplayLanguageManager.GetDisplayLanguageIndex();

            var button = new Button("Tizen.NUI.Components.Button.Outlined")
            {
                Text = Resources.IDS_ST_BUTTON_OK
            };
            button.Clicked += (bo, be) =>
            {
                LanguageInputDisplayLanguageManager.SetDisplayLanguage(LanguageInputDisplayLanguageManager.LanguageList[picker.CurrentValue].GetLocale());

                NavigateBack();
            };

            content.Add(picker);
            content.Add(button);

            return content;
        }
    }
}
