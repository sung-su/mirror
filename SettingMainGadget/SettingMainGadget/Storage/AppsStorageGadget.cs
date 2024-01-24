using SettingCore;
using SettingCore.Views;
using SettingMainGadget.Apps;
using SettingMainGadget.TextResources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Tizen.Applications;
using Tizen.Context.AppHistory;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Binding;
using Tizen.NUI.Components;

namespace Setting.Menu.Storage
{
    public class AppsStorageGadget : SettingCore.MenuGadget
    {
        public override string ProvideTitle() => NUIGadgetResourceManager.GetString(nameof(Resources.IDS_SM_TMBODY_APPS_ABB));
        public override IEnumerable<MoreMenuItem> ProvideMoreMenu() => MoreMenu();

        private string defaultIcon = System.IO.Path.Combine(Application.Current.DirectoryInfo.Resource, "default_app_icon.svg");

        private View content;
        private MoreMenuItem sortBySizeMenuItem;
        private MoreMenuItem sortByNameMenuItem;
        private MoreMenuItem sortByUseMenuItem;
        private CollectionView collectionView;
        private Loading appsLoading;

        private List<Package> packages = new List<Package>();
        private List<ApplicationInfo> applicationInfos = new List<ApplicationInfo>();
        private List<UsageStatisticsData> usageStatisticsDatas = new List<UsageStatisticsData>();

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
                Action = () => { SortAppications(currentSortType != SortType.name_asc ? SortType.name_asc : SortType.name_desc); }
            };

            sortByUseMenuItem = new MoreMenuItem()
            {
                Text = NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_FREQUENCY_OF_USE)),
                Action = () => { SortAppications(currentSortType != SortType.frequency_desc ? SortType.frequency_desc : SortType.frequency_asc); }
            };

            return new List<MoreMenuItem>
            {
                new MoreMenuItem()
                {
                    Text = NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_HEADER_SORT_BY)),
                },
                sortBySizeMenuItem,
                sortByNameMenuItem,
                sortByUseMenuItem,
            };
        }

        protected override View OnCreate()
        {
            base.OnCreate();

            content = new View()
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

            appsLoading = new Loading();
            appsLoading.Play();
            content.Add(appsLoading);

            OnPageAppeared += CreateView;

            return content;
        }

        private void GetData()
        {
            var usageStatistics = new UsageStatistics(UsageStatistics.SortOrderType.LastLaunchTimeNewest);
            usageStatisticsDatas = usageStatistics.Query(System.DateTime.Now.AddYears(-1), System.DateTime.Now).ToList();

            packages = PackageManager.GetPackages().ToList();
            packages = packages.Where(a => a.InstalledStorageType == StorageType.Internal && !String.IsNullOrEmpty(a.Label) && a.PackageType != PackageType.WGT).OrderBy(x => x.Label).ToList();
        }

        private void CreateView()
        {
            GetData();

            collectionView = new CollectionView()
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

                    return item;
                }),
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
                ScrollingDirection = ScrollableBase.Direction.Vertical,
                SelectionMode = ItemSelectionMode.SingleAlways,
                BackgroundColor = Color.Transparent,
            };

            var calculating = NUIGadgetResourceManager.GetString(nameof(Resources.IDS_SM_SBODY_CALCULATING_ING));

            foreach (var package in packages)
            {
                var iconPath = File.Exists(package.IconPath) ? package.IconPath : defaultIcon;
                var appInfo = new ApplicationInfo(package.Id, package.Label, iconPath, calculating);

                // usage info 
                var appStatistics = usageStatisticsDatas.FirstOrDefault(a => a.AppId == package.Id);
                if (appStatistics != null)
                {
                    appInfo.LastLaunchTime = appStatistics.LastLaunchTime;
                }

                applicationInfos.Add(appInfo);
            }
            collectionView.ItemsSource = applicationInfos;
            collectionView.Relayout += StopLoading;
            content.Add(collectionView);

            sortByNameMenuItem.IconPath = GetSortIcon(true);
            _ = UpdateSizeInfo();
        }

        private void StopLoading(object sender, EventArgs e)
        {
            collectionView.Relayout -= StopLoading;
            if (appsLoading != null)
            {
                appsLoading?.Stop();
                appsLoading?.Unparent();
                appsLoading?.Dispose();
            }
        }

        private async Task UpdateSizeInfo()
        {
            foreach (var package in packages)
            {
                var packageSizeInfo = await package.GetSizeInformationAsync();
                var appInfo = applicationInfos.Where(a => a.AppId == package.Id).FirstOrDefault();
                if (appInfo != null)
                {
                    appInfo.AppSize = packageSizeInfo.AppSize;
                    appInfo.SizeToDisplay = AppManager.GetSizeString(packageSizeInfo.AppSize);
                }
            }

            if (sortBySizeMenuItem.Action is null)
            {
                sortBySizeMenuItem.Text = NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_SIZE));
                sortBySizeMenuItem.Action = () => { SortAppications(currentSortType != SortType.size_asc ? SortType.size_asc : SortType.size_desc); };
            }
        }

        private void SortAppications(SortType sortType)
        {
            ClearSortIcons();
            switch (sortType)
            {
                case SortType.size_asc:
                    applicationInfos = applicationInfos.OrderBy(x => x.AppSize).ToList();
                    sortBySizeMenuItem.IconPath = GetSortIcon(true);
                    break;
                case SortType.size_desc:
                    applicationInfos = applicationInfos.OrderByDescending(x => x.AppSize).ToList();
                    sortBySizeMenuItem.IconPath = GetSortIcon(false);
                    break;
                case SortType.name_asc:
                    applicationInfos = applicationInfos.OrderBy(x => x.Name).ToList();
                    sortByNameMenuItem.IconPath = GetSortIcon(true);
                    break;
                case SortType.name_desc:
                    applicationInfos = applicationInfos.OrderByDescending(x => x.Name).ToList();
                    sortByNameMenuItem.IconPath = GetSortIcon(false);
                    break;
                case SortType.frequency_asc:
                    applicationInfos = applicationInfos.OrderBy(x => x.LastLaunchTime).ThenBy(a => a.Name).ToList();
                    sortByUseMenuItem.IconPath = GetSortIcon(true);
                    break;
                case SortType.frequency_desc:
                    applicationInfos = applicationInfos.OrderByDescending(x => x.LastLaunchTime).ToList();
                    sortByUseMenuItem.IconPath = GetSortIcon(false);
                    break;
            }

            currentSortType = sortType;

            // changing items source in post so that it does not block the closing of the more menu
            _ = Task.Run(async () =>
            {
                await CoreApplication.Post(() =>
                {
                    collectionView.ItemsSource = applicationInfos;
                    return true;
                });
            });
        }

        private void ClearSortIcons()
        {
            sortBySizeMenuItem.IconPath = string.Empty;
            sortByNameMenuItem.IconPath = string.Empty;
            sortByUseMenuItem.IconPath = string.Empty;
        }

        private string GetSortIcon(bool isAscending)
        {
            if (isAscending)
            {
                return IsLightTheme ? "more-menu/sort-ascending.svg" : "more-menu/dt-sort-ascending.svg";
            }

            return IsLightTheme ? "more-menu/sort-descending.svg" : "more-menu/dt-sort-descending.svg";
        }

        private class ApplicationInfo : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public string AppId { get; }
            public string Name { get; set; }
            public string IconPath { get; set; }
            public long AppSize { get; set; }
            public System.DateTime LastLaunchTime { get; set; }

            private string size;
            public string SizeToDisplay
            {
                get => size;
                set
                {
                    if (value != size)
                    {
                        size = value;
                        RaisePropertyChanged(nameof(SizeToDisplay));
                    }
                }
            }

            public ApplicationInfo(string appid, string name, string iconPath, string size)
            {
                AppId = appid;
                Name = name;
                IconPath = iconPath;
                SizeToDisplay = size;
            }

            /// <summary>
            /// Raises PropertyChanged event.
            /// </summary>
            protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private enum SortType
        {
            size_asc,
            size_desc,
            name_asc,
            name_desc,
            frequency_asc,
            frequency_desc,
        }
    }
}