using System.Collections.Generic;
using Tizen.NUI.Components;
using Tizen.NUI;
using Tizen.System;
using Tizen.NUI.BaseComponents;
using System.Linq;
using System.Threading;
using SettingCore.Views;

namespace SettingCore
{
    public static class GadgetNavigation
    {
        // keep page-gadget binding, to update Title & ContextMenu strings when language changed
        private static Dictionary<Page, MenuGadget> gadgetPages = new Dictionary<Page, MenuGadget>();

        private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1);

        static GadgetNavigation()
        {
            SystemSettings.LocaleLanguageChanged += (object sender, LocaleLanguageChangedEventArgs e) => {
                foreach (var pair in gadgetPages)
                {
                    if (pair.Key is ContentPage page)
                    {
                        var title = pair.Value.ProvideTitle();

                        if (page.AppBar != null)
                        {
                            page.AppBar.Title = title;
                        }
                    }
                }
            };

            NUIApplication.GetDefaultWindow().GetDefaultNavigator().Popped += (s, e) =>
            {
                if (gadgetPages.TryGetValue(e.Page, out MenuGadget gadget))
                {
                    NUIGadgetManager.Remove(gadget);
                    gadgetPages.Remove(e.Page);
                }
            };
        }

        public static void NavigateBack()
        {
            NUIApplication.GetDefaultWindow().GetDefaultNavigator().Pop();
        }

        public static void NavigateTo(string menuPath)
        {
            semaphore.Wait();

            var info = GadgetManager.Instance.GetGadgetInfoFromPath(menuPath);
            if (info == null)
            {
                Logger.Warn($"could not find gadget for menupath: {menuPath}");
                semaphore.Release();
                return;
            }

            if (gadgetPages.Values.Select(x => x.ClassName).Contains(info.ClassName))
            {
                Logger.Verbose($"Already navigated to menupath: {menuPath}");
                semaphore.Release();
                return;
            }

            try
            {
                var gadget = NUIGadgetManager.Add(info.Pkg.ResourceType, info.ClassName) as MenuGadget;

                var title = gadget.ProvideTitle();
                var content = gadget.MainView;

                // TODO: remove style customization with scalable unit, when merged to TizenFX
                var appBarStyle = ThemeManager.GetStyle("Tizen.NUI.Components.AppBar") as AppBarStyle;
                appBarStyle.TitleTextLabel.PixelSize = 24f.SpToPx();

                var backButton = new Views.BackButton();
                backButton.Margin = new Extents(0, 8, 0, 0).SpToPx();
                backButton.Clicked += (s, e) => NavigateBack();

                var page = new BaseContentPage
                {
                    // TODO: CornerRadius depends on SettingViewBorder.CornerRadius - SettingViewBorder.BorderLineThickness, which is defined at SettingView project.
                    CornerRadius = (26.0f - 6.0f).SpToPx(),
                    ClippingMode = ClippingModeType.ClipChildren,
                    AppBar = new AppBar(appBarStyle)
                    {
                        Size = new Size(-1, 64).SpToPx(),
                        Title = title,
                        NavigationContent = backButton,
                        ThemeChangeSensitive = true,
                    },
                    Content = content,
                    ThemeChangeSensitive = true,
                };

                var moreItems = new List<View>();

                var moreActions = gadget.ProvideMoreActions();
                if (moreActions != null)
                {
                    moreItems.AddRange(moreActions);
                }

                var moreButton = GetMoreButton(gadget.ProvideMoreMenu());
                if (moreButton != null)
                {
                    moreItems.Add(moreButton);
                }

                page.AppBar.Actions = moreItems;

                NUIApplication.GetDefaultWindow().GetDefaultNavigator().Push(page);
                gadgetPages.Add(page, gadget);
            }
            catch (System.Exception e) // TODO: add separate catch blocks for specific exceptions?
            {
                Logger.Error($"could not create gadget for menu path: {menuPath} => {e}");
            }

            semaphore.Release();
        }

        private static Button GetMoreButton(IEnumerable<MoreMenuItem> moreMenu)
        {
            if (moreMenu == null)
            {
                return null;
            }

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

                NUIApplication.GetDefaultWindow().GetDefaultNavigator().Popped += (s, e) =>
                {
                    if (menu != null)
                    {
                        menu.Dismiss();
                    }
                };

                var items = new List<MenuItem>();
                foreach (var moreMenuItem in moreMenu)
                {
                    var item = new MenuItem { Text = moreMenuItem.Text };
                    if (moreMenuItem.Action is null)
                    {
                        item.TextColor = new Color("#83868F");
                    }
                    else
                    {
                        item.Clicked += (s, e) =>
                        {
                            moreMenuItem.Action?.Invoke();
                            menu.Dismiss();
                        };
                    }

                    items.Add(item);
                }
                menu.Items = items;
                menu.Post();
            };

            return moreButton;
        }
    }
}
