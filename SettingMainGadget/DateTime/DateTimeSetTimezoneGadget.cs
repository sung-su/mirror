using SettingAppTextResopurces.TextResources;
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
        public override string ProvideTitle() => Resources.IDS_ST_BODY_TIME_ZONE;

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
                RadioButton radioButton = new RadioButton()
                {
                    ThemeChangeSensitive = true,
                    ItemHorizontalAlignment = HorizontalAlignment.Begin,
                    Text = timeZone.DisplayName,
                    IsSelected = SystemSettings.LocaleTimeZone == timeZone.Info.Id,
                    Margin = new Extents(24, 0, 0, 0).SpToPx(),
                };

                radioButtonGroup.Add(radioButton);
                content.Add(radioButton);
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
