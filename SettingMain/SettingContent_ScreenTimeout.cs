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
    public class ScreenTimeoutInfo
    {
        private readonly string Name = null;
        private readonly int Value;


        public ScreenTimeoutInfo(string name, int value)
        {
            Name = name;
            Value = value;
        }


        public string GetName()
        {
            return Name;
        }

        public int GetValue()
        {
            return Value;
        }
    };


    class SettingContent_ScreenTimeout : SettingContent_Base
    {

        private static readonly ScreenTimeoutInfo[] TimeoutList =
        {
            new ScreenTimeoutInfo(Resources.IDS_ST_BODY_ALWAYS_ON, 0),
            new ScreenTimeoutInfo(Resources.IDS_ST_BODY_15SEC, 15),
            new ScreenTimeoutInfo(Resources.IDS_ST_BODY_30SEC, 30),
            new ScreenTimeoutInfo(Resources.IDS_ST_BODY_1_MINUTE, 60),
            new ScreenTimeoutInfo(Resources.IDS_ST_BODY_2_MINUTES, 120),
            new ScreenTimeoutInfo(Resources.IDS_ST_BODY_5_MINUTES, 300),
            new ScreenTimeoutInfo(Resources.IDS_ST_BODY_10_MINUTES, 600)
        };



        private string[] PickerItems;
        public SettingContent_ScreenTimeout()
            : base()
        {


            mTitle = Resources.IDS_ST_BODY_SCREEN_TIMEOUT_ABB2;

            PickerItems = new string[TimeoutList.Length];
            for (int i = 0; i < TimeoutList.Length; i++)
            {
                PickerItems[i] = TimeoutList[i].GetName();
            }
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
            picker.CurrentValue = GetScreenTimeoutIndex();
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

                SetScreenTimeout(picker.CurrentValue);

                // Update Widget Content by sending message to pop the fourth page.
                Bundle nextBundle2 = new Bundle();
                nextBundle2.AddItem("WIDGET_ACTION", "POP");
                String encodedBundle2 = nextBundle2.Encode();
                SetContentInfo(encodedBundle2);
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
            content.Add(new TextLabel(Resources.IDS_ST_BODY_SCREEN_TIMEOUT_ABB2));
            content.Add(picker);
            content.Add(button);

            return content;
        }

        void SetScreenTimeout(int index)
        {
            SystemSettings.ScreenBacklightTime = TimeoutList[index].GetValue();
        }

        public static int GetScreenTimeoutIndex()
        {
            //int value = SystemSettings.ScreenBacklightTime;
            int value = Vconf.GetInt("db/setting/lcd_backlight_normal");

            Tizen.Log.Debug("NUI", "ScreenTimeout : " + value.ToString());

            int index = 0;
            if (value < 15)
            {
                value = 0;
                index = 0;
            }
            else if (value >= 15 && value < 30)
            {
                value = 15;
                index = 1;
            }
            else if (value >= 30 && value < 60)
            {
                value = 30;
                index = 2;
            }
            else if (value >= 60 && value < 120)
            {
                value = 60;
                index = 3;
            }
            else if (value >= 120 && value < 300)
            {
                value = 120;
                index = 4;
            }
            else if (value >= 300 && value < 600)
            {
                value = 300;
                index = 5;
            }
            else
            {
                value = 600;
                index = 6;
            }

            //SystemSettings.ScreenBacklightTime = value;
            Vconf.SetInt("db/setting/lcd_backlight_normal", value);

            return index;
        }
        public static string GetScreenTimeoutName()
        {
            return TimeoutList[GetScreenTimeoutIndex()].GetName();

        }
    }
}

