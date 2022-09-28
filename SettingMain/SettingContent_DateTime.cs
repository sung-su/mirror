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

            item = base.CreateItemWithCheck(Resources.IDS_ST_MBODY_AUTO_UPDATE, Resources.IDS_ST_HEADER_UNAVAILABLE, false, true);
            content.Add(item);

            item = CreateItemWithCheck(Resources.IDS_ST_BODY_SET_DATE, Resources.IDS_ST_HEADER_UNAVAILABLE);
            content.Add(item);
            item = CreateItemWithCheck(Resources.IDS_ST_BODY_SET_TIME, Resources.IDS_ST_HEADER_UNAVAILABLE);
            content.Add(item);

            item = CreateItemWithCheck(Resources.IDS_ST_BODY_TIME_ZONE, Resources.IDS_ST_HEADER_UNAVAILABLE);
            content.Add(item);

            item = CreateItemWithCheck(Resources.IDS_ST_MBODY_24_HOUR_CLOCK, Resources.IDS_ST_SBODY_SHOW_THE_TIME_IN_24_HOUR_FORMAT_INSTEAD_OF_12_HOUR_HAM_PM_FORMAT, false, true);
            content.Add(item);

            return content;
        }
    }
}
