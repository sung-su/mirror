using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Collections.Generic; // for Dictionary
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.Applications;

namespace SettingMain
{
    public class Program : NUIWidgetApplication
    {

        public static readonly string ItemContentNameIcon = "ItemIcon";
        public static readonly string ItemContentNameTitle = "ItemTitle";
        public static readonly string ItemContentNameDescription = "ItemDescription";

        public Program(Dictionary<System.Type, string> widgetSet) : base(widgetSet)
        {

        }

        protected override void OnCreate()
        {
            base.OnCreate();
        }

        public void OnKeyEvent(object sender, Window.KeyEventArgs e)
        {
            if (e.Key.State == Key.StateType.Down && (e.Key.KeyPressedName == "XF86Back" || e.Key.KeyPressedName == "Escape"))
            {
                Exit();
            }
        }

        static void Main(string[] args)
        {
            Dictionary<System.Type, string> widgetSet = new Dictionary<Type, string>();
            widgetSet.Add(typeof(SettingContent_Sound), "sound@org.tizen.cssettings");
            widgetSet.Add(typeof(SettingContent_Display), "display@org.tizen.cssettings");
            widgetSet.Add(typeof(SettingContent_Applications), "apps@org.tizen.cssettings");
            widgetSet.Add(typeof(SettingContent_Storage), "storage@org.tizen.cssettings");
            widgetSet.Add(typeof(SettingContent_LanguageInput), "languageinput@org.tizen.cssettings");
            widgetSet.Add(typeof(SettingContent_DisplayLanguage), "displaylanguage@org.tizen.cssettings");
            widgetSet.Add(typeof(SettingContent_DateTime), "datetime@org.tizen.cssettings");
            widgetSet.Add(typeof(SettingContent_AboutDevice), "aboutdevice@org.tizen.cssettings");
            
            widgetSet.Add(typeof(SettingContent_LegalInfo), "content_legalinfo@org.tizen.cssettings");
            widgetSet.Add(typeof(SettingDialog_Rename), "dlg_rename@org.tizen.cssettings");
            widgetSet.Add(typeof(SettingContent_DeviceStatus), "content_devicestatus@org.tizen.cssettings");


            var app = new Program(widgetSet);
            app.Run(args);
        }
    }
}
