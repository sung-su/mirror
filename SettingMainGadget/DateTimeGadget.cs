using SettingAppTextResopurces.TextResources;
using SettingCore;
using SettingCore.Customization;
using SettingCore.Views;
using SettingMainGadget;
using SettingMainGadget.DateTime;
using System.Linq;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.System;

namespace Setting.Menu
{
    public class DateTimeGadget : SettingCore.MainMenuGadget
    {
        public override Color ProvideIconColor() => new Color("#205493");

        public override string ProvideIconPath() => GetResourcePath("datetime.svg");

        public override string ProvideTitle() => Resources.IDS_ST_BODY_DATE_AND_TIME;

        private Sections sections = new Sections();
        private View content;

        private TextListItem mDateItem = null;
        private TextListItem mTimeItem = null;
        private TextListItem mTimezoneItem = null;

        private SwitchListItem autoUpdateItem = null;
        private SwitchListItem timeFormatItem = null;

        protected override View OnCreate()
        {
            base.OnCreate();

            SystemSettings.TimeChanged += SystemSettings_TimeChanged;
            SystemSettings.LocaleTimeZoneChanged += SystemSettings_LocaleTimeZoneChanged;
            SystemSettings.LocaleTimeFormat24HourSettingChanged += SystemSettings_LocaleTimeFormat24HourSettingChanged;

            content = new ScrollableBase()
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

            CreateView();

            return content;
        }

        protected override void OnDestroy()
        {
            SystemSettings.TimeChanged -= SystemSettings_TimeChanged;
            SystemSettings.LocaleTimeZoneChanged -= SystemSettings_LocaleTimeZoneChanged;
            SystemSettings.LocaleTimeFormat24HourSettingChanged -= SystemSettings_LocaleTimeFormat24HourSettingChanged;

            base.OnDestroy();
        }

        private void CreateView()
        {
            sections.RemoveAllSectionsFromView(content);

            if(autoUpdateItem != null)
            {
                content.Remove(autoUpdateItem);
            }

            autoUpdateItem = new SwitchListItem(Resources.IDS_ST_MBODY_AUTO_UPDATE, isSelected: DateTimeManager.AutoTimeUpdate);
            autoUpdateItem.Switch.SelectedChanged += (o, e) =>
            {
                DateTimeManager.AutoTimeUpdate = e.IsSelected;
                ApplyAutomaticTimeUpdate();
            };
            content.Add(autoUpdateItem);

            mDateItem = TextListItem.CreatePrimaryTextItemWithSecondaryText(Resources.IDS_ST_BODY_SET_DATE, System.DateTime.Now.ToString("MMM d, yyyy"));
            if (mDateItem != null)
            {
                mDateItem.Clicked += (o, e) =>
                {
                    NavigateTo(MainMenuProvider.DateTime_SetDate);
                };
                sections.Add(MainMenuProvider.DateTime_SetDate, mDateItem);
            }

            mTimeItem = TextListItem.CreatePrimaryTextItemWithSecondaryText(Resources.IDS_ST_BODY_SET_TIME, DateTimeManager.FormattedTime);
            if (mTimeItem != null)
            {
                mTimeItem.Clicked += (o, e) =>
                {
                    NavigateTo(MainMenuProvider.DateTime_SetTime);
                };
                sections.Add(MainMenuProvider.DateTime_SetTime, mTimeItem);
            }

            mTimezoneItem = TextListItem.CreatePrimaryTextItemWithSecondaryText(Resources.IDS_ST_BODY_TIME_ZONE, DateTimeTimezoneManager.GetTimezoneName());
            if (mTimezoneItem != null)
            {
                mTimezoneItem.Clicked += (o, e) =>
                {
                    NavigateTo(MainMenuProvider.DateTime_SetTimezone);
                };
                sections.Add(MainMenuProvider.DateTime_SetTimezone, mTimezoneItem);
            }

            var customization = GetCustomization().OrderBy(c => c.Order);
            Logger.Debug($"customization: {customization.Count()}");
            foreach (var cust in customization)
            {
                string visibility = cust.IsVisible ? "visible" : "hidden";
                Logger.Verbose($"Customization: {cust.MenuPath} - {visibility} - {cust.Order}");
                if (cust.IsVisible && sections.TryGetValue(cust.MenuPath, out View row))
                {
                    content.Add(row);
                }
            }

            if (timeFormatItem != null)
            {
                content.Remove(timeFormatItem);
            }

            timeFormatItem = new SwitchListItem(Resources.IDS_ST_MBODY_24_HOUR_CLOCK, Resources.IDS_ST_SBODY_SHOW_THE_TIME_IN_24_HOUR_FORMAT_INSTEAD_OF_12_HOUR_HAM_PM_FORMAT, DateTimeManager.Is24HourFormat);
            timeFormatItem.Switch.SelectedChanged += (o, e) =>
            {
                DateTimeManager.Is24HourFormat = e.IsSelected;
            };
            content.Add(timeFormatItem);

            ApplyAutomaticTimeUpdate();
        }

        private void ApplyAutomaticTimeUpdate()
        {
            if (mDateItem != null) mDateItem.IsEnabled = !SystemSettings.AutomaticTimeUpdate;
            if (mTimeItem != null) mTimeItem.IsEnabled = !SystemSettings.AutomaticTimeUpdate;
            if (mTimezoneItem != null) mTimezoneItem.IsEnabled = !SystemSettings.AutomaticTimeUpdate;
        }

        private void SystemSettings_TimeChanged(object sender, Tizen.System.TimeChangedEventArgs e)
        {
            if (mDateItem != null)
                mDateItem.Secondary = System.DateTime.Now.ToString("MMM d, yyyy");
            if (mTimeItem != null)
                mTimeItem.Secondary = DateTimeManager.FormattedTime;
        }

        private void SystemSettings_LocaleTimeZoneChanged(object sender, LocaleTimeZoneChangedEventArgs e)
        {
            if (mTimezoneItem != null)
                mTimezoneItem.Secondary = DateTimeTimezoneManager.GetTimezoneName();
        }

        private void SystemSettings_LocaleTimeFormat24HourSettingChanged(object sender, LocaleTimeFormat24HourSettingChangedEventArgs e)
        {
            if (mTimeItem != null)
                mTimeItem.Secondary = DateTimeManager.FormattedTime;
        }
    }
}
