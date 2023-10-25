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
using Tizen.Context.AppHistory;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;

namespace Setting.Menu.Storage
{
    public class AppsStorageGadget : SettingCore.MenuGadget
    {
        public override string ProvideTitle() => NUIGadgetResourceManager.GetString(nameof(Resources.IDS_SM_TMBODY_APPS_ABB));
        public override IEnumerable<MoreMenuItem> ProvideMoreMenu() => MoreMenu();

        private string defaultIcon = System.IO.Path.Combine(Application.Current.DirectoryInfo.Resource, "default_app_icon.svg");

        private ScrollableBase content;
        private MoreMenuItem sortMenuItem;
        private PackageSizeInformation packageSizeInfo;

        private List<Package> packages = new List<Package>();
        private List<ApplicationInfo> applicationInfos = new List<ApplicationInfo>();

        private SortType currentSortType = SortType.name_asc;
        
        private List<MoreMenuItem> MoreMenu()
        {
            sortMenuItem = new MoreMenuItem()
            {
                Text = $"{NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_SIZE))}: {NUIGadgetResourceManager.GetString(nameof(Resources.IDS_SM_SBODY_CALCULATING_ING))}",
            };

            return new List<MoreMenuItem>
            {
                new MoreMenuItem()
                {
                    Text = NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_HEADER_SORT_BY)),
                },
                sortMenuItem,
                new MoreMenuItem()
                {
                    Text = NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_NAME)),
                    Action = () => { SortAppications(currentSortType != SortType.name_asc ? SortType.name_asc : SortType.name_desc); }
                },
                new MoreMenuItem()
                {
                    Text = "Frequency of use", // TODO : add translation to Resources
                    Action = () => { SortAppications(currentSortType != SortType.frequency_desc ? SortType.frequency_desc : SortType.frequency_asc); }
                }
            };
        }

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

            CreateItems();

            return content;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        private void CreateItems()
        {
            var usageStatistics = new UsageStatistics(UsageStatistics.SortOrderType.LastLaunchTimeNewest);
            var statistics = usageStatistics.Query(System.DateTime.Now.AddYears(-1), System.DateTime.Now);
            var listItems = new Dictionary<string, TextWithIconListItem>();

            packages = PackageManager.GetPackages().ToList();
            packages = packages.Where(a => a.InstalledStorageType == StorageType.Internal && !String.IsNullOrEmpty(a.Label) && a.PackageType != PackageType.WGT).OrderBy(x => x.Label).ToList();

            foreach (var package in packages)
            {
                var iconPath = File.Exists(package.IconPath) ? package.IconPath : defaultIcon;
                var appInfo = new ApplicationInfo(package.Id, package.Label, iconPath);

                // usage info 
                var appStatistics = statistics.FirstOrDefault(a => a.AppId == package.Id);
                if (appStatistics != null)
                {
                    appInfo.LastLaunchTime = appStatistics.LastLaunchTime;
                }

                var appItem = new TextWithIconListItem(appInfo.Name, Color.Transparent, iconPath: appInfo.IconPath, subText: NUIGadgetResourceManager.GetString(nameof(Resources.IDS_SM_SBODY_CALCULATING_ING)));
                // TODO : goto app info by AppId
                //appItem.Clicked += (s, e) => {};

                content.Add(appItem);
                listItems.Add(appInfo.AppId, appItem);
                applicationInfos.Add(appInfo);
            }

            _ = UpdateSizeInfo(listItems);
        }

        private void SortAppications(SortType sortType)
        {
            content.RemoveAllChildren(true);

            var listItems = new Dictionary<string, TextWithIconListItem>();

            switch (sortType)
            {
                case SortType.size_asc:
                    applicationInfos = applicationInfos.OrderBy(x => x.AppSize).ToList();
                    break;
                case SortType.size_desc:
                    applicationInfos = applicationInfos.OrderByDescending(x => x.AppSize).ToList();
                    break;
                case SortType.name_asc:
                    applicationInfos = applicationInfos.OrderBy(x => x.Name).ToList();
                    break;
                case SortType.name_desc:
                    applicationInfos = applicationInfos.OrderByDescending(x => x.Name).ToList();
                    break;
                case SortType.frequency_asc:
                    applicationInfos = applicationInfos.OrderBy(x => x.LastLaunchTime).ThenBy(a => a.Name).ToList();
                    break;
                case SortType.frequency_desc:
                    applicationInfos = applicationInfos.OrderByDescending(x => x.LastLaunchTime).ToList();
                    break;
            }

            foreach (var app in applicationInfos)
            {
                var item = new TextWithIconListItem(app.Name, Color.Transparent, iconPath: app.IconPath, subText: AppManager.GetSizeString(app.AppSize));
                content.Add(item);

                if (app.AppSize == 0)
                {
                    listItems.Add(app.AppId, item);
                }
            }

            if (listItems.Count > 0)
            {
                _ = UpdateSizeInfo(listItems);
            }

            currentSortType = sortType;
        }

        private async Task UpdateSizeInfo(Dictionary<string, TextWithIconListItem> items)
        {
            foreach (var item in items)
            {
                packageSizeInfo = await packages.FirstOrDefault(a => a.Id == item.Key).GetSizeInformationAsync();

                var app = applicationInfos.FirstOrDefault(a => a.AppId == item.Key);
                if (app != null)
                {
                    app.AppSize = packageSizeInfo.AppSize;
                    item.Value.SubText = AppManager.GetSizeString(packageSizeInfo.AppSize);
                }
            }

            if (sortMenuItem.Action is null)
            {
                sortMenuItem.Text = NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_SIZE));
                sortMenuItem.Action = () => { SortAppications(currentSortType != SortType.size_asc ? SortType.size_asc : SortType.size_desc); };
            }
        }

        private class ApplicationInfo
        {
            public string AppId { get; }
            public string Name { get; set; }
            public string IconPath { get; set; }
            public long AppSize { get; set; }
            public System.DateTime LastLaunchTime { get; set; }

            public ApplicationInfo(string appid, string name, string iconPath)
            {
                AppId = appid;
                Name = name;
                IconPath = iconPath;
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