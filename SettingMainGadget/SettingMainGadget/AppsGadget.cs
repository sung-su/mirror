using SettingCore;
using SettingCore.Views;
using SettingMainGadget;
using SettingMainGadget.TextResources;
using System.Collections.Generic;
using System.Linq;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;

namespace Setting.Menu
{
    internal class AppsGadget : MainMenuGadget
    {
        public override Color ProvideIconColor() => new Color(IsLightTheme ? "#7F2982" : "#922F95");

        public override string ProvideIconPath() => GetResourcePath("apps.svg");

        public override string ProvideTitle() => NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_APPLICATIONS));

        private ScrollableBase content;
        private Sections sections = new Sections();

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

            CreateView();

            return content;
        }

        private void CreateView()
        {
            content.RemoveAllChildren(true);
            sections.RemoveAllSectionsFromView(content);

            var appsManagerItem = TextListItem.CreatePrimaryTextItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_APPLICATION_MANAGER)));
            sections.Add(MainMenuProvider.Apps_AppsManager, appsManagerItem);
            appsManagerItem.Clicked += (o, e) =>
            {
                NavigateTo(MainMenuProvider.Apps_AppsManager);
            };
            content.Add(appsManagerItem);

            var defaultAppsItem = TextListItem.CreatePrimaryTextItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_HEADER_DEFAULT_APPLICATIONS_ABB)));
            sections.Add(MainMenuProvider.Apps_DefaultApps, defaultAppsItem);
            defaultAppsItem.Clicked += (o, e) =>
            {
                NavigateTo(MainMenuProvider.Apps_DefaultApps);
            };
            content.Add(defaultAppsItem);
        }
        protected override void OnCustomizationUpdate(IEnumerable<MenuCustomizationItem> items)
        {
            Logger.Verbose($"{nameof(AppsGadget)} got customization with {items.Count()} items. Recreating view.");
            CreateView();
        }

    }
}
