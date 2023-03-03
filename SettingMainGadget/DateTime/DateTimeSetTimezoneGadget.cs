using SettingAppTextResopurces.TextResources;
using SettingCore;
using SettingMainGadget.DateTime;
using System.Collections.ObjectModel;
using System.Linq;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;

namespace Setting.Menu.DateTime
{
    public class DateTimeSetTimezoneGadget : SettingCore.MenuGadget
    {
        public override string ProvideTitle() => Resources.IDS_ST_BODY_TIME_ZONE;

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

            ReadOnlyCollection<string> rc = new ReadOnlyCollection<string>(DateTimeTimezoneManager.TimeZones.Select(x => x.Id).ToList());
            picker.DisplayedValues = rc;
            picker.MinValue = 0;
            picker.MaxValue = DateTimeTimezoneManager.TimeZones.Count - 1;
            picker.CurrentValue = DateTimeTimezoneManager.GetTimezoneIndex();

            var button = new Button()
            {
                Text = Resources.IDS_ST_BUTTON_OK
            };
            button.Clicked += (bo, be) =>
            {
                DateTimeTimezoneManager.SetTimezone(DateTimeTimezoneManager.TimeZones[picker.CurrentValue].Id);
                NavigateBack();
            };

            content.Add(picker);
            content.Add(button);

            return content;
        }
    }
}
