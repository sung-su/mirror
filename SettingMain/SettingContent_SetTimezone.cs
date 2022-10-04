using System;
using System.Collections.Generic; // for Dictionary
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.Applications;
using System.Collections.ObjectModel;
using Tizen.System;

using SettingAppTextResopurces.TextResources;


namespace SettingMain
{
    class SettingContent_SetTimezone : SettingContent_Base
    {
        private string[] PickerItems;
        public SettingContent_SetTimezone()
            : base()
        {
            mTitle = Resources.IDS_ST_BUTTON_BACK;

            PickerItems = new string[10];
            PickerItems[0] = "Time zone 1";
            PickerItems[1] = "Time zone 2";
            PickerItems[2] = "Time zone 3";
            PickerItems[3] = "Time zone 4";
            PickerItems[4] = "Time zone 5";
            PickerItems[5] = "Time zone 6";
            PickerItems[6] = "Time zone 7";
            PickerItems[7] = "Time zone 8";
            PickerItems[8] = "Time zone 9";
            PickerItems[9] = "Time zone 10";
        }

        protected override View CreateContent(Window window)
        {
            var picker = new Picker()
            {
                // WidthSpecification = LayoutParamPolicies.MatchParent,
                // HeightSpecification = LayoutParamPolicies.MatchParent,
                // Size = new Size(100, 200),
            };

            ReadOnlyCollection<string> rc = new ReadOnlyCollection<string>(PickerItems);
            picker.DisplayedValues = rc;
            picker.MinValue = 0;
            picker.MaxValue = PickerItems.Length - 1;
            Tizen.Log.Debug("NUI", "DisplayedValues : " + picker.DisplayedValues);

            var button = new Button()
            {
                // WidthSpecification = LayoutParamPolicies.MatchParent,
                // HeightSpecification = LayoutParamPolicies.MatchParent,
                Text = Resources.IDS_ST_BUTTON_OK
            };
            button.Clicked += (bo, be) =>
            {
                
                Tizen.Log.Debug("NUI", String.Format("current : {0}", PickerItems[picker.CurrentValue]));


#if false
                // Update Widget Content by sending message to pop the fourth page.
                Bundle nextBundle2 = new Bundle();
                nextBundle2.AddItem("WIDGET_ACTION", "POP");
                String encodedBundle2 = nextBundle2.Encode();
                SetContentInfo(encodedBundle2);
#endif
            };


            var content = new View()
            {

                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
                Layout = new LinearLayout()
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                },
            };
            content.Add(new TextLabel(Resources.IDS_ST_BODY_TIME_ZONE));
            content.Add(picker);
            content.Add(button);

            return content;
        }
    }
}
