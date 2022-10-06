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
    class SettingContent_Applications : SettingContent_Base
    {
        public SettingContent_Applications()
            : base()
        {
            mTitle = Resources.IDS_ST_BODY_APPLICATIONS;
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

            DefaultLinearItem item;
            item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_APPLICATION_MANAGER);
            content.Add(item);

            item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_HEADER_DEFAULT_APPLICATIONS_ABB);
            content.Add(item);
 
            return content;
        }

    }
}
