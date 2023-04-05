using SettingCore.TextResources;
using SettingCore.Views;
using SettingMainGadget.LanguageInput;
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
                    HorizontalAlignment = HorizontalAlignment.Begin,
                    VerticalAlignment = VerticalAlignment.Top,
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                },
            };

            RadioButtonGroup radioButtonGroup = new RadioButtonGroup();

            for (int i = 0; i < LanguageInputDisplayLanguageManager.LanguageList.Count; i++)
            {
                RadioButtonListItem item = new RadioButtonListItem(LanguageInputDisplayLanguageManager.LanguageList[i].GetName());
                item.RadioButton.IsSelected = i.Equals(LanguageInputDisplayLanguageManager.GetDisplayLanguageIndex());

                radioButtonGroup.Add(item.RadioButton);
                content.Add(item);
            }

            radioButtonGroup.SelectedChanged += (o, e) =>
            {
                LanguageInputDisplayLanguageManager.SetDisplayLanguage(LanguageInputDisplayLanguageManager.LanguageList[radioButtonGroup.SelectedIndex].GetLocale());
            };

            return content;
        }
    }
}
