using SettingMainGadget.TextResources;
using SettingCore;
using SettingCore.Views;
using SettingMainGadget;
using SettingMainGadget.DateTime;
using System.Linq;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.System;
using Tizen;

namespace Setting.Menu
{
    public class DateTimeGadget : SettingCore.MainMenuGadget
    {
        public override Color ProvideIconColor() => new Color(IsLightTheme ? "#205493" : "#2560A8");

        public override string ProvideIconPath() => GetResourcePath("datetime.svg");

        public override string ProvideTitle() => NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_DATE_AND_TIME));

        private Sections sections = new Sections();
        private View content;

        private TextListItem mDateItem = null;
        private TextListItem mTimeItem = null;
        private TextListItem mTimezoneItem = null;

        private SwitchListItem autoUpdateItem = null;
        private SwitchListItem timeFormatItem = null;

        private bool isAutomaticTimeUpdateSupported;
        private string VconfTimezone = "db/setting/timezone";

        protected override View OnCreate()
        {
            base.OnCreate();

            AttachToEvents();

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
            DetachFromEvents();

            base.OnDestroy();
        }

        private void CreateView()
        {
            sections.RemoveAllSectionsFromView(content);

            try
            {
                // RPI4 device unable to get isAutomaticTimeUpdate system setting
                var autoUpdate = SystemSettings.AutomaticTimeUpdate;
                isAutomaticTimeUpdateSupported = true;
            }
            catch (System.Exception e)
            {
                Logger.Warn($"AutomaticTimeUpdate is not supported: {e.Message}");
            }

            isAutomaticTimeUpdateSupported = true;
            autoUpdateItem = new SwitchListItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_MBODY_AUTO_UPDATE)), isSelected: isAutomaticTimeUpdateSupported ? DateTimeManager.AutoTimeUpdate : false);
            autoUpdateItem.IsEnabled = isAutomaticTimeUpdateSupported;
            autoUpdateItem.Switch.SelectedChanged += (o, e) =>
            {
                DateTimeManager.AutoTimeUpdate = e.IsSelected;
                ApplyAutomaticTimeUpdate();
            };
            sections.Add(MainMenuProvider.DateTime_AutoUpdate, autoUpdateItem);

            mDateItem = TextListItem.CreatePrimaryTextItemWithSecondaryText(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_SET_DATE)), System.DateTime.Now.ToString("MMM d, yyyy"));
            if (mDateItem != null)
            {
                mDateItem.Clicked += (o, e) =>
                {
                    NavigateTo(MainMenuProvider.DateTime_SetDate);
                };
                sections.Add(MainMenuProvider.DateTime_SetDate, mDateItem);
            }

            mTimeItem = TextListItem.CreatePrimaryTextItemWithSecondaryText(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_SET_TIME)), DateTimeManager.FormattedTime);
            if (mTimeItem != null)
            {
                mTimeItem.Clicked += (o, e) =>
                {
                    NavigateTo(MainMenuProvider.DateTime_SetTime);
                };
                sections.Add(MainMenuProvider.DateTime_SetTime, mTimeItem);
            }

            mTimezoneItem = TextListItem.CreatePrimaryTextItemWithSecondaryText(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_TIME_ZONE)), DateTimeTimezoneManager.GetTimezoneName().timezoneName);
            if (mTimezoneItem != null)
            {
                mTimezoneItem.Clicked += (o, e) =>
                {
                    NavigateTo(MainMenuProvider.DateTime_SetTimezone);
                };
                sections.Add(MainMenuProvider.DateTime_SetTimezone, mTimezoneItem);
            }

            timeFormatItem = new SwitchListItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_MBODY_24_HOUR_CLOCK)), NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_SBODY_SHOW_THE_TIME_IN_24_HOUR_FORMAT_INSTEAD_OF_12_HOUR_HAM_PM_FORMAT)), DateTimeManager.Is24HourFormat);
            timeFormatItem.Switch.SelectedChanged += (o, e) =>
            {
                DateTimeManager.Is24HourFormat = e.IsSelected;
            };
            sections.Add(MainMenuProvider.DateTime_TimeFormat, timeFormatItem);

            ApplyAutomaticTimeUpdate();

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
        }

        private void ApplyAutomaticTimeUpdate()
        {
            if (mDateItem != null) mDateItem.IsEnabled = !DateTimeManager.AutoTimeUpdate;
            if (mTimeItem != null) mTimeItem.IsEnabled = !DateTimeManager.AutoTimeUpdate;
            if (mTimezoneItem != null) mTimezoneItem.IsEnabled = !DateTimeManager.AutoTimeUpdate;
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
            (string offset, string timezoneName) = DateTimeTimezoneManager.GetTimezoneName();

            Vconf.SetString(VconfTimezone, offset);

            if (mTimezoneItem != null)
            {
                mTimezoneItem.Secondary = timezoneName;
            }
        }

        private void SystemSettings_LocaleTimeFormat24HourSettingChanged(object sender, LocaleTimeFormat24HourSettingChangedEventArgs e)
        {
            if (mTimeItem != null)
                mTimeItem.Secondary = DateTimeManager.FormattedTime;
        }

        private void AttachToEvents()
        {
            try
            {
                SystemSettings.TimeChanged += SystemSettings_TimeChanged;
            }
            catch (System.Exception e)
            {
                Logger.Warn($"Cannot attach to SystemSettings.TimeChanged ({e.GetType()})");
            }

            try
            {
                SystemSettings.LocaleTimeZoneChanged += SystemSettings_LocaleTimeZoneChanged;
            }
            catch (System.Exception e)
            {
                Logger.Warn($"Cannot attach to SystemSettings.LocaleTimeZoneChanged ({e.GetType()})");
            }

            try
            {
                SystemSettings.LocaleTimeFormat24HourSettingChanged += SystemSettings_LocaleTimeFormat24HourSettingChanged;
            }
            catch (System.Exception e)
            {
                Logger.Warn($"Cannot attach to SystemSettings.LocaleTimeFormat24HourSettingChanged ({e.GetType()})");
            }
        }

        private void DetachFromEvents()
        {
            try
            {
                SystemSettings.TimeChanged -= SystemSettings_TimeChanged;
            }
            catch (System.Exception e)
            {
                Logger.Warn($"Cannot detach from SystemSettings.TimeChanged ({e.GetType()})");
            }

            try
            {
                SystemSettings.LocaleTimeZoneChanged -= SystemSettings_LocaleTimeZoneChanged;
            }
            catch (System.Exception e)
            {
                Logger.Warn($"Cannot detach from SystemSettings.LocaleTimeZoneChanged ({e.GetType()})");
            }

            try
            {
                SystemSettings.LocaleTimeFormat24HourSettingChanged -= SystemSettings_LocaleTimeFormat24HourSettingChanged;
            }
            catch (System.Exception e)
            {
                Logger.Warn($"Cannot detach from SystemSettings.LocaleTimeFormat24HourSettingChanged ({e.GetType()})");
            }
        }
    }
}
