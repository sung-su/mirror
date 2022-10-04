using System;
using System.ComponentModel;
using System.Diagnostics;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.Applications;
using Tizen.Telephony;
using Tizen.Network.Bluetooth;

using SettingAppTextResopurces.TextResources;

namespace SettingMain
{
    class SettingContent_AboutDevice : SettingContent_Base
    {
        public SettingContent_AboutDevice()
            : base()
        {
            mTitle = Resources.IDS_ST_BODY_ABOUT_DEVICE;
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

            item = CreateItemWithCheck(Resources.IDS_ST_HEADER_MANAGE_CERTIFICATES_ABB);
            content.Add(item);
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    // Update Widget Content by sending message to add the third page in advance.
                    Bundle nextBundle = new Bundle();
                    nextBundle.AddItem("WIDGET_ID", "certificates@org.tizen.setting-certificates");
                    nextBundle.AddItem("WIDGET_WIDTH", window.Size.Width.ToString());
                    nextBundle.AddItem("WIDGET_HEIGHT", window.Size.Height.ToString());
                    nextBundle.AddItem("WIDGET_PAGE", "CONTENT_PAGE");
                    nextBundle.AddItem("WIDGET_ACTION", "PUSH");
                    String encodedBundle = nextBundle.Encode();
                    SetContentInfo(encodedBundle);
                };
            }

            item = CreateItemWithCheck(Resources.IDS_ST_MBODY_LEGAL_INFORMATION_ABB);
            content.Add(item);
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    // Update Widget Content by sending message to push the third page.
                    Bundle nextBundle = new Bundle();
                    nextBundle.AddItem("WIDGET_ID", "legalinfo@org.tizen.cssettings");
                    nextBundle.AddItem("WIDGET_WIDTH", window.Size.Width.ToString());
                    nextBundle.AddItem("WIDGET_HEIGHT", window.Size.Height.ToString());
                    nextBundle.AddItem("WIDGET_PAGE", "CONTENT_PAGE");
                    nextBundle.AddItem("WIDGET_ACTION", "PUSH");
                    String encodedBundle = nextBundle.Encode();
                    SetContentInfo(encodedBundle);
                };
            }


            /////////////////////////////////////////////////////

            item = CreateItemWithCheck(Resources.IDS_ST_BODY_DEVICE_INFO);
            content.Add(item);

            String name = Vconf.GetString("db/setting/device_name");
            item = CreateItemWithCheck(Resources.IDS_ST_BODY_NAME, name);
            content.Add(item);
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    // Update Widget Content by sending message to push the third page.
                    Bundle nextBundle = new Bundle();
                    nextBundle.AddItem("WIDGET_ID", "dlg_rename@org.tizen.cssettings");
                    nextBundle.AddItem("WIDGET_WIDTH", window.Size.Width.ToString());
                    nextBundle.AddItem("WIDGET_HEIGHT", window.Size.Height.ToString());
                    nextBundle.AddItem("WIDGET_PAGE", "DIALOG_PAGE");
                    nextBundle.AddItem("WIDGET_ACTION", "PUSH");
                    String encodedBundle = nextBundle.Encode();
                    SetContentInfo(encodedBundle);
                };
            }

            string valuestring;
            bool result  = Tizen.System.Information.TryGetValue<string>("http://tizen.org/system/model_name", out valuestring);
            if(result)
                item = CreateItemWithCheck(Resources.IDS_ST_BODY_MODEL_NUMBER, valuestring);
            else
                item = CreateItemWithCheck(Resources.IDS_ST_BODY_MODEL_NUMBER, Resources.IDS_ST_HEADER_UNAVAILABLE);
            content.Add(item);

            result = Tizen.System.Information.TryGetValue<string>("http://tizen.org/feature/platform.version", out valuestring);
            if (result)
                item = CreateItemWithCheck(Resources.IDS_ST_MBODY_TIZEN_VERSION, valuestring);
            else
                item = CreateItemWithCheck(Resources.IDS_ST_MBODY_TIZEN_VERSION, Resources.IDS_ST_HEADER_UNAVAILABLE);
            content.Add(item);


            result = Tizen.System.Information.TryGetValue<string>("http://tizen.org/system/platform.processor", out valuestring);
            if (result)
                item = CreateItemWithCheck("CPU", valuestring);
            else
                item = CreateItemWithCheck("CPU", Resources.IDS_ST_HEADER_UNAVAILABLE);
            content.Add(item);

            var memusage = new Tizen.System.SystemMemoryUsage();
            float ram_total_gb = memusage.Total / 1000000.0F;
            item = CreateItemWithCheck("RAM", String.Format("{0:0.0} GB",ram_total_gb));
            content.Add(item);

            int screenwidth, screenheight;
            bool result1 = Tizen.System.Information.TryGetValue<int>("http://tizen.org/feature/screen.width", out screenwidth);
            bool result2 = Tizen.System.Information.TryGetValue<int>("http://tizen.org/feature/screen.height", out screenheight);

            if (result1 && result2)
                item = CreateItemWithCheck(Resources.IDS_ST_BODY_RESOLUTION, String.Format("{0:d} x {1:d}", screenwidth, screenheight));
            else
                item = CreateItemWithCheck(Resources.IDS_ST_BODY_RESOLUTION, Resources.IDS_ST_HEADER_UNAVAILABLE);
            content.Add(item);

            item = CreateItemWithCheck(Resources.IDS_ST_BODY_STATUS, Resources.IDS_ST_BODY_SHOW_NETWORK_STATUS_AND_OTHER_INFORMATION);
            if (item != null)
            {

                item.Clicked += (o, e) =>
                {
                    // Update Widget Content by sending message to add the third page in advance.
                    Bundle nextBundle = new Bundle();
                    nextBundle.AddItem("WIDGET_ID", "devicestatus@org.tizen.cssettings");
                    nextBundle.AddItem("WIDGET_WIDTH", window.Size.Width.ToString());
                    nextBundle.AddItem("WIDGET_HEIGHT", window.Size.Height.ToString());
                    nextBundle.AddItem("WIDGET_PAGE", "CONTENT_PAGE");
                    nextBundle.AddItem("WIDGET_ACTION", "PUSH");
                    String encodedBundle = nextBundle.Encode();
                    SetContentInfo(encodedBundle);
                };
                content.Add(item);
            }

            return content;
        }
    }



    class SettingDialog_Rename : Widget
    {
        protected override void OnCreate(string contentInfo, Window window)
        {
            Bundle bundle = Bundle.Decode(contentInfo);

            window.BackgroundColor = Color.Transparent;

            var button = new Button()
            {
                Text = "OK",
            };
            button.Clicked += (o, e) =>
            {
                // Update Widget Content by sending message to pop the fourth page.
                Bundle nextBundle2 = new Bundle();
                nextBundle2.AddItem("WIDGET_ACTION", "POP");
                String encodedBundle2 = nextBundle2.Encode();
                SetContentInfo(encodedBundle2);
            };

            var dialog = new AlertDialog()
            {
                Title = "Legal Infomation",
                Message = "Message",
                Actions = new View[] { button },
            };

            window.Add(dialog);
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        protected override void OnResume()
        {
            base.OnResume();
        }

        protected override void OnResize(Window window)
        {
            base.OnResize(window);
        }

        protected override void OnTerminate(string contentInfo, TerminationType type)
        {
            base.OnTerminate(contentInfo, type);
        }

        protected override void OnUpdate(string contentInfo, int force)
        {
            base.OnUpdate(contentInfo, force);
        }
    }
}
