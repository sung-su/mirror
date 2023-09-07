using SettingCore;
using SettingCore.Views;
using SettingMainGadget.Apps;
using SettingMainGadget.TextResources;
using Tizen;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;

namespace Setting.Menu.Apps
{
    public class AppsDefaultHomeGadget : MenuGadget
    {
        public override string ProvideTitle() => NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_HEADER_HOME_SCREEN_ABB));

        private const string homeAppCategory = "http://tizen.org/category/homeapp";
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

            var homeScreenApps = AppManager.GetApplicationsInfoByCategory(homeAppCategory);

            if (homeScreenApps.Count > 0)
            {
                var defaultHomeScreenApp = "";
                if (Vconf.TryGetString(vconf_menuscreen, out defaultHomeScreenApp))
                {
                    Logger.Warn($"Could not get vconf value: {vconf_menuscreen}");
                }

                RadioButtonGroup radioButtonGroup = new RadioButtonGroup();

                for (int i = 0; i < homeScreenApps.Count; i++)
                {
                    RadioButtonListItem item = new RadioButtonListItem(homeScreenApps[i].Label);
                    item.RadioButton.IsSelected = homeScreenApps[i].ApplicationId == defaultHomeScreenApp;

                    radioButtonGroup.Add(item.RadioButton);
                    content.Add(item);
                }

                radioButtonGroup.SelectedChanged += (o, e) =>
                {
                    Vconf.SetString(vconf_menuscreen, homeScreenApps[radioButtonGroup.SelectedIndex].ApplicationId);
                };
            }

            return content;
        }
    }
}
