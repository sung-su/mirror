using System.Collections.Generic;
using Tizen.NUI.Components;
using Tizen.NUI;
using Tizen.System;

namespace SettingCore
{
    internal static class GadgetNavigation
    {
        // keep page-gadget binding, to update Title & ContextMenu strings when language changed
        private static Dictionary<Page, MenuGadget> gadgetPages = new Dictionary<Page, MenuGadget>();

        static GadgetNavigation()
        {
            SystemSettings.LocaleLanguageChanged += (object sender, LocaleLanguageChangedEventArgs e) => {
                foreach (var pair in gadgetPages)
                {
                    if (pair.Key is ContentPage page)
                    {
                        var title = pair.Value.ProvideTitle();
                        page.AppBar.Title = title;
                    }
                }
            };
        }

        public static void NavigateBack()
        {
            var page = NUIApplication.GetDefaultWindow().GetDefaultNavigator().Pop();

            if (gadgetPages.TryGetValue(page, out MenuGadget gadget))
            {
                NUIGadgetManager.Remove(gadget);
                gadgetPages.Remove(page);
            }
        }

        public static void NavigateTo(string menuPath)
        {
            var info = GadgetManager.GetGadgetInfoFromPath(menuPath);
            if (info == null)
            {
                Logger.Warn($"could not find gadget for menupath: {menuPath}");
                return;
            }

            try
            {
                var gadget = NUIGadgetManager.Add(info.Pkg.ResourceType, info.ClassName) as MenuGadget;

                var title = gadget.ProvideTitle();
                var content = gadget.MainView;

                // TODO: remove style customization with scalable unit, when merged to TizenFX
                var appBarStyle = ThemeManager.GetStyle("Tizen.NUI.Components.AppBar") as AppBarStyle;
                appBarStyle.Size = new Size(-1, 64).SpToPx();
                appBarStyle.TitleTextLabel.PointSize = 18f.SpToPt();

                var backButton = new Button(appBarStyle.BackButton);
                backButton.Clicked += (s, e) => NavigateBack();

                var page = new ContentPage
                {
                    AppBar = new AppBar(appBarStyle)
                    {
                        Title = title,
                        NavigationContent = backButton,
                    },
                    Content = content,
                };

                var moreMenu = gadget.ProvideMoreMenu();
                if (moreMenu != null)
                {
                    var moreButton = GetMoreButton(moreMenu);
                    if (moreButton != null)
                    {
                        page.AppBar.Actions = new[] { moreButton };
                    }
                }

                NUIApplication.GetDefaultWindow().GetDefaultNavigator().Push(page);
                gadgetPages.Add(page, gadget);
            }
            catch (System.Exception e) // TODO: add separate catch blocks for specific exceptions?
            {
                Logger.Error($"could not create gadget for menu path: {menuPath} => {e}");
            }
        }

        private static Button GetMoreButton(IEnumerable<MoreMenuItem> moreMenu)
        {
            var moreButton = new Button
            {
                IconURL = System.IO.Path.Combine(Tizen.Applications.Application.Current.DirectoryInfo.Resource, "more-menu.svg"),
                Size = new Size(48, 48).SpToPx(),
            };
            moreButton.Clicked += (s, e) => {
                var menu = new Menu
                {
                    Anchor = moreButton,
                    HorizontalPositionToAnchor = Menu.RelativePosition.Start,
                    VerticalPositionToAnchor = Menu.RelativePosition.End,
                };

                var items = new List<MenuItem>();
                foreach (var moreMenuItem in moreMenu)
                {
                    var item = new MenuItem { Text = moreMenuItem.Text };
                    item.Clicked += (s, e) =>
                    {
                        moreMenuItem.Action?.Invoke();
                        menu.Dismiss();
                    };

                    items.Add(item);
                }
                menu.Items = items;
                menu.Post();
            };

            return moreButton;
        }
    }
}
