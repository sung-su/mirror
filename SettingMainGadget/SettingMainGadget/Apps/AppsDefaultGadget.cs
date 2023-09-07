using SettingCore;
using SettingCore.Views;
using SettingMainGadget;
using SettingMainGadget.Apps;
using SettingMainGadget.TextResources;
using System;
using System.Collections.Generic;
using System.Linq;
using Tizen;
using Tizen.Applications;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;

namespace Setting.Menu.Apps
{
    public class AppsDefaultGadget : MenuGadget
    {
        public override string ProvideTitle() => NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_HEADER_DEFAULT_APPLICATIONS_ABB));

        private const string vconf_menuscreen = "db/setting/menuscreen/package_name";

        private View content;

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

            CreateView();

            return content;
        }

        private void CreateView()
        {
            var launchByDefault = new TextHeaderListItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_LAUNCH_BY_DEFAULT)));
            content.Add(launchByDefault);

            var defaultAppsView = new View()
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                Layout = new LinearLayout()
                {
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                },
            };

            // add default home screen app

            if (Vconf.TryGetString(vconf_menuscreen, out string defaultHomeScreenApp))
            {
                Logger.Warn($"Could not get vconf value: {vconf_menuscreen}");
            }

            if (defaultHomeScreenApp != "")
            {
                var homeScreenAppInfo = ApplicationManager.GetInstalledApplication(defaultHomeScreenApp);

                var homeScreenItem = TextListItem.CreatePrimaryTextItemWithSecondaryText(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_HOME)), homeScreenAppInfo.Label);

                var homeScreenApps = AppManager.GetApplicationsInfoByCategory(homeScreenAppInfo.Categories.FirstOrDefault());
                if (homeScreenApps.Count > 1)
                {
                    homeScreenItem.Clicked += (o, e) =>
                    {
                        NavigateTo(MainMenuProvider.Apps_DefaultHome);
                    };
                }

                defaultAppsView.Add(homeScreenItem);
            }

            // add default apps

            var defaultAppsId = AppControl.GetDefaultApplicationIds();

            foreach (var id in defaultAppsId)
            {
                var defaultAppInfo = new ApplicationInfo(id);
                var defaultApp = TextListItem.CreatePrimaryTextItemWithSecondaryText(defaultAppInfo.Label, defaultAppInfo.PackageId);
                defaultAppsView.Add(defaultApp);
            }

            content.Add(defaultAppsView);

            var clearDefaults = new TextHeaderListItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_CLEAR_DEFAULTS)));
            // TODO : open AppInfo
            content.Add(clearDefaults);

            if (defaultAppsId.Count() == 0)
            {
                var noApps = new TextLabel()
                {
                    Text = NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_THERE_ARE_NO_APPS_SET_AS_DEFAULTS)),
                    TextColor = ThemeManager.PlatformThemeId == "org.tizen.default-light-theme" ? new Color("#CACACA") : new Color("#83868F"),
                    PixelSize = 24.SpToPx(),
                    Margin = new Extents(16, 16, 16, 16).SpToPx(),
                };
                content.Add(noApps);
            }
        }
    }
}
