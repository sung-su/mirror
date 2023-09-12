using SettingCore;
using SettingCore.Views;
using SettingMainGadget.Apps;
using SettingMainGadget.TextResources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tizen.Applications;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;

namespace Setting.Menu.Apps
{
    public class AppsManagerGadget : MenuGadget
    {
        public override string ProvideTitle() => NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_APPLICATION_MANAGER));

        private string defaultIcon = System.IO.Path.Combine(Application.Current.DirectoryInfo.Resource, "default_app_icon.svg");

        private View content;
        private View installedAppsContent;
        private View runningAppsContent;
        private View allAppsContent;

        private Loading installedAppsIndicator;
        private PackageSizeInformation packageSizeInfo;

        private List<Package> allPackages = new List<Package>();
        private Dictionary<Package, TextWithIconListItem> installedApps = new Dictionary<Package, TextWithIconListItem>();
        private Dictionary<Package, TextWithIconListItem> allApps = new Dictionary<Package, TextWithIconListItem>();
        private Dictionary<ApplicationInfo, TextWithIconListItem> runningApps = new Dictionary<ApplicationInfo, TextWithIconListItem>();

        protected override View OnCreate()
        {
            base.OnCreate();

            content = new View
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
                Layout = new LinearLayout()
                {
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                },
            };

            AddTabs();
            OnPageAppeared += AddTabsContent;

            return content;
        }

        private void AddTabs()
        {
            var tabView = new TabView()
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
            };

            // installed apps tab

            var installedAppsTabButton = new TabButton()
            {
                Text = NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_DOWNLOADS))
            };

            installedAppsContent = new View()
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
                Layout = new LinearLayout()
                {
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                },
            };

            installedAppsIndicator = new Loading();
            installedAppsIndicator.Play();
            installedAppsContent.Add(installedAppsIndicator);

            // running apps tab

            var runningAppsTabButton = new TabButton()
            {
                Text = NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_RUNNING))
            };

            runningAppsContent = CreateScrollableBase();

            // all apps tab

            var allAppsTabButton = new TabButton()
            {
                Text = NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_OPT_ALL))
            };

            allAppsContent = CreateScrollableBase();

            tabView.AddTab(installedAppsTabButton, installedAppsContent);
            tabView.AddTab(runningAppsTabButton, runningAppsContent);
            tabView.AddTab(allAppsTabButton, allAppsContent);

            content.Add(tabView);
        }

        private void AddTabsContent()
        {
            allPackages = PackageManager.GetPackages().ToList();

            AddInstalledApps(installedAppsContent);
            _ = AddRunningAppsAsync(runningAppsContent);
            AddAllApps(allAppsContent);
        }

        private void AddInstalledApps(View content)
        {
            var packages = allPackages.Where(a => a.IsPreloaded == false && !string.IsNullOrEmpty(a.Label) && a.PackageType != PackageType.WGT).OrderBy(x => x.Label).ToList();

            if (packages.Count() == 0)
            {
                var noAppsLabel = new TextLabel
                {
                    WidthSpecification = LayoutParamPolicies.MatchParent,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Text = NUIGadgetResourceManager.GetString(nameof(Resources.IDS_FP_BODY_NO_APPLICATIONS)),
                    PixelSize = 24.SpToPx(),
                    TextColor = IsLightTheme ? new Color("#CACACA") : new Color("#666666"),
                };
                var infoLabel = new TextLabel
                {
                    WidthSpecification = LayoutParamPolicies.MatchParent,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    MultiLine = true,
                    Ellipsis = true,
                    Text = NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_AFTER_YOU_DOWNLOAD_AND_INSTALL_APPLICATIONS_APPLICATIONS_WILL_BE_SHOWN_HERE)),
                    PixelSize = 24.SpToPx(),
                    Margin = new Extents(16, 16, 0, 0).SpToPx(),
                    TextColor = IsLightTheme ? new Color("#CACACA") : new Color("#666666"),
                };

                content.Add(noAppsLabel);
                content.Add(infoLabel);

                installedAppsIndicator.Stop();
                installedAppsIndicator.Unparent();
                installedAppsIndicator.Dispose();

                return;
            }

            var scrollView = CreateScrollableBase();

            scrollView.Add(CreateAppSizeLabel());

            foreach (var package in packages)
            {
                var iconPath = File.Exists(package.IconPath) ? package.IconPath : defaultIcon;

                var appItem = new TextWithIconListItem(package.Label, Color.Transparent, iconPath: iconPath, subText: NUIGadgetResourceManager.GetString(nameof(Resources.IDS_SM_SBODY_CALCULATING_ING)));
                appItem.Clicked += (s, e) =>
                {
                    // TODO : goto app info by AppId
                };

                installedApps.Add(package, appItem);

                scrollView.Add(appItem);
                content.Add(scrollView);
            }

            installedAppsIndicator.Stop();
            installedAppsIndicator.Unparent();
            installedAppsIndicator.Dispose();

            _ = UpdateSizeInfo(installedApps);
        }

        private async Task AddRunningAppsAsync(View content)
        {
            var runningApplicationsContexts = await ApplicationManager.GetAllRunningApplicationsAsync();

            content.Add(CreateAppSizeLabel());

            var applicationInfoList = new List<ApplicationInfo>();

            foreach (var application in runningApplicationsContexts)
            {
                applicationInfoList.Add(new ApplicationInfo(application.ApplicationId));
            }

            applicationInfoList = applicationInfoList.OrderBy(x => x.Label).ToList();

            foreach (var applicationInfo in applicationInfoList)
            {
                var iconPath = File.Exists(applicationInfo.IconPath) ? applicationInfo.IconPath : defaultIcon;

                var appItem = new TextWithIconListItem(applicationInfo.Label, Color.Transparent, iconPath: iconPath, subText: NUIGadgetResourceManager.GetString(nameof(Resources.IDS_SM_SBODY_CALCULATING_ING)));
                appItem.Clicked += (s, e) =>
                {
                    // TODO : goto app info by AppId
                };

                runningApps.Add(applicationInfo, appItem);
                content.Add(appItem);
            }

            UpdateRAMSizeInfo(runningApps);
        }

        private void AddAllApps(View content)
        {
            var packages = allPackages.Where(x => !string.IsNullOrEmpty(x.Label) && x.PackageType != PackageType.WGT).OrderBy(x => x.Label).ToList();

            content.Add(CreateAppSizeLabel());

            foreach (var package in packages)
            {
                var iconPath = File.Exists(package.IconPath) ? package.IconPath : defaultIcon;

                var appItem = new TextWithIconListItem(package.Label, Color.Transparent, iconPath: iconPath, subText: NUIGadgetResourceManager.GetString(nameof(Resources.IDS_SM_SBODY_CALCULATING_ING)));
                appItem.Clicked += (s, e) =>
                {
                    // TODO : goto app info by AppId
                };

                allApps.Add(package, appItem);
                content.Add(appItem);
            }

            _ = UpdateSizeInfo(allApps);
        }

        private async Task UpdateSizeInfo(Dictionary<Package, TextWithIconListItem> packages)
        {
            foreach (var package in packages)
            {
                packageSizeInfo = await package.Key.GetSizeInformationAsync();
                package.Value.SubText = AppManager.GetSizeString(packageSizeInfo.AppSize);
            }
        }

        private void UpdateRAMSizeInfo(Dictionary<ApplicationInfo, TextWithIconListItem> infos)
        {
            foreach (var info in infos)
            {
                var appContext = new ApplicationRunningContext(info.Key.ApplicationId);
                var processMemmory = new Tizen.System.ProcessMemoryUsage(new List<int> { appContext.ProcessId });
                processMemmory.Update(new List<int> { appContext.ProcessId });
                info.Value.SubText = AppManager.GetSizeString(processMemmory.GetVsz(appContext.ProcessId));
            }
        }

        private ScrollableBase CreateScrollableBase()
        {
            return new ScrollableBase()
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
        }

        private TextLabel CreateAppSizeLabel()
        {
            return new TextLabel
            {
                Text = "App size", // TODO : add translation to Resources 
                TextColor = IsLightTheme ? new Color("#83868F") : new Color("#666666"),
                PixelSize = 24.SpToPx(),
                Margin = new Extents(20, 0, 16, 16).SpToPx(),
            };
        }
    }
}
