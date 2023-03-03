using SettingAppTextResopurces.TextResources;
using SettingCore;
using SettingMain;
using SettingMainGadget.DateTime;
using System;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.System;

namespace Setting.Menu
{
    public class DateTimeGadget : SettingCore.MainMenuGadget
    {
        public override Color ProvideIconColor() => new Color("#205493");

        public override string ProvideIconPath() => "main-menu-icons/datetime.svg";

        public override string ProvideTitle() => Resources.IDS_ST_BODY_DATE_AND_TIME;

        private DefaultLinearItem mDateItem = null;
        private DefaultLinearItem mTimeItem = null;
        private DefaultLinearItem mTimezoneItem = null;

        protected override View OnCreate()
        {
            base.OnCreate();

            SystemSettings.LocaleTimeFormat24HourSettingChanged += SystemSettings_LocaleTimeFormat24HourSettingChanged;

            return CreateView();
        }

        protected override void OnDestroy()
        {
            SystemSettings.LocaleTimeFormat24HourSettingChanged -= SystemSettings_LocaleTimeFormat24HourSettingChanged;

            base.OnDestroy();
        }

        private View CreateView()
        {
            var content = new ScrollableBase()
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

            DefaultLinearItem item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_MBODY_AUTO_UPDATE, null, false, true);
            if (item != null)
            {
                var toggle = item.Extra as Switch;
                toggle.IsSelected = DateTimeManager.AutoTimeUpdate;

                toggle.SelectedChanged += (o, e) =>
                {
                    DateTimeManager.AutoTimeUpdate = e.IsSelected;
                    ApplyAutomaticTimeUpdate();
                };

                content.Add(item);
            }

            mDateItem = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_SET_DATE, System.DateTime.Now.ToString("MMM d, yyyy"));
            if (mDateItem != null)
            {
                mDateItem.Clicked += (o, e) =>
                {
                };
                content.Add(mDateItem);
            }

            mTimeItem = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_SET_TIME, DateTimeManager.FormattedTime);
            if (mTimeItem != null)
            {
                mTimeItem.Clicked += (o, e) =>
                {
                };
                content.Add(mTimeItem);
            }

            mTimezoneItem = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_TIME_ZONE, DateTimeTimezoneManager.GetTimezoneName());
            if (mTimezoneItem != null)
            {
                mTimezoneItem.Clicked += (o, e) =>
                {
                };
                content.Add(mTimezoneItem);
            }

            item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_MBODY_24_HOUR_CLOCK, Resources.IDS_ST_SBODY_SHOW_THE_TIME_IN_24_HOUR_FORMAT_INSTEAD_OF_12_HOUR_HAM_PM_FORMAT, false, true);
            if (item != null)
            {
                var toggle = item.Extra as Switch;
                toggle.IsSelected = DateTimeManager.Is24HourFormat;

                toggle.SelectedChanged += (o, e) =>
                {
                    DateTimeManager.Is24HourFormat = e.IsSelected;
                };

                content.Add(item);
            }

            ApplyAutomaticTimeUpdate();

            return content;
        }

        private void ApplyAutomaticTimeUpdate()
        {
            if (mDateItem != null) mDateItem.IsEnabled = !SystemSettings.AutomaticTimeUpdate;
            if (mTimeItem != null) mTimeItem.IsEnabled = !SystemSettings.AutomaticTimeUpdate;
            if (mTimezoneItem != null) mTimezoneItem.IsEnabled = !SystemSettings.AutomaticTimeUpdate;
        }

        private void SystemSettings_LocaleTimeFormat24HourSettingChanged(object sender, LocaleTimeFormat24HourSettingChangedEventArgs e)
        {
            if (mTimeItem != null)
                mTimeItem.SubText = DateTimeManager.FormattedTime;
        }
    }
}
