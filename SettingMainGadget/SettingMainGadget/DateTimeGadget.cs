using SettingMainGadget.TextResources;
using SettingCore;
using SettingCore.Views;
using SettingMainGadget;
using SettingMainGadget.DateTime;
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

        private ScrollableBase content;

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

            CreateContent();

            return content;
        }

        protected override void OnDestroy()
        {
            DetachFromEvents();

            base.OnDestroy();
        }

        private void CreateContent()
        {
            content.RemoveAllChildren(true);
            sections.Clear();

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

            // section: auto update switch item 
            sections.Add(MainMenuProvider.DateTime_AutoUpdate, () =>
            {
                autoUpdateItem = new SwitchListItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_MBODY_AUTO_UPDATE)), isSelected: isAutomaticTimeUpdateSupported ? DateTimeManager.AutoTimeUpdate : false);
                autoUpdateItem.IsEnabled = isAutomaticTimeUpdateSupported;
                autoUpdateItem.Switch.SelectedChanged += (o, e) =>
                {
                    DateTimeManager.AutoTimeUpdate = e.IsSelected;
                    ApplyAutomaticTimeUpdate();
                };
                content.Add(autoUpdateItem);
            });

            // section: set date
            sections.Add(MainMenuProvider.DateTime_SetDate, () =>
            {
                mDateItem = TextListItem.CreatePrimaryTextItemWithSecondaryText(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_SET_DATE)), System.DateTime.Now.ToString("MMM d, yyyy"));
                if (mDateItem != null)
                {
                    mDateItem.Clicked += (o, e) =>
                    {
                        NavigateTo(MainMenuProvider.DateTime_SetDate);
                    };
                    content.Add(mDateItem);
                }
            });

            // section: set time
            sections.Add(MainMenuProvider.DateTime_SetTime, () =>
            {
                mTimeItem = TextListItem.CreatePrimaryTextItemWithSecondaryText(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_SET_TIME)), DateTimeManager.FormattedTime);
                if (mTimeItem != null)
                {
                    mTimeItem.Clicked += (o, e) =>
                    {
                        NavigateTo(MainMenuProvider.DateTime_SetTime);
                    };
                    content.Add(mTimeItem);
                }
            });

            // section: time zone
            sections.Add(MainMenuProvider.DateTime_SetTimezone, () =>
            {
                mTimezoneItem = TextListItem.CreatePrimaryTextItemWithSecondaryText(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_TIME_ZONE)), DateTimeTimezoneManager.GetTimezoneName().timezoneName);
                if (mTimezoneItem != null)
                {
                    mTimezoneItem.Clicked += (o, e) =>
                    {
                        NavigateTo(MainMenuProvider.DateTime_SetTimezone);
                    };
                    content.Add(mTimezoneItem);
                }
                ApplyAutomaticTimeUpdate();
            });

            // section: time format 
            sections.Add(MainMenuProvider.DateTime_TimeFormat, () =>
            {
                timeFormatItem = new SwitchListItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_MBODY_24_HOUR_CLOCK)), NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_SBODY_SHOW_THE_TIME_IN_24_HOUR_FORMAT_INSTEAD_OF_12_HOUR_HAM_PM_FORMAT)), DateTimeManager.Is24HourFormat);
                timeFormatItem.Switch.SelectedChanged += (o, e) =>
                {
                    DateTimeManager.Is24HourFormat = e.IsSelected;
                };
                content.Add(timeFormatItem);
            });

            CreateItems();
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
