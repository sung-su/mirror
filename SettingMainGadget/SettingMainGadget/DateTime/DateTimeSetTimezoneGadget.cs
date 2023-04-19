using SettingMainGadget.TextResources;
using SettingCore.Views;
using SettingMainGadget.DateTime;
using System.Linq;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.System;

namespace Setting.Menu.DateTime
{
    public class DateTimeSetTimezoneGadget : SettingCore.MenuGadget
    {
        public override string ProvideTitle() => NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_TIME_ZONE));

        private ScrollableBase content = null;

        protected override View OnCreate()
        {
            base.OnCreate();

            content = new ScrollableBase
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

            RadioButtonGroup radioButtonGroup = new RadioButtonGroup();

            var timeZones = DateTimeTimezoneManager.GetTimeZones();
            timeZones = timeZones.OrderBy(a => a.City).ThenBy(x => x.Continent).ToList();

            foreach (var timeZone in timeZones)
            {
                RadioButtonListItem item = new RadioButtonListItem(timeZone.DisplayName);
                item.RadioButton.IsSelected = SystemSettings.LocaleTimeZone == timeZone.Info.Id;

                radioButtonGroup.Add(item.RadioButton);
                content.Add(item);
            }

            radioButtonGroup.SelectedChanged += (o, e) =>
            {
                DateTimeTimezoneManager.SetTimezone(timeZones[radioButtonGroup.SelectedIndex].Info.Id);
            };

            content.Relayout += (s, e) =>
            {
                var timeZone = timeZones.Where(x => x.Info.Id == SystemSettings.LocaleTimeZone).FirstOrDefault();

                if (timeZone != null)
                {
                    content.ScrollToIndex(timeZones.IndexOf(timeZone));
                }
            };

            return content;
        }
    }
}
