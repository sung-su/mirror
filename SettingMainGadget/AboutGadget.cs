using SettingAppTextResopurces.TextResources;
using SettingCore;
using SettingCore.Customization;
using SettingCore.Views;
using SettingMain;
using SettingMainGadget;
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
        private const int MAX_DEVICE_NAME_LEN = 32;
        private TextListItem renameDevice;
        private string VconfDeviceName = "db/setting/device_name";
        private string SystemModelName = "tizen.org/system/model_name";

        private Sections sections = new Sections();
        private View content;

        public override Color ProvideIconColor() => new Color("#301A4B");

        public override string ProvideIconPath() => GetResourcePath("about.svg");

        public override string ProvideTitle() => Resources.IDS_ST_BODY_ABOUT_DEVICE;

        protected override View OnCreate()
        {
            base.OnCreate();
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

            SystemSettings.DeviceNameChanged += SystemSettings_DeviceNameChanged;
            CreateView();

            return content;
        }

        protected override void OnDestroy()
        {
            SystemSettings.DeviceNameChanged -= SystemSettings_DeviceNameChanged;

            base.OnDestroy();
        }

        private void SystemSettings_DeviceNameChanged(object sender, DeviceNameChangedEventArgs e)
        {
            Logger.Verbose($"Device name changed: {e.Value}");
            renameDevice.Secondary = e.Value;
        }


        private void CreateView()
        {
            sections.RemoveAllSectionsFromView(content);

            var manageCertificates = TextListItem.CreatePrimaryTextItem(Resources.IDS_ST_HEADER_MANAGE_CERTIFICATES_ABB);
            manageCertificates.Clicked += (s, e) =>
            {
                NavigateTo(MainMenuProvider.About_ManageCertificates);
            };
            sections.Add(MainMenuProvider.About_ManageCertificates, manageCertificates);

            var openSourceLicenses = TextListItem.CreatePrimaryTextItem(Resources.IDS_ST_BODY_OPEN_SOURCE_LICENCES);
            openSourceLicenses.Clicked += (s, e) =>
            {
                NavigateTo(MainMenuProvider.About_OpenSourceLicenses);
            };
            sections.Add(MainMenuProvider.About_OpenSourceLicenses, openSourceLicenses);

            // TODO: replace with header text (gray)
            var deviceInfo = TextListItem.CreatePrimaryTextItem(Resources.IDS_ST_BODY_DEVICE_INFO);
            sections.Add(MainMenuProvider.About_DeviceInfo, deviceInfo);

            if (Vconf.TryGetString(VconfDeviceName, out string name))
            {
                Logger.Warn($"Could not get vconf value: {VconfDeviceName}");
            }

            renameDevice = TextListItem.CreatePrimaryTextItemWithSecondaryText(Resources.IDS_ST_BODY_NAME, name);
            renameDevice.Clicked += (s, e) =>
            {
                ShowRenamePopup(name);
            };
            sections.Add(MainMenuProvider.About_RenameDevice, renameDevice);

            bool result = Tizen.System.Information.TryGetValue<string>("http://tizen.org/system/model_name", out string modelNumberText);
            var modelNumber = TextListItem.CreatePrimaryTextItemWithSecondaryText(Resources.IDS_ST_BODY_MODEL_NUMBER, result ? modelNumberText : Resources.IDS_ST_HEADER_UNAVAILABLE);
            sections.Add(MainMenuProvider.About_ModelNumber, modelNumber);

            result = Tizen.System.Information.TryGetValue<string>("http://tizen.org/feature/platform.version", out string platformVersionText);
            var platformVersion = TextListItem.CreatePrimaryTextItemWithSecondaryText(Resources.IDS_ST_MBODY_TIZEN_VERSION, result ? platformVersionText : Resources.IDS_ST_HEADER_UNAVAILABLE);
            sections.Add(MainMenuProvider.About_TizenVersion, platformVersion);

            result = Tizen.System.Information.TryGetValue<string>("http://tizen.org/system/platform.processor", out string platformProcessorText);
            var cpu = TextListItem.CreatePrimaryTextItemWithSecondaryText("CPU", result ? platformProcessorText : Resources.IDS_ST_HEADER_UNAVAILABLE);
            sections.Add(MainMenuProvider.About_Cpu, cpu);

            var memusage = new Tizen.System.SystemMemoryUsage();
            float ram_total_gb = memusage.Total / (float)(1024 * 1024);
            var ram = TextListItem.CreatePrimaryTextItemWithSecondaryText("RAM", string.Format("{0:0.0} GB", ram_total_gb));
            sections.Add(MainMenuProvider.About_Ram, ram);

            bool result1 = Tizen.System.Information.TryGetValue<int>("http://tizen.org/feature/screen.width", out int screenwidth);
            bool result2 = Tizen.System.Information.TryGetValue<int>("http://tizen.org/feature/screen.height", out int screenheight);

            var resolution = TextListItem.CreatePrimaryTextItemWithSecondaryText(Resources.IDS_ST_BODY_RESOLUTION, result1 && result2 ? $"{screenwidth} x {screenheight}" : Resources.IDS_ST_HEADER_UNAVAILABLE);
            sections.Add(MainMenuProvider.About_Resolution, resolution);

            if (IsEmulBin() == false)
            {
                var showOther = TextListItem.CreatePrimaryTextItemWithSecondaryText(Resources.IDS_ST_BODY_STATUS, Resources.IDS_ST_BODY_SHOW_NETWORK_STATUS_AND_OTHER_INFORMATION);
                showOther.Clicked += (s, e) =>
                {
                    NavigateTo(MainMenuProvider.About_DeviceStatus);
                };
                sections.Add(MainMenuProvider.About_DeviceStatus, showOther);
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

        private void ShowRenamePopup(string name)
        {
            var content = new View()
            {
                BackgroundColor = new Color("#FAFAFA"),
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
                Layout = new LinearLayout()
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                },
            };

            var textTitle = SettingItemCreator.CreateItemTitle(Resources.IDS_ST_HEADER_RENAME_DEVICE);
            textTitle.Margin = new Extents(24, 0, 0, 0).SpToPx();
            content.Add(textTitle);

            var textSubTitle = new TextLabel(Resources.IDS_ST_BODY_DEVICE_NAMES_ARE_DISPLAYED)
            {
                MultiLine = true,
                LineWrapMode = LineWrapMode.Character,
                Size = new Size(Window.Instance.WindowSize.Width - 20 * 2, 100),
                Margin = new Extents(24, 24, 0, 0).SpToPx(),
            };
            content.Add(textSubTitle);

            PropertyMap placeholder = new PropertyMap();
            placeholder.Add("color", new PropertyValue(Color.CadetBlue));
            placeholder.Add("fontFamily", new PropertyValue("Serif"));
            placeholder.Add("pointSize", new PropertyValue(25.0f));

            var textField = new TextField
            {
                BackgroundColor = Color.White,

                Placeholder = placeholder,

                MaxLength = MAX_DEVICE_NAME_LEN,
                EnableCursorBlink = true,
                Text = name,
            };
            content.Add(textField);

            var button = new Button()
            {
                Text = Resources.IDS_ST_BUTTON_OK,
                Margin = new Extents(0, 0, 0, 32).SpToPx(),
            };
            button.Clicked += (o, e) =>
            {
                // Change Device Name
                Vconf.SetString("db/setting/device_name", textField.Text);
                NUIApplication.GetDefaultWindow().GetDefaultNavigator().Pop();
            };
            content.Add(button);

            RoundedDialogPage.ShowDialog(content);
        }
    }
}
