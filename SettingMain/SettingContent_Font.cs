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
    class SettingContent_Font : SettingContent_Base
    {
        public SettingContent_Font()
            : base()
        {
            mTitle = Resources.IDS_ST_BODY_FONT;
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


            SystemSettingsFontSize fontSize = SystemSettings.FontSize;
            Tizen.Log.Debug("NUI", "font size : " + fontSize.ToString());
                
            string fontType = SystemSettings.FontType;
            Tizen.Log.Debug("NUI", "font type : " + fontType);


            DefaultLinearItem item = null;

            item = CreateItemStatic(Resources.IDS_ST_BODY_SIZE);
            content.Add(item);
            var slideritem = CreateSliderItem("BRIGHTNESS", null, 5);

            content.Add(slideritem);


            item = CreateItemStatic(Resources.IDS_ST_BODY_TYPE);
            content.Add(item);


            RadioButtonGroup radiogroup = new RadioButtonGroup();
            RadioButtonGroup.SetIsGroupHolder(content, true);

            RadioButton radioButton = null;
            
            radioButton = new RadioButton()
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                Text = fontType,
                IsSelected = true,
            };
            radiogroup.Add(radioButton);
            content.Add(radioButton);


            return content;
        }


    }
}
