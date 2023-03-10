using SettingAppTextResopurces.TextResources;
using SettingCore;
using SettingCore.Customization;
using SettingMain;
using System;
using System.Collections.Generic;
using System.Linq;
using Tizen;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.System;

namespace Setting.Menu
{
    public class AboutGadget : SettingCore.MainMenuGadget
    {
        private DefaultLinearItem mDevicenameItem;
        private string VconfDeviceName = "db/setting/device_name";
        private string SystemModelName = "tizen.org/system/model_name";

        private Dictionary<string, View> sections = new Dictionary<string, View>();
        private View content;

        public override Color ProvideIconColor() => new Color("#301A4B");

        public override string ProvideIconPath() => GetResourcePath("about.svg");

        public override string ProvideTitle() => Resources.IDS_ST_BODY_ABOUT_DEVICE;

        protected override View OnCreate()
        {
            content = new ScrollableBase()
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

            CreateView();

            return content;
        }

        private void CreateView()
        {
            foreach (var section in sections)
            {
                content.Remove(section.Value);
            }
            sections.Clear();

            DefaultLinearItem item = null;

            item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_HEADER_MANAGE_CERTIFICATES_ABB);
            sections.Add("Setting.Menu.About.ManageCertificates", item);

            item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_OPEN_SOURCE_LICENCES);
            sections.Add("Setting.Menu.About.OpenSourceLicenses", item);
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    NavigateTo("Setting.Menu.About.OpenSourceLicenses");
                };
            }
            sections.Add("Setting.Menu.About.DeviceInfo", SettingItemCreator.CreateItemStatic(Resources.IDS_ST_BODY_DEVICE_INFO));

            if (Vconf.TryGetString(VconfDeviceName, out string name))
            {
                Logger.Warn($"Could not get vconf value: {VconfDeviceName}");
            }
            item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_NAME, name);
            mDevicenameItem = item;
            if (item != null)
            {
                item.Clicked += (o, e) =>
                {
                    //TODO
                    NavigateTo("Setting.Menu.About.RenameDevice");
                };
            }
            sections.Add("Setting.Menu.About.RenameDevice", item);

            string valuestring;
            bool result = Tizen.System.Information.TryGetValue<string>("http://tizen.org/system/model_name", out valuestring);
            if (result)
                item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_MODEL_NUMBER, valuestring);
            else
                item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_MODEL_NUMBER, Resources.IDS_ST_HEADER_UNAVAILABLE);
            sections.Add("Setting.Menu.About.ModelNumber", item);

            result = Tizen.System.Information.TryGetValue<string>("http://tizen.org/feature/platform.version", out valuestring);
            if (result)
                item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_MBODY_TIZEN_VERSION, valuestring);
            else
                item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_MBODY_TIZEN_VERSION, Resources.IDS_ST_HEADER_UNAVAILABLE);
            sections.Add("Setting.Menu.About.TizenVersion", item);

            result = Tizen.System.Information.TryGetValue<string>("http://tizen.org/system/platform.processor", out valuestring);
            if (result)
                item = SettingItemCreator.CreateItemWithCheck("CPU", valuestring);
            else
                item = SettingItemCreator.CreateItemWithCheck("CPU", Resources.IDS_ST_HEADER_UNAVAILABLE);
            sections.Add("Setting.Menu.About.Cpu", item);

            var memusage = new Tizen.System.SystemMemoryUsage();
            float ram_total_gb = memusage.Total / 1000000.0F;
            item = SettingItemCreator.CreateItemWithCheck("RAM", String.Format("{0:0.0} GB", ram_total_gb));
            sections.Add("Setting.Menu.About.Ram", item);

            bool result1 = Tizen.System.Information.TryGetValue<int>("http://tizen.org/feature/screen.width", out int screenwidth);
            bool result2 = Tizen.System.Information.TryGetValue<int>("http://tizen.org/feature/screen.height", out int screenheight);

            if (result1 && result2)
                item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_RESOLUTION, String.Format("{0:d} x {1:d}", screenwidth, screenheight));
            else
                item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_RESOLUTION, Resources.IDS_ST_HEADER_UNAVAILABLE);
            sections.Add("Setting.Menu.About.Resolution", item);

            if (IsEmulBin() == false)
            {
                item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_STATUS, Resources.IDS_ST_BODY_SHOW_NETWORK_STATUS_AND_OTHER_INFORMATION);
                if (item != null)
                {

                    item.Clicked += (o, e) =>
                    {
                        NavigateTo("Setting.Menu.About.DeviceStatus");
                    };
                    sections.Add("Setting.Menu.About.DeviceStatus", item);
                }
            }

            // add only visible sections to content view in required order
            var customization = GetCustomization().OrderBy(c => c.Order);
            foreach (var cust in customization)
            {
                string visibility = cust.IsVisible ? "visible" : "hidden";
                Logger.Verbose($"Customization: {cust.MenuPath} - {visibility} - {cust.Order}");
                if (cust.IsVisible && sections.TryGetValue(cust.MenuPath, out View row))
                {
                    content.Add(row);
                }
            }
        }

        protected override void OnCustomizationUpdate(IEnumerable<MenuCustomizationItem> items)
        {
            Logger.Verbose($"{nameof(AboutGadget)} got customization with {items.Count()} items. Recreating view.");
            CreateView();
        }

        private bool IsEmulBin()
        {
            Information.TryGetValue<string>(SystemModelName, out string model);
            return (model.Equals("Emulator") || model.Equals("EMULATOR"));
        }
    }
}
