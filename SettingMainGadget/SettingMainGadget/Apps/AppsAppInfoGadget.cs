using SettingCore;
using SettingCore.Views;
using SettingMainGadget.Apps;
using SettingMainGadget.TextResources;
using System.Linq;
using System.Threading.Tasks;
using Tizen.Applications;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.Security;

namespace Setting.Menu.Apps
{
    public class AppsAppInfoGadget : MenuGadget
    {
        // update the resources according to the GUI, application info -> app info
        public override string ProvideTitle() => NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_APPLICATION_INFO));

        private ScrollableBase content;

        protected override View OnCreate()
        {
            base.OnCreate();

            content = new ScrollableBase
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

            _ = CreateContent();

            return content;
        }

        private async Task CreateContent()
        {
            var app = AppManager.CurrentApp;
            var appInfo = ApplicationManager.GetInstalledApplication(app.Id);
            var appContext = AppManager.GetRunningContext();

            var appInfoView = new View()
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.WrapContent,
                Layout = new LinearLayout()
                {
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                },
            };

            var appVersion = new TextWithIconListItem(app.Label, Color.Transparent, iconPath: app.IconPath, subText: $"{NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_VERSION))} {app.Version}");

            var close = new Button("Tizen.NUI.Components.Button.Outlined")
            {
                Text = NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BUTTON_FORCE_STOP)),
                Size = new Size(252, 48).SpToPx(),
                WidthResizePolicy = ResizePolicyType.FitToChildren,
                HeightResizePolicy = ResizePolicyType.FitToChildren,
                IsEnabled = appContext != null && appContext.State == ApplicationRunningContext.AppState.Background,
            };

            close.Clicked += (s, e) =>
            {
                try
                {
                    if (!appContext.IsTerminated)
                    {
                        ApplicationManager.TerminateBackgroundApplication(appContext);
                    }
                }
                catch (System.Exception ex)
                {
                    Logger.Warn($"Couldn't close the application {app.Id}, {ex.Message}");
                }
            };

            var uninstall = new Button("Tizen.NUI.Components.Button.Outlined")
            {
                Text = NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BUTTON_UNINSTALL)),
                Size = new Size(252, 48).SpToPx(),
                WidthResizePolicy = ResizePolicyType.FitToChildren,
                HeightResizePolicy = ResizePolicyType.FitToChildren,
                IsEnabled = app.IsRemovable
            };

            uninstall.Clicked += (s, e) =>
            {
                ShowUninstallPopup(app.Id);
            };

            var buttonsView = new View()
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.WrapContent,
                Layout = new FlexLayout()
                {
                    Alignment = FlexLayout.AlignmentType.Center,
                    Justification = FlexLayout.FlexJustification.SpaceEvenly,
                    Direction = FlexLayout.FlexDirection.Row,
                },
                Margin = new Extents(0, 0, 16, 24).SpToPx(),
            };

            buttonsView.Add(close);
            buttonsView.Add(uninstall);

            var infoView = new View()
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.WrapContent,
                Layout = new LinearLayout()
                {
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                },
            };

            var packageSizeInfo = await app.GetSizeInformationAsync();

            var appSize = packageSizeInfo.AppSize + packageSizeInfo.ExternalAppSize;
            var userDataSize = packageSizeInfo.DataSize + packageSizeInfo.ExternalDataSize;
            var cachedSize = packageSizeInfo.CacheSize + packageSizeInfo.ExternalCacheSize;
            var totalSize = appSize + userDataSize + cachedSize;

            infoView.Add(new TextHeaderListItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_STORAGE))));
            infoView.Add(TextListItem.CreatePrimaryTextItemWithSubText(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_TOTAL_SIZE)), AppManager.GetSizeString(totalSize)));
            infoView.Add(TextListItem.CreatePrimaryTextItemWithSubText(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_APPLICATION)), AppManager.GetSizeString(appSize)));
            infoView.Add(TextListItem.CreatePrimaryTextItemWithSubText(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_USER_DATA)), AppManager.GetSizeString(userDataSize)));

            infoView.Add(new TextHeaderListItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_TMBODY_CACHE))));
            infoView.Add(TextListItem.CreatePrimaryTextItemWithSubText(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_TMBODY_CACHE)), AppManager.GetSizeString(cachedSize)));
            var clearCache = TextListItem.CreatePrimaryTextItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_HEADER_CLEAR_CACHE_ABB)));

            if (cachedSize > 0)
            {
                clearCache.Clicked += (s, e) =>
                {
                    PackageManager.ClearCacheDirectory(app.Id);
                };
            }
            else
            {
                clearCache.IsEnabled = false;
            }

            infoView.Add(clearCache);

            var defaultApps = AppControl.GetDefaultApplicationIds();

            if (defaultApps != null && defaultApps.Contains(app.Id))
            {
                infoView.Add(new TextHeaderListItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_DEFAULT_APP_SETTINGS))));
                var defaultApp = TextListItem.CreatePrimaryTextItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_CLEAR_DEFAULT_APP_SETTINGS)));
                clearCache.Clicked += (s, e) =>
                {
                    // TODO : clear defaults
                };
            }

            if (app.PackageType == PackageType.WGT)
            {
                infoView.Add(new TextHeaderListItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_WEB_APP))));
                var webSettings = TextListItem.CreatePrimaryTextItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_WEBSITE_SETTINGS)));
                webSettings.Clicked += (s, e) =>
                {
                    // TODO : web settings
                };
            }

            if (app.Privileges.Count() > 0)
            {
                infoView.Add(new TextHeaderListItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_HEADER_PRIVILEGES))));

                foreach (var privilege in app.Privileges)
                {
                    try
                    {
                        // TODO : how to get api version
                        var privilegeItem = TextListItem.CreatePrimaryTextItemWithSubText(Privilege.GetDisplayName("9", privilege), Privilege.GetDescription("9", privilege));
                        infoView.Add(privilegeItem);
                    }
                    catch (System.Exception ex)
                    {
                        Logger.Warn($"{ex.Message}");
                    }
                }
            }

            appInfoView.Add(appVersion);
            content.Add(appInfoView);
            content.Add(buttonsView);
            content.Add(infoView);
        }

        private void ShowUninstallPopup(string appid)
        {
            var content = new View()
            {
                BackgroundColor = ThemeManager.PlatformThemeId == "org.tizen.default-light-theme" ? new Color("#FAFAFA") : new Color("#16131A"),
                WidthSpecification = LayoutParamPolicies.WrapContent,
                HeightSpecification = LayoutParamPolicies.WrapContent,
                Layout = new LinearLayout()
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                },
            };

            // title text
            var textTitle = new TextLabel(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BUTTON_UNINSTALL)))
            {
                FontFamily = "BreezeSans",
                PixelSize = 24.SpToPx(),
                Margin = new Extents(0, 0, 24, 16).SpToPx(),
            };

            content.Add(textTitle);

            // main text
            var textSubTitle = new TextLabel(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_APP_WILL_BE_UNINSTALLED)))
            {
                FontFamily = "BreezeSans",
                PixelSize = 24.SpToPx(),
                SizeWidth = 618.SpToPx(),
                MultiLine = true,
                LineWrapMode = LineWrapMode.Word,
                Margin = new Extents(24, 24, 0, 24).SpToPx(),
            };
            content.Add(textSubTitle);

            // buttons
            View buttons = new View()
            {
                HeightSpecification = LayoutParamPolicies.WrapContent,
                Layout = new FlexLayout()
                {
                    Alignment = FlexLayout.AlignmentType.Center,
                    Justification = FlexLayout.FlexJustification.SpaceBetween,
                    Direction = FlexLayout.FlexDirection.Row,
                },
                Size2D = new Size2D(658, 80).SpToPx(),
                Padding = new Extents(16, 16, 16, 16).SpToPx(),
                Margin = new Extents(16, 16, 0, 16).SpToPx(),
            };

            var uninstallButton = new Button()
            {
                WidthResizePolicy = ResizePolicyType.FitToChildren,
                HeightResizePolicy = ResizePolicyType.FitToChildren,
                Text = NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BUTTON_UNINSTALL)),
                Size = new Size(252, 48).SpToPx(),
            };

            uninstallButton.Clicked += (o, e) =>
            {
                var appInfoLabel = ApplicationManager.GetInstalledApplication(appid).Label;
                NUIApplication.GetDefaultWindow().GetDefaultNavigator().Pop();

                try
                {
                    if (PackageManager.Uninstall(appid))
                    {
                        Notification.MakeToast($"{appInfoLabel} uninstalled.", Notification.ToastBottom).Post(Notification.ToastShort);
                        GadgetNavigation.NavigateBack();
                    }
                    else
                    {
                        Notification.MakeToast(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_POP_FAILED_TO_UNINSTALL_THE_APP)), Notification.ToastBottom).Post(Notification.ToastShort);
                    }
                }
                catch (System.Exception ex)
                {
                    Logger.Warn($"Couldn't uninstall the application {appid}, {ex.Message}");
                    Notification.MakeToast(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_POP_FAILED_TO_UNINSTALL_THE_APP)), Notification.ToastBottom).Post(Notification.ToastShort);
                }
            };

            var cancelButton = new Button("Tizen.NUI.Components.Button.Outlined")
            {
                WidthResizePolicy = ResizePolicyType.FitToChildren,
                HeightResizePolicy = ResizePolicyType.FitToChildren,
                Text = NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BUTTON_CANCEL)),
                Size = new Size(252, 48).SpToPx(),
            };

            cancelButton.Clicked += (o, e) => { NUIApplication.GetDefaultWindow().GetDefaultNavigator().Pop(); };

            buttons.Add(cancelButton);
            buttons.Add(uninstallButton);

            content.Add(buttons);

            RoundedDialogPage.ShowDialog(content);
        }
    }
}
