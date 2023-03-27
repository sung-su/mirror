using SettingAppTextResopurces.TextResources;
using SettingMainGadget.LanguageInput;
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
                    HorizontalAlignment = HorizontalAlignment.Begin,
                    VerticalAlignment = VerticalAlignment.Top,
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                },
            };

            RadioButtonGroup radioButtonGroup = new RadioButtonGroup();

            for (int i = 0; i < LanguageInputDisplayLanguageManager.LanguageList.Count; i++)
            {
                RadioButton radioButton = new RadioButton()
                {
                    ThemeChangeSensitive = true,
                    Text = LanguageInputDisplayLanguageManager.LanguageList[i].GetName(),
                    IsSelected = i.Equals(LanguageInputDisplayLanguageManager.GetDisplayLanguageIndex()),
                    Margin = new Extents(24, 0, 0, 0).SpToPx(),
                };

                radioButtonGroup.Add(radioButton);
                content.Add(radioButton);
            }

            radioButtonGroup.SelectedChanged += (o, e) =>
            {
                LanguageInputDisplayLanguageManager.SetDisplayLanguage(LanguageInputDisplayLanguageManager.LanguageList[radioButtonGroup.SelectedIndex].GetLocale());
            };

            return content;
        }
    }
}
