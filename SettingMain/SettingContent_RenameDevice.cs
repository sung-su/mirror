using System;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.Applications;

using SettingAppTextResopurces.TextResources;

namespace SettingMain
{
    class SettingContent_RenameDevice : SettingContent_Base
    {
        private const int MAX_DEVICE_NAME_LEN = 32;

        TextField mTextField;
        public SettingContent_RenameDevice()
            : base()
        {
            mTitle = Resources.IDS_ST_BUTTON_BACK;
            mTextField = null;
        }

        protected override View CreateContent(Window window)
        {
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

            var textTitle = SettingItemCreator.CreateItemTitle(Resources.IDS_ST_HEADER_RENAME_DEVICE);
            content.Add(textTitle);

            var textSubTitle = new TextLabel(Resources.IDS_ST_BODY_DEVICE_NAMES_ARE_DISPLAYED)
            {
                MultiLine = true,
                LineWrapMode = LineWrapMode.Character,
                Size2D = new Size2D(window.WindowSize.Width-20*2, 100),
            };
            content.Add(textSubTitle);

            String name = Vconf.GetString("db/setting/device_name");

            mTextField = new TextField
            {
                BackgroundColor = Color.White
            };
            /*
             * PropertyMap placeholder = new PropertyMap();
                        placeholder.Add("color", new PropertyValue(Color.CadetBlue));
                        placeholder.Add("fontFamily", new PropertyValue("Serif"));
                        placeholder.Add("pointSize", new PropertyValue(25.0f));
                        mTextField.Placeholder = placeholder;
                        mTextField.MaxLength = MAX_DEVICE_NAME_LEN;
                        mTextField.EnableCursorBlink = true;

                        mTextField.Text = name;
            */
            mTextField.Text = "0123456789123456789";
            content.Add(mTextField);


            var button = new Button()
            {
                // WidthSpecification = LayoutParamPolicies.MatchParent,
                // HeightSpecification = LayoutParamPolicies.MatchParent,
                Text = Resources.IDS_ST_BUTTON_OK
            };
            button.Clicked += (o, e) =>
            {
                // Change Device Name
                Vconf.SetString("db/setting/device_name", mTextField.Text);


                RequestWidgetPop();
            };
            content.Add(button);

            return content;
        }
    }
}
