using System;
using System.Collections.Generic; // for Dictionary
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.Applications;

using Tizen.System;

using SettingAppTextResopurces.TextResources;

namespace SettingMain
{
    class SettingContent_DateTime : SettingContent_Base
    {
        DefaultLinearItem mDateItem = null;
        DefaultLinearItem mTimeItem = null;
        public SettingContent_DateTime()
            : base()
        {
            mTitle = Resources.IDS_ST_BODY_DATE_AND_TIME;
        }

        protected override View CreateContent(Window window)
        {
            // Content of the page which scrolls items vertically.
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

            DefaultLinearItem item = null;

            bool bAutoupdate = Vconf.GetBool("db/setting/automatic_time_update");

            item = base.CreateItemWithCheck(Resources.IDS_ST_MBODY_AUTO_UPDATE, null, false, true);
            if (item)
            {
                var toggle = item.Extra as Switch;
                toggle.IsSelected = bAutoupdate;


                toggle.SelectedChanged += (o, e) =>
                {
                    if (e.IsSelected)
                    {
                        Vconf.SetBool("db/setting/automatic_time_update", true);
                        Tizen.Log.Debug("NUI", "Auto Update is ON!\n");
                    }
                    else
                    {
                        Vconf.SetBool("db/setting/automatic_time_update", false);
                        Tizen.Log.Debug("NUI", "Auto Update is OFF!\n");
                    }
                };

                content.Add(item);
            }


            item = CreateItemWithCheck(Resources.IDS_ST_BODY_SET_DATE, DateTime.Now.ToShortDateString());
            mDateItem = item;
            if (item)
            {
                item.Clicked += (o, e) =>
                {
                    // Update Widget Content by sending message to add the third page in advance.
                    Bundle nextBundle = new Bundle();
                    nextBundle.AddItem("WIDGET_ID", "setdate@org.tizen.cssettings");
                    nextBundle.AddItem("WIDGET_WIDTH", window.Size.Width.ToString());
                    nextBundle.AddItem("WIDGET_HEIGHT", window.Size.Height.ToString());
                    nextBundle.AddItem("WIDGET_PAGE", "CONTENT_PAGE");
                    nextBundle.AddItem("WIDGET_ACTION", "PUSH");
                    String encodedBundle = nextBundle.Encode();
                    SetContentInfo(encodedBundle);

                };
                content.Add(item);
            }






            item = CreateItemWithCheck(Resources.IDS_ST_BODY_SET_TIME, DateTime.Now.ToShortTimeString());
            mTimeItem = item;
            if (item)
            {
                item.Clicked += (o, e) =>
                {
                    // Update Widget Content by sending message to add the third page in advance.
                    Bundle nextBundle = new Bundle();
                    nextBundle.AddItem("WIDGET_ID", "settime@org.tizen.cssettings");
                    nextBundle.AddItem("WIDGET_WIDTH", window.Size.Width.ToString());
                    nextBundle.AddItem("WIDGET_HEIGHT", window.Size.Height.ToString());
                    nextBundle.AddItem("WIDGET_PAGE", "CONTENT_PAGE");
                    nextBundle.AddItem("WIDGET_ACTION", "PUSH");
                    String encodedBundle = nextBundle.Encode();
                    SetContentInfo(encodedBundle);

                };
                content.Add(item);
            }

            string strTimezone = SystemSettings.LocaleTimeZone;
            item = CreateItemWithCheck(Resources.IDS_ST_BODY_TIME_ZONE, strTimezone);
            if (item)
            {
#if false
                item.Clicked += (o, e) =>
                {
                    // Update Widget Content by sending message to add the third page in advance.
                    Bundle nextBundle = new Bundle();
                    nextBundle.AddItem("WIDGET_ID", "settimezone@org.tizen.cssettings");
                    nextBundle.AddItem("WIDGET_WIDTH", window.Size.Width.ToString());
                    nextBundle.AddItem("WIDGET_HEIGHT", window.Size.Height.ToString());
                    nextBundle.AddItem("WIDGET_PAGE", "CONTENT_PAGE");
                    nextBundle.AddItem("WIDGET_ACTION", "PUSH");
                    String encodedBundle = nextBundle.Encode();
                    SetContentInfo(encodedBundle);
                };
#endif
                content.Add(item);
            }

            bool bTime24 = (Vconf.GetInt("db/menu_widget/regionformat_time1224") == 2)? true : false;
            item = CreateItemWithCheck(Resources.IDS_ST_MBODY_24_HOUR_CLOCK, Resources.IDS_ST_SBODY_SHOW_THE_TIME_IN_24_HOUR_FORMAT_INSTEAD_OF_12_HOUR_HAM_PM_FORMAT, false, true);
            if (item)
            {
                var toggle = item.Extra as Switch;
                toggle.IsSelected = bTime24;


                toggle.SelectedChanged += (o, e) =>
                {
                    if (e.IsSelected)
                    {
                        Vconf.SetInt("db/menu_widget/regionformat_time1224", 2);
                        Tizen.Log.Debug("NUI", "Auto Update is ON!\n");
                    }
                    else
                    {
                        Vconf.SetInt("db/menu_widget/regionformat_time1224", 1);
                        Tizen.Log.Debug("NUI", "Auto Update is OFF!\n");
                    }
                };

                content.Add(item);
            }


            return content;
        }


        private void SystemSettings_TimeChanged(object sender, Tizen.System.TimeChangedEventArgs e)
        {
            Tizen.Log.Debug("NUI", "SystemSettings_TimeChanged is called\n");

            mDateItem.SubText = DateTime.Now.ToShortDateString();
            mTimeItem.SubText = DateTime.Now.ToShortTimeString();

        }

        protected override void OnCreate(string contentInfo, Window window)
        {
            base.OnCreate(contentInfo, window);
            Tizen.System.SystemSettings.TimeChanged += SystemSettings_TimeChanged;
        }
        protected override void OnTerminate(string contentInfo, TerminationType type)
        {
            Tizen.System.SystemSettings.TimeChanged -= SystemSettings_TimeChanged;

            base.OnTerminate(contentInfo, type);
        }

    }
}
