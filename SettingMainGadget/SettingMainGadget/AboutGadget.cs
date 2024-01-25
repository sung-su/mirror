using SettingMainGadget.TextResources;
using SettingCore;
using SettingCore.Views;
using SettingMainGadget;
using System.Collections.Generic;
using System.Linq;
using Tizen;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.System;
using System;

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
        private View popup;
        private TextField textField;
        private TextLabel warning;
        private Button renameButton;
        private string deviceName;
        private bool isLightTheme => ThemeManager.PlatformThemeId == "org.tizen.default-light-theme";

        public override Color ProvideIconColor() => new Color(IsLightTheme ? "#301A4B" : "#CAB4E5");

        public override string ProvideIconPath() => GetResourcePath("about.svg");

        public override string ProvideTitle() => NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_ABOUT_DEVICE));

        protected override View OnCreate()
        {
            base.OnCreate();
            content = new ScrollableBase()
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
                ScrollingDirection = ScrollableBase.Direction.Vertical,
                HideScrollbar = false,
                ThemeChangeSensitive = true,
                Layout = new LinearLayout()
                {
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                },
            };

            SystemSettings.DeviceNameChanged += SystemSettings_DeviceNameChanged;
            content.Relayout += Content_Relayout;
            CreateView();

            return content;
        }

        private void Content_Relayout(object sender, EventArgs e)
        {
            if(popup != null && popup.IsOnWindow)
            {
                NUIApplication.GetDefaultWindow().GetDefaultNavigator().Pop();
                ShowRenamePopup(deviceName);
            }
        }

        protected override void OnDestroy()
        {
            SystemSettings.DeviceNameChanged -= SystemSettings_DeviceNameChanged;
            content.Relayout -= Content_Relayout;

            base.OnDestroy();
        }

        private void SystemSettings_DeviceNameChanged(object sender, DeviceNameChangedEventArgs e)
        {
            Logger.Verbose($"Device name changed: {e.Value}");
            deviceName = e.Value;
            renameDevice.Secondary = e.Value;
        }


        private void CreateView()
        {
            sections.RemoveAllSectionsFromView(content);

            var manageCertificates = TextListItem.CreatePrimaryTextItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_HEADER_MANAGE_CERTIFICATES_ABB)));
            manageCertificates.Clicked += (s, e) =>
            {
                NavigateTo(MainMenuProvider.About_ManageCertificates);
            };
            sections.Add(MainMenuProvider.About_ManageCertificates, manageCertificates);

            var openSourceLicenses = TextListItem.CreatePrimaryTextItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_OPEN_SOURCE_LICENCES)));
            openSourceLicenses.Clicked += (s, e) =>
            {
                NavigateTo(MainMenuProvider.About_OpenSourceLicenses);
            };
            sections.Add(MainMenuProvider.About_OpenSourceLicenses, openSourceLicenses);

            var scalableUI = TextListItem.CreatePrimaryTextItem("Scalable UI for Developers");
            scalableUI.Clicked += (s, e) =>
            {
                NavigateTo(MainMenuProvider.About_ScalableUI);
            };
            sections.Add(MainMenuProvider.About_ScalableUI, scalableUI);

            var deviceInfo = new TextHeaderListItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_DEVICE_INFO)));
            sections.Add(MainMenuProvider.About_DeviceInfo, deviceInfo);

            if (Vconf.TryGetString(VconfDeviceName, out deviceName))
            {
                Logger.Warn($"Could not get vconf value: {VconfDeviceName}");
            }

            renameDevice = TextListItem.CreatePrimaryTextItemWithSecondaryText(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_NAME)), deviceName);
            renameDevice.Clicked += (s, e) =>
            {
                ShowRenamePopup(deviceName);
            };
            sections.Add(MainMenuProvider.About_RenameDevice, renameDevice);

            bool result = Tizen.System.Information.TryGetValue<string>("http://tizen.org/system/model_name", out string modelNumberText);
            var modelNumber = TextListItem.CreatePrimaryTextItemWithSecondaryText(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_MODEL_NUMBER)), result ? modelNumberText : NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_HEADER_UNAVAILABLE)));
            sections.Add(MainMenuProvider.About_ModelNumber, modelNumber);

            result = Tizen.System.Information.TryGetValue<string>("http://tizen.org/feature/platform.version", out string platformVersionText);
            var platformVersion = TextListItem.CreatePrimaryTextItemWithSecondaryText(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_MBODY_TIZEN_VERSION)), result ? platformVersionText : NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_HEADER_UNAVAILABLE)));
            platformVersion.MultiTap += (s, e) =>
            {
                GadgetManager.Instance.ChangeMenuPathOrder(MainMenuProvider.About_ScalableUI, 30);

                var toast = Notification.MakeToast("Scalable UI for Developers menu enabled", Notification.ToastCenter);
                toast.Post(1000);
            };
            sections.Add(MainMenuProvider.About_TizenVersion, platformVersion);

            result = Tizen.System.Information.TryGetValue<string>("http://tizen.org/system/platform.processor", out string platformProcessorText);
            var cpu = TextListItem.CreatePrimaryTextItemWithSecondaryText("CPU", result ? platformProcessorText : NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_HEADER_UNAVAILABLE)));
            sections.Add(MainMenuProvider.About_Cpu, cpu);

            var memusage = new Tizen.System.SystemMemoryUsage();
            float ram_total_gb = memusage.Total / (float)(1024 * 1024);
            var ram = TextListItem.CreatePrimaryTextItemWithSecondaryText("RAM", string.Format("{0:0.0} GB", ram_total_gb));
            sections.Add(MainMenuProvider.About_Ram, ram);

            bool result1 = Tizen.System.Information.TryGetValue<int>("http://tizen.org/feature/screen.width", out int screenwidth);
            bool result2 = Tizen.System.Information.TryGetValue<int>("http://tizen.org/feature/screen.height", out int screenheight);

            var resolution = TextListItem.CreatePrimaryTextItemWithSecondaryText(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_RESOLUTION)), result1 && result2 ? $"{screenwidth} x {screenheight}" : NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_HEADER_UNAVAILABLE)));
            sections.Add(MainMenuProvider.About_Resolution, resolution);

            if (IsEmulBin() == false)
            {
                var showOther = TextListItem.CreatePrimaryTextItemWithSubText(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_STATUS)), NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_SHOW_NETWORK_STATUS_AND_OTHER_INFORMATION)));
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
            popup = new View()
            {
                BackgroundColor = isLightTheme ? new Color("#FAFAFA") : new Color("#16131A"),
                WidthSpecification = LayoutParamPolicies.WrapContent,
                HeightSpecification = LayoutParamPolicies.WrapContent,
                Layout = new LinearLayout()
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                },
                Padding = new Extents(16, 16, 16, 16).SpToPx(),
            };
            popup.BoxShadow = isLightTheme ? new Shadow(8.0f, new Color(0.0f, 0.0f, 0.0f, 0.16f), new Vector2(0.0f, 2.0f)) : new Shadow(6.0f, new Color("#FFFFFF29"), new Vector2(0.0f, 1.0f));

            var content = new View()
            {
                BackgroundColor = Color.Transparent,
                WidthSpecification = LayoutParamPolicies.WrapContent,
                HeightSpecification = LayoutParamPolicies.WrapContent,
                Layout = new LinearLayout()
                {
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                },
            };

            var header = new View()
            {
                BackgroundColor = Color.Transparent,
                WidthSpecification = LayoutParamPolicies.WrapContent,
                HeightSpecification = LayoutParamPolicies.WrapContent,
                Layout = new LinearLayout()
                {
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                },
            };

            var textView = new View()
            {
                BackgroundColor = Color.Transparent,
                WidthSpecification = LayoutParamPolicies.WrapContent,
                HeightSpecification = LayoutParamPolicies.WrapContent,
                Layout = new LinearLayout()
                {
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                    HorizontalAlignment = HorizontalAlignment.Center,
                },
                Padding = new Extents(8, 8, 8, 8).SpToPx(),
            };

            //title text
            var textTitle = new TextLabel(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_HEADER_RENAME_DEVICE)))
            {
                PixelSize = 24.SpToPx(),
                WidthResizePolicy = ResizePolicyType.FillToParent,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Extents(0, 0, 0, 8).SpToPx(),
            };

            // main text
            var textSubTitle = new TextLabel(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_DEVICE_NAMES_ARE_DISPLAYED)))
            {
                PixelSize = 24.SpToPx(),
                MultiLine = true,
                LineWrapMode = LineWrapMode.Word,
                SizeWidth = 634.SpToPx(),
                Margin = new Extents(0, 0, 8, 0).SpToPx(),
            };

            textView.Add(textTitle);
            textView.Add(textSubTitle);

            var mainView = new View()
            {
                BackgroundColor = Color.Transparent,
                WidthSpecification = LayoutParamPolicies.WrapContent,
                HeightSpecification = LayoutParamPolicies.WrapContent,
                Layout = new LinearLayout()
                {
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                },
                Padding = new Extents(8, 8, 8, 8).SpToPx(),
            };

            var entryView = new View()
            {
                BackgroundColor = Color.Transparent,
                WidthSpecification = LayoutParamPolicies.WrapContent,
                HeightSpecification = LayoutParamPolicies.WrapContent,
                Layout = new LinearLayout()
                {
                    LinearOrientation = LinearLayout.Orientation.Horizontal,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                },
            };

            var textFieldView = new View()
            {
                BackgroundColor = isLightTheme ? new Color("#FAFAFA") : new Color("#1D1A21"),
                WidthSpecification = LayoutParamPolicies.WrapContent,
                WidthResizePolicy = ResizePolicyType.FitToChildren,
                SizeHeight = 48.SpToPx(),
                Layout = new LinearLayout()
                {
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                },
                Padding = new Extents(10, 0, 0, 0).SpToPx(),
            };

            //entry view
            PropertyMap placeholder = new PropertyMap();
            placeholder.Add("color", new PropertyValue(isLightTheme ? new Color("#CACACA") : new Color("#666666")));
            placeholder.Add("fontFamily", new PropertyValue("BreezeSans"));
            placeholder.Add("pixelSize", new PropertyValue(24.SpToPx()));
            placeholder.Add("text", new PropertyValue("Type text"));

            textField = new TextField
            {
                FontFamily = "BreezeSans",
                SizeWidth = 582.SpToPx(),
                SizeHeight = 48.SpToPx(),
                VerticalAlignment = VerticalAlignment.Center,
                Placeholder = placeholder,
                BackgroundColor = isLightTheme ? new Color("#FAFAFA") : new Color("#1D1A21"),
                MaxLength = MAX_DEVICE_NAME_LEN,
                EnableCursorBlink = true,
                PixelSize = 24.SpToPx(),
                Text = name,
            };
            textField.TextChanged += TextField_TextChanged;

            CancelButton cancelTextButton = new CancelButton()
            {
                Margin = new Extents(10, 0, 0, 0).SpToPx(),
            };
            cancelTextButton.Clicked += cancelTextButton_Clicked;

            textFieldView.Add(textField);
            entryView.Add(textFieldView);
            entryView.Add(cancelTextButton);

            View separator = new View
            {
                WidthResizePolicy = ResizePolicyType.FillToParent,
                Size = new Size(592, 1).SpToPx(),
                BackgroundColor = isLightTheme ? new Color("#FF6200") : new Color("#FF8A00"),
            };

            warning = new TextLabel(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_TPOP_MAXIMUM_NUMBER_OF_CHARACTERS_REACHED)))
            {
                PixelSize = 16.SpToPx(),
                TextColor = new Color("#A40404"),
                Margin = new Extents(8, 8, 8, 0).SpToPx(),
            };

            mainView.Add(entryView);
            mainView.Add(separator);
            mainView.Add(warning);

            header.Add(textView);
            header.Add(mainView);

            var buttons = new View()
            {
                BackgroundColor = Color.Transparent,
                SizeWidth = 658.SpToPx(),
                Layout = new FlexLayout()
                {
                    Justification = FlexLayout.FlexJustification.SpaceBetween,
                    Direction = FlexLayout.FlexDirection.Row
                },
                Padding = new Extents(16, 16, 16, 16).SpToPx(),
            };

            renameButton = new Button()
            {
                WidthResizePolicy = ResizePolicyType.FitToChildren,
                HeightResizePolicy = ResizePolicyType.FitToChildren,
                Text = NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BUTTON_RENAME)),
                Size = new Size(252, 48).SpToPx(),
            };
            renameButton.Clicked += (object sender, ClickedEventArgs e) => {
                Vconf.SetString(VconfDeviceName, textField.Text);
                NUIApplication.GetDefaultWindow().GetDefaultNavigator().Pop();
            };

            var cancelButton = new Button("Tizen.NUI.Components.Button.Outlined")
            {
                WidthResizePolicy = ResizePolicyType.FitToChildren,
                HeightResizePolicy = ResizePolicyType.FitToChildren,
                Text = NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BUTTON_CANCEL)),
                Size = new Size(252, 48).SpToPx(),
            };

            cancelButton.Clicked += (object sender, ClickedEventArgs e) => { NUIApplication.GetDefaultWindow().GetDefaultNavigator().Pop(); };
            buttons.Add(cancelButton);
            buttons.Add(renameButton);

            content.Add(header);
            content.Add(buttons);
            popup.Add(content);

            checkNameLength(textField);

            RoundedDialogPage.ShowDialog(popup);
        }

        private void TextField_TextChanged(object sender, TextField.TextChangedEventArgs e)
        {
            checkNameLength(e.TextField);
        }

        private void checkNameLength(TextField textField)
        {
            if (textField.Text.Length >= MAX_DEVICE_NAME_LEN)
            {
                warning.Show();
                renameButton.IsEnabled = false;
            }
            else
            {
                warning.Hide();
                renameButton.IsEnabled = true;
            }

            if (textField.Text == string.Empty)
            {
                renameButton.IsEnabled = false;
            }
        }

        private void cancelTextButton_Clicked(object sender, ClickedEventArgs e)
        {
            textField.Text = string.Empty;
        }
    }
}
