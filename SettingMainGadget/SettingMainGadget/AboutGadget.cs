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
using SettingMainGadget.Common.Views;
using SettingMainGadget.Common;

namespace Setting.Menu
{
    public class AboutGadget : SettingCore.MainMenuGadget
    {
        private const int MAX_DEVICE_NAME_LEN = 32;
        private TextListItem renameDevice;
        private string VconfDeviceName = "db/setting/device_name";
        private string SystemModelName = "tizen.org/system/model_name";

        private ScrollableBase content;
        private AlertDialog renameAlertDialog;
        private TextField renameTextField;
        private TextLabel warning;
        private Button renameButton;
        private string deviceName;

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
            if(renameAlertDialog != null && renameAlertDialog.IsOnWindow)
            {
                string currentName = renameTextField.Text;
                RemoveAlertDialog(renameAlertDialog);
                CreateRenameDeviceAlertDialog(currentName);
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
            content.RemoveAllChildren(true);
            sections.Clear();

            sections.Add(MainMenuProvider.About_ManageCertificates, () =>
            {
                var manageCertificates = TextListItem.CreatePrimaryTextItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_HEADER_MANAGE_CERTIFICATES_ABB)));
                manageCertificates.Clicked += (s, e) =>
                {
                    NavigateTo(MainMenuProvider.About_ManageCertificates);
                };
                content.Add(manageCertificates);
            });

            sections.Add(MainMenuProvider.About_OpenSourceLicenses, () =>
            {
                var openSourceLicenses = TextListItem.CreatePrimaryTextItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_OPEN_SOURCE_LICENCES)));
                openSourceLicenses.Clicked += (s, e) =>
                {
                    NavigateTo(MainMenuProvider.About_OpenSourceLicenses);
                };
                content.Add(openSourceLicenses);
            });

            sections.Add(MainMenuProvider.About_ScalableUI, () =>
            {
                var scalableUI = TextListItem.CreatePrimaryTextItem("Scalable UI for Developers");
                scalableUI.Clicked += (s, e) =>
                {
                    NavigateTo(MainMenuProvider.About_ScalableUI);
                };
                content.Add(scalableUI);
            });

            sections.Add(MainMenuProvider.About_ScalableUI, () =>
            {
                var scalableUI = TextListItem.CreatePrimaryTextItem("Scalable UI for Developers");
                scalableUI.Clicked += (s, e) =>
                {
                    NavigateTo(MainMenuProvider.About_ScalableUI);
                };
                content.Add(scalableUI);
            });

            sections.Add(MainMenuProvider.About_DeviceInfo, () =>
            {
                var deviceInfo = new TextHeaderListItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_DEVICE_INFO)));
                content.Add(deviceInfo);
            });

            sections.Add(MainMenuProvider.About_RenameDevice, () =>
            {
                if (Vconf.TryGetString(VconfDeviceName, out deviceName))
                {
                    Logger.Warn($"Could not get vconf value: {VconfDeviceName}");
                }

                renameDevice = TextListItem.CreatePrimaryTextItemWithSecondaryText(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_NAME)), deviceName);
                renameDevice.Clicked += (s, e) =>
                {
                    CreateRenameDeviceAlertDialog(deviceName);
                };
                content.Add(renameDevice);
            });

            sections.Add(MainMenuProvider.About_ModelNumber, () =>
            {
                bool result = Tizen.System.Information.TryGetValue<string>("http://tizen.org/system/model_name", out string modelNumberText);
                var modelNumber = TextListItem.CreatePrimaryTextItemWithSecondaryText(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_MODEL_NUMBER)), result ? modelNumberText : NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_HEADER_UNAVAILABLE)));
                content.Add(modelNumber);
            });

            sections.Add(MainMenuProvider.About_TizenVersion, () =>
            {
                bool result = Tizen.System.Information.TryGetValue<string>("http://tizen.org/feature/platform.version", out string platformVersionText);
                var platformVersion = TextListItem.CreatePrimaryTextItemWithSecondaryText(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_MBODY_TIZEN_VERSION)), result ? platformVersionText : NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_HEADER_UNAVAILABLE)));
                platformVersion.MultiTap += (s, e) =>
                {
                    GadgetManager.Instance.ChangeMenuPathOrder(MainMenuProvider.About_ScalableUI, 30);

                    var toast = Notification.MakeToast("Scalable UI for Developers menu enabled", Notification.ToastCenter);
                    toast.Post(1000);
                };
                content.Add(platformVersion);
            });

            sections.Add(MainMenuProvider.About_Cpu, () =>
            {
                bool result = Tizen.System.Information.TryGetValue<string>("http://tizen.org/system/platform.processor", out string platformProcessorText);
                var cpu = TextListItem.CreatePrimaryTextItemWithSecondaryText("CPU", result ? platformProcessorText : NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_HEADER_UNAVAILABLE)));
                content.Add(cpu);
            });

            sections.Add(MainMenuProvider.About_Ram, () =>
            {
                var memusage = new Tizen.System.SystemMemoryUsage();
                float ram_total_gb = memusage.Total / (float)(1024 * 1024);
                var ram = TextListItem.CreatePrimaryTextItemWithSecondaryText("RAM", string.Format("{0:0.0} GB", ram_total_gb));
                content.Add(ram);
            });

            sections.Add(MainMenuProvider.About_Resolution, () =>
            {
                var screenSize = NUIApplication.GetScreenSize();
                var resolution = TextListItem.CreatePrimaryTextItemWithSecondaryText(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_RESOLUTION)), $"{screenSize.Width} x {screenSize.Height}");
                content.Add(resolution);
            });

            if (IsEmulBin() == false)
            {
                sections.Add(MainMenuProvider.About_DeviceStatus, () =>
                {
                    var showOther = TextListItem.CreatePrimaryTextItemWithSubText(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_STATUS)), NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_SHOW_NETWORK_STATUS_AND_OTHER_INFORMATION)));
                    showOther.Clicked += (s, e) =>
                    {
                        NavigateTo(MainMenuProvider.About_DeviceStatus);
                    };
                    content.Add(showOther);
                });
            }

            CreateItems();
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

        private void CreateRenameDeviceAlertDialog(string name)
        {
            View contentArea = new View()
            {
                Name = "RenameDeviceDialog",
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.WrapContent,
                BackgroundColor = Color.Transparent,
                Layout = new LinearLayout()
                {
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Extents(0, 0, 8, 16).SpToPx(),
                }
            };

            TextLabel contentMessgae = new TextLabel()
            {
                StyleName = "LabelText",
                ThemeChangeSensitive = true,
                PixelSize = 18.SpToPx(),
                FontFamily = "BreezeSans",
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.WrapContent,
                HorizontalAlignment = HorizontalAlignment.Center,
                Text = NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_DEVICE_NAMES_ARE_DISPLAYED)),
                MultiLine = true,
            };
            contentArea.Add(contentMessgae);

            View inputArea = new View()
            {
                Name = "AlertDialogInputArea",
                BackgroundColor = Color.Transparent,
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = 48.SpToPx(),
                Margin = new Extents(0, 0, 24, 0).SpToPx(),
                Layout = new RelativeLayout()
            };
            contentArea.Add(inputArea);

            View inputBaseLine= new View()
            {
                Name = "InputLineView",
                StyleName = "InputLine",
                HeightSpecification = 2.SpToPx(),
                WidthSpecification = LayoutParamPolicies.MatchParent,
                BackgroundColor= AppAttributes.IsLightTheme ? Color.Black : Color.White,
            };
            contentArea.Add(inputBaseLine);

            PropertyMap placeholder = new PropertyMap();
            placeholder.Add("color", new PropertyValue(AppAttributes.IsLightTheme ? new Color("#CACACA") : new Color("#666666")));
            placeholder.Add("fontFamily", new PropertyValue("BreezeSans"));
            placeholder.Add("pixelSize", new PropertyValue(24.SpToPx()));
            placeholder.Add("text", new PropertyValue("Enter Device Name"));

            renameTextField = new TextField()
            {
                Name = "AlertDialogInputField",
                Text = name,
                HorizontalAlignment = HorizontalAlignment.Begin,
                PixelSize = 24.SpToPx(),
                Placeholder = placeholder,
            };
            renameTextField.TextChanged += TextField_TextChanged;
            inputArea.Add(renameTextField);

            RelativeLayout.SetLeftRelativeOffset(renameTextField, 0.0f);
            RelativeLayout.SetFillHorizontal(renameTextField, true);
            RelativeLayout.SetHorizontalAlignment(renameTextField, RelativeLayout.Alignment.Start);

            IconButton clearButton = new IconButton(GetResourcePath("cross_button.png"));
            inputArea.Add(clearButton);

            clearButton.Clicked += (object o, ClickedEventArgs e) =>
            {
                renameTextField.Text = string.Empty;
            };

            RelativeLayout.SetRightRelativeOffset(clearButton, 1.0f);
            RelativeLayout.SetHorizontalAlignment(clearButton, RelativeLayout.Alignment.End);
            RelativeLayout.SetRightTarget(renameTextField, clearButton);
            RelativeLayout.SetRightRelativeOffset(renameTextField, 0.0f);

            View buttonArea = new View()
            {
                BackgroundColor = Color.Transparent,
                HeightSpecification = LayoutParamPolicies.WrapContent,
                WidthSpecification = LayoutParamPolicies.MatchParent,
                Layout = new FlexLayout()
                {
                    Direction = FlexLayout.FlexDirection.Row,
                    Justification = FlexLayout.FlexJustification.SpaceBetween,
                },
            };

            Button cancelButton = new Button("Tizen.NUI.Components.Button.Outlined")
            {
                Name = "AlertDialogCreateButton",
                Text = "Cancel",
                WidthResizePolicy = ResizePolicyType.FitToChildren,
                HeightResizePolicy = ResizePolicyType.FitToChildren,
                CornerRadius = AppAttributes.PopupActionButtonCornerRadius,
                Size2D = AppAttributes.PopupActionButtonSize,
                Margin = AppAttributes.PopupActionButtonMargin,
            };
            buttonArea.Add(cancelButton);

            renameButton= new Button()
            {
                Name = "AlertDialogCreateButton",
                Text = "Rename",
                WidthResizePolicy = ResizePolicyType.FitToChildren,
                HeightResizePolicy = ResizePolicyType.FitToChildren,
                Size2D = AppAttributes.PopupActionButtonSize,
                CornerRadius = AppAttributes.PopupActionButtonCornerRadius,
                Margin = AppAttributes.PopupActionButtonMargin,
            };
            buttonArea.Add(renameButton);

            warning = new TextLabel(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_TPOP_MAXIMUM_NUMBER_OF_CHARACTERS_REACHED)))
            {
                PixelSize = 16.SpToPx(),
                TextColor = new Color("#A40404"),
                Margin = new Extents(8, 8, 8, 0).SpToPx(),
            };
            warning.Hide();
            contentArea.Add(warning);

            renameAlertDialog = new AlertDialog()
            {
                ThemeChangeSensitive = true,
                StyleName = "Dialogs",
                WidthSpecification = AppAttributes.IsPortrait ? AppAttributes.WindowWidth - 64.SpToPx() : (int)(AppAttributes.WindowWidth * 0.7f),
                HeightSpecification = LayoutParamPolicies.WrapContent,
                Layout = new LinearLayout()
                {
                    Padding = new Extents(80, 80, 40, 40).SpToPx(),
                    Margin = new Extents(32, 32, 0, 0).SpToPx(),
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Top,
                },
                TitleContent = new TextLabel()
                {
                    Name = "AlertDialogTitle",
                    StyleName = "LabelText",
                    ThemeChangeSensitive = true,
                    PixelSize = 40.SpToPx(),
                    FontFamily = "BreezeSans",
                    WidthSpecification = LayoutParamPolicies.MatchParent,
                    HeightSpecification = LayoutParamPolicies.WrapContent,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Text = NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_HEADER_RENAME_DEVICE)),
                },
                Content = contentArea,
                ActionContent = buttonArea,
                BoxShadow = AppAttributes.PopupBoxShadow,
            };
            if (cancelButton.SizeWidth > renameAlertDialog.SizeWidth / 2 - 40.SpToPx())
            {
                cancelButton.SizeWidth = renameAlertDialog.SizeWidth / 2 - 40.SpToPx();
                renameButton.SizeWidth = renameAlertDialog.SizeWidth / 2 - 40.SpToPx();
            }
            AppAttributes.DefaultWindow.Add(renameAlertDialog);

            cancelButton.Clicked += (object o, ClickedEventArgs e) =>
            {
                RemoveAlertDialog(renameAlertDialog);
            };

            renameButton.Clicked += (object sender, ClickedEventArgs e) => {
                string newDeviceName = renameTextField.Text;
                if (newDeviceName.Length > 0)
                {
                    Vconf.SetString(VconfDeviceName, renameTextField.Text);
                }
                RemoveAlertDialog(renameAlertDialog);
            };
        }

        private void DeleteChildren(List<View> childrenList)
        {
            if (childrenList == null || childrenList.Count == 0)
            {
                return;
            }
            foreach (View child in childrenList)
            {
                child?.Dispose();
            }
        }

        private void RemoveAlertDialog(AlertDialog dialog)
        {
            List<View> contentChilds = dialog.Content.Children.GetRange(0, dialog.Content.Children.Count);
            List<View> actionContentChilds = dialog.ActionContent.Children.GetRange(0, dialog.ActionContent.Children.Count);
            foreach (View child in contentChilds)
            {
                if (child != null)
                {
                    if (child.Name == "AlertDialogInputArea")
                    {
                        List<View> inputAreaChildrenList = child.Children.GetRange(0, child.Children.Count);
                        DeleteChildren(inputAreaChildrenList);
                    }
                    child.Dispose();
                }
            }
            DeleteChildren(actionContentChilds);
            AppAttributes.DefaultWindow.Remove(dialog);
            dialog.Dispose();
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
            renameTextField.Text = string.Empty;
        }
    }
}
