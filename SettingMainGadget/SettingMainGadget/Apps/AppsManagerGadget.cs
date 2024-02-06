using SettingCore;
using SettingCore.Views;
using SettingMainGadget;
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
using Tizen.NUI.Binding;
using Tizen.NUI.Components;

namespace Setting.Menu.Apps
{
    public class AppsManagerGadget : MenuGadget
    {
        public override string ProvideTitle() => NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_APPLICATION_MANAGER));

        private string defaultIcon = System.IO.Path.Combine(Application.Current.DirectoryInfo.Resource, "default_app_icon.svg");

        public override IEnumerable<MoreMenuItem> ProvideMoreMenu() => MoreMenu();

        private View content;
        private View installedAppsContent;
        private View runningAppsContent;
        private View allAppsContent;

        private CollectionView installedAppsView;
        private CollectionView runningAppsView;
        private CollectionView allAppsView;

        private MoreMenuItem sortBySizeMenuItem;
        private MoreMenuItem sortByNameMenuItem;

        private Loading installedAppsIndicator;
        private List<Package> allPackages = new List<Package>();

        private List<AppManager.ApplicationItemInfo> installedAppsInfos = new List<AppManager.ApplicationItemInfo>();
        private List<AppManager.ApplicationItemInfo> runningAppsInfos = new List<AppManager.ApplicationItemInfo>();
        private List<AppManager.ApplicationItemInfo> allAppsInfos = new List<AppManager.ApplicationItemInfo>();

        private SortType currentSortType = SortType.name_asc;

        private List<MoreMenuItem> MoreMenu()
        {
            sortBySizeMenuItem = new MoreMenuItem()
            {
                Text = $"{NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_SIZE))}: {NUIGadgetResourceManager.GetString(nameof(Resources.IDS_SM_SBODY_CALCULATING_ING))}",
            };

            sortByNameMenuItem = new MoreMenuItem()
            {
                Text = NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_NAME)),
                Action = () => { SortApplications(currentSortType != SortType.name_asc ? SortType.name_asc : SortType.name_desc); }
            };

            return new List<MoreMenuItem>
            {
                new MoreMenuItem()
                {
                    Text = NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_HEADER_SORT_BY)),
                },
                sortBySizeMenuItem,
                sortByNameMenuItem,
            };
        }

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

            PackageManager.UninstallProgressChanged += PackageManager_UninstallProgressChanged;
            PackageManager.InstallProgressChanged += PackageManager_InstallProgressChanged;

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

            var tabButtonStyle = ThemeManager.GetStyle("Tizen.NUI.Components.TabButton") as TabButtonStyle;
            tabButtonStyle.Padding = new Extents(2, 2, 16, 16).SpToPx();
            tabButtonStyle.Icon.Size = new Size(2, -1).SpToPx();

            // installed apps tab

            var installedAppsTabButton = new TabButton(tabButtonStyle)
            {
                Text = NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_DOWNLOADS))
            };

            installedAppsContent = TabView();

            var layout = installedAppsContent.Layout as LinearLayout;
            layout.HorizontalAlignment = HorizontalAlignment.Center;

            installedAppsIndicator = new Loading();
            installedAppsIndicator.Play();
            installedAppsContent.Add(installedAppsIndicator);

            // running apps tab

            var runningAppsTabButton = new TabButton(tabButtonStyle)
            {
                Text = NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_RUNNING))
            };

            runningAppsContent = TabView();

            // all apps tab

            var allAppsTabButton = new TabButton(tabButtonStyle)
            {
                Text = NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_OPT_ALL))
            };

            allAppsContent = TabView();

            tabView.AddTab(installedAppsTabButton, installedAppsContent);
            tabView.AddTab(runningAppsTabButton, runningAppsContent);
            tabView.AddTab(allAppsTabButton, allAppsContent);

            content.Add(tabView);
        }

        private async void AddTabsContent()
        {
            OnPageAppeared -= AddTabsContent;
            allPackages = PackageManager.GetPackages().ToList();

            await GetData();

            AddInstalledApps();
            AddRunningApps();
            AddAllApps();

            sortByNameMenuItem.IconPath = GetSortIcon(true);

            _ = UpdateSizeInfo();
        }

        private async Task GetData()
        {
            var installed = allPackages.Where(a => a.IsPreloaded == false && !string.IsNullOrEmpty(a.Label) && a.PackageType != PackageType.WGT).OrderBy(x => x.Label).ToList();
            var all = allPackages.Where(x => !string.IsNullOrEmpty(x.Label) && x.PackageType != PackageType.WGT).OrderBy(x => x.Label).ToList();
            var calculating = NUIGadgetResourceManager.GetString(nameof(Resources.IDS_SM_SBODY_CALCULATING_ING));

            foreach (var package in installed)
            {
                var iconPath = File.Exists(package.IconPath) ? package.IconPath : defaultIcon;
                var appInfo = new AppManager.ApplicationItemInfo(package.Id, package.Label, iconPath, calculating);
                installedAppsInfos.Add(appInfo);
            }

            foreach (var package in all)
            {
                var iconPath = File.Exists(package.IconPath) ? package.IconPath : defaultIcon;
                var appInfo = new AppManager.ApplicationItemInfo(package.Id, package.Label, iconPath, calculating);

                allAppsInfos.Add(appInfo);
            }

            var runningApplicationsContexts = await ApplicationManager.GetAllRunningApplicationsAsync();
            var applicationInfoList = new List<ApplicationInfo>();

            foreach (var application in runningApplicationsContexts)
            {
                applicationInfoList.Add(new ApplicationInfo(application.ApplicationId));
            }

            applicationInfoList = applicationInfoList.OrderBy(x => x.Label).ToList();

            foreach (var applicationInfo in applicationInfoList)
            {
                var iconPath = File.Exists(applicationInfo.IconPath) ? applicationInfo.IconPath : defaultIcon;
                var appInfo = new AppManager.ApplicationItemInfo(applicationInfo.ApplicationId, applicationInfo.Label, iconPath, calculating);
                appInfo.PackageId = applicationInfo.PackageId;

                runningAppsInfos.Add(appInfo);
            }
        }

        private void AddInstalledApps()
        {
            if (installedAppsInfos.Count() == 0)
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

                installedAppsContent.Add(noAppsLabel);
                installedAppsContent.Add(infoLabel);

                installedAppsIndicator?.Stop();
                installedAppsIndicator?.Unparent();
                installedAppsIndicator?.Dispose();

                return;
            }

            var layout = installedAppsContent.Layout as LinearLayout;
            layout.HorizontalAlignment = HorizontalAlignment.Begin;

            installedAppsView = CreateCollectionView();
            installedAppsView.ItemsSource = installedAppsInfos;
            installedAppsView.Relayout += StopLoading;

            installedAppsContent.Add(CreateAppSizeLabel());
            installedAppsContent.Add(installedAppsView);
        }

        private void AddRunningApps()
        {
            runningAppsView = CreateCollectionView();
            runningAppsView.ItemsSource = runningAppsInfos;
            runningAppsContent.Add(CreateAppSizeLabel());
            runningAppsContent.Add(runningAppsView);
        }

        private void AddAllApps()
        {
            allAppsView = CreateCollectionView();
            allAppsView.ItemsSource = allAppsInfos;
            allAppsContent.Add(CreateAppSizeLabel());
            allAppsContent.Add(allAppsView);
        }

        private async Task UpdateSizeInfo()
        {
            foreach (var package in allPackages)
            {
                var packageSizeInfo = await package.GetSizeInformationAsync();

                var installedAppInfo = installedAppsInfos.Where(a => a.AppId == package.Id).FirstOrDefault();
                if (installedAppInfo != null)
                {
                    installedAppInfo.AppSize = packageSizeInfo.AppSize;
                    installedAppInfo.SizeToDisplay = AppManager.GetSizeString(packageSizeInfo.AppSize);
                }                
 
                var allAppInfo = allAppsInfos.Where(a => a.AppId == package.Id).FirstOrDefault();
                if (allAppInfo != null)
                {
                    allAppInfo.AppSize = packageSizeInfo.AppSize;
                    allAppInfo.SizeToDisplay = AppManager.GetSizeString(packageSizeInfo.AppSize);
                }
            }

            UpdateRAMSizeInfo();

            if (sortBySizeMenuItem.Action is null)
            {
                sortBySizeMenuItem.Text = NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_SIZE));
                sortBySizeMenuItem.Action = () => { SortApplications(currentSortType != SortType.size_asc ? SortType.size_asc : SortType.size_desc); };
            }
        }

        private void UpdateRAMSizeInfo()
        {
            foreach (var info in runningAppsInfos)
            {
                var appContext = new ApplicationRunningContext(info.AppId);
                var processMemmory = new Tizen.System.ProcessMemoryUsage(new List<int> { appContext.ProcessId });
                processMemmory.Update(new List<int> { appContext.ProcessId });
                var vsz = processMemmory.GetVsz(appContext.ProcessId);

                info.AppSize = vsz;
                info.SizeToDisplay = AppManager.GetSizeString(vsz);
            }
        }

        private void SortApplications(SortType sortType)
        {
            ClearSortIcons();
            switch (sortType)
            {
                case SortType.size_asc:
                    installedAppsInfos = installedAppsInfos.OrderBy(x => x.AppSize).ToList();
                    runningAppsInfos = runningAppsInfos.OrderBy(x => x.AppSize).ToList();
                    allAppsInfos = allAppsInfos.OrderBy(x => x.AppSize).ToList();
                    sortBySizeMenuItem.IconPath = GetSortIcon(true);
                    break;
                case SortType.size_desc:
                    installedAppsInfos = installedAppsInfos.OrderByDescending(x => x.AppSize).ToList();
                    runningAppsInfos = runningAppsInfos.OrderByDescending(x => x.AppSize).ToList();
                    allAppsInfos = allAppsInfos.OrderByDescending(x => x.AppSize).ToList();
                    sortBySizeMenuItem.IconPath = GetSortIcon(false);
                    break;
                case SortType.name_asc:
                    installedAppsInfos = installedAppsInfos.OrderBy(x => x.Name).ToList();
                    runningAppsInfos = runningAppsInfos.OrderBy(x => x.Name).ToList();
                    allAppsInfos = allAppsInfos.OrderBy(x => x.Name).ToList();
                    sortByNameMenuItem.IconPath = GetSortIcon(true);
                    break;
                case SortType.name_desc:
                    installedAppsInfos = installedAppsInfos.OrderByDescending(x => x.Name).ToList();
                    runningAppsInfos = runningAppsInfos.OrderByDescending(x => x.Name).ToList();
                    allAppsInfos = allAppsInfos.OrderByDescending(x => x.Name).ToList();
                    sortByNameMenuItem.IconPath = GetSortIcon(false);
                    break;
            }

            currentSortType = sortType;

            // changing items source in post so that it does not block the closing of the more menu
            _ = Task.Run(async () =>
            {
                await CoreApplication.Post(() =>
                {
                    installedAppsView.ItemsSource = installedAppsInfos;
                    runningAppsView.ItemsSource = runningAppsInfos;
                    allAppsView.ItemsSource = allAppsInfos;
                    return true;
                });
            });
        }

        private void ClearSortIcons()
        {
            sortBySizeMenuItem.IconPath = string.Empty;
            sortByNameMenuItem.IconPath = string.Empty;
        }

        private string GetSortIcon(bool isAscending)
        {
            if (isAscending)
            {
                return IsLightTheme ? "more-menu/sort-ascending.svg" : "more-menu/dt-sort-ascending.svg";
            }

            return IsLightTheme ? "more-menu/sort-descending.svg" : "more-menu/dt-sort-descending.svg";
        }

        private void StopLoading(object sender, EventArgs e)
        {
            installedAppsView.Relayout -= StopLoading;

            installedAppsIndicator?.Stop();
            installedAppsIndicator?.Unparent();
            installedAppsIndicator?.Dispose();
        }

        private View TabView()
        {
            return new View()
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
                Layout = new LinearLayout()
                {
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                    VerticalAlignment = VerticalAlignment.Center,
                },
            };
        }

        private TextLabel CreateAppSizeLabel()
        {
            return new TextLabel
            {
                Text = NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_APP_SIZE)),
                TextColor = IsLightTheme ? new Color("#83868F") : new Color("#666666"),
                PixelSize = 24.SpToPx(),
                Margin = new Extents(20, 0, 16, 16).SpToPx(),
            };
        }

        private CollectionView CreateCollectionView()
        {
            var collectionView = new CollectionView()
            {
                ItemsLayouter = new LinearLayouter(),
                ItemTemplate = new DataTemplate(() =>
                {
                    CollectionViewItem item = new CollectionViewItem()
                    {
                        WidthSpecification = LayoutParamPolicies.MatchParent,
                    };

                    item.TextLabel.SetBinding(TextLabel.TextProperty, "Name");
                    item.Icon.SetBinding(ImageView.ResourceUrlProperty, "IconPath");
                    item.SubTextLabel.SetBinding(TextLabel.TextProperty, "SizeToDisplay");

                    item.Clicked += (s, e) =>
                    {
                        var context = item.BindingContext as AppManager.ApplicationItemInfo;
                        if (context != null)
                        {
                            var packageId = String.IsNullOrEmpty(context.PackageId) ? context.AppId : context.PackageId;
                            AppManager.CurrentApp = PackageManager.GetPackage(packageId);
                            NavigateTo(MainMenuProvider.Apps_AppInfo);
                        }
                    };

                    return item;
                }),
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
                ScrollingDirection = ScrollableBase.Direction.Vertical,
                SelectionMode = ItemSelectionMode.SingleAlways,
                BackgroundColor = Color.Transparent,
            };

            var calculating = NUIGadgetResourceManager.GetString(nameof(Resources.IDS_SM_SBODY_CALCULATING_ING));

            return collectionView;
        }

        private async void PackageManager_InstallProgressChanged(object sender, PackageManagerEventArgs e)
        {
            if (e.State == PackageEventState.Completed)
            {
                try
                {
                    var package = PackageManager.GetPackage(e.PackageId);

                    if(installedAppsInfos.Where(a => a.AppId == package.Id).FirstOrDefault() is null)
                    {
                        var packageSizeInfo = await package.GetSizeInformationAsync();
                        var size = AppManager.GetSizeString(packageSizeInfo.AppSize);

                        var iconPath = File.Exists(package.IconPath) ? package.IconPath : defaultIcon;
                        var appInfo = new AppManager.ApplicationItemInfo(package.Id, package.Label, iconPath, size);
                        appInfo.AppSize = packageSizeInfo.AppSize;

                        installedAppsInfos.Add(appInfo);
                        installedAppsView.ItemsSource = installedAppsInfos;

                        SortApplications(currentSortType);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn($"Updating apps gadget after installation failed: {ex.Message}");
                }
            }
        }

        private void PackageManager_UninstallProgressChanged(object sender, PackageManagerEventArgs e)
        {
            if (e.State == PackageEventState.Completed)
            {
                try
                {
                    // the completed status of the uninstall process comes twice, so this protects against a second update 
                    PackageManager.UninstallProgressChanged -= PackageManager_UninstallProgressChanged;

                    installedAppsInfos = installedAppsInfos.Where(a => a.AppId != e.PackageId).ToList();
                    installedAppsView.ItemsSource = installedAppsInfos;

                    PackageManager.UninstallProgressChanged += PackageManager_UninstallProgressChanged;
                }
                catch (Exception ex)
                {
                    Logger.Warn($"Updating apps gadget after uninstallation failed: {ex.Message}");
                }
            }
        }

        private enum SortType
        {
            size_asc,
            size_desc,
            name_asc,
            name_desc,
        }

        protected override void OnDestroy()
        {
            PackageManager.UninstallProgressChanged -= PackageManager_UninstallProgressChanged;
            PackageManager.InstallProgressChanged -= PackageManager_InstallProgressChanged;

            base.OnDestroy();
        }
    }
}
