using System.Collections.Generic;
using Tizen.NUI.Components;
using Tizen.NUI;
using Tizen.System;
using Tizen.NUI.BaseComponents;
using System.Linq;
using System.Threading;
using SettingCore.Views;
using System;

namespace SettingCore
{
    public static class GadgetNavigation
    {
        private static bool IsLightTheme => ThemeManager.PlatformThemeId == "org.tizen.default-light-theme";

        // keep page-gadget binding, to update Title & ContextMenu strings when language changed
        private static Dictionary<Page, MenuGadget> gadgetPages = new Dictionary<Page, MenuGadget>();

        public static event EventHandler<bool> OnWindowModeChanged;

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
                NUIApplication.GetDefaultWindow().GetDefaultNavigator().EnableBackNavigation = true;
            };
        }

        public static void SetFullScreenMode(bool fullScreen)
        {
            OnWindowModeChanged?.Invoke(null, fullScreen);
        }

        public static void NavigateBack()
        {
            var baseContentPage = NUIApplication.GetDefaultWindow().GetDefaultNavigator().Peek();

            if (baseContentPage is BaseContentPage)
            {
                NUIApplication.GetDefaultWindow().GetDefaultNavigator().Pop();
            }
            else
            {
                SetFullScreenMode(false);

                try
                {
                    // CreateTransitions issue
                    NUIApplication.GetDefaultWindow().GetDefaultNavigator().PopWithTransition();
                }
                catch (Exception ex)
                {
                    Logger.Warn($"{ex.Message}");
                }
            }
        }

        public static void NavigateTo(string menuPath)
        {
            try
            {
                semaphore.Wait();

                var info = GadgetManager.Instance.GetGadgetInfoFromPath(menuPath);
                if (info == null)
                {
                    Logger.Warn($"could not find gadget for menupath: {menuPath}");
                    return;
                }

                if (gadgetPages.Values.Select(x => x.ClassName).Contains(info.ClassName))
                {
                    Logger.Verbose($"Already navigated to menupath: {menuPath}");
                    return;
                }

                var gadget = NUIGadgetManager.Add(info.Pkg.ResourceType, info.ClassName) as MenuGadget;
                var title = gadget.ProvideTitle();
                var content = gadget.MainView;

                if (info.IsFullScreenMode)
                {
                    SetFullScreenMode(true);

                    var contentPage = new ContentPage
                    {
                        CornerRadius = 0.SpToPx(),
                        Content = content,
                        ThemeChangeSensitive = true,
                    };

                    try
                    {
                        // CreateTransitions issue
                        NUIApplication.GetDefaultWindow().GetDefaultNavigator().PushWithTransition(contentPage);
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn($"{ex.Message}");
                    }

                    gadgetPages.Add(contentPage, gadget);
                    return;
                }

                // TODO: remove style customization with scalable unit, when merged to TizenFX
                var appBarStyle = ThemeManager.GetStyle("Tizen.NUI.Components.AppBar") as AppBarStyle;
                appBarStyle.TitleTextLabel.PixelSize = 24f.SpToPx();

                var backButton = new BackButton();
                backButton.Margin = new Extents(0, 8, 0, 0).SpToPx();
                backButton.Clicked += (s, e) => NavigateBack();

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
                        Actions = moreItems,
                    },
                    Content = content,
                    ThemeChangeSensitive = true,
                };

                page.Appeared += (s, e) =>
                {
                    gadget.OnPageAppeared?.Invoke();
                };

                NUIApplication.GetDefaultWindow().GetDefaultNavigator().Push(page);
                gadgetPages.Add(page, gadget);
            }
            catch (Exception e)
            {
                Logger.Error($"could not create gadget for menu path: {menuPath} => {e}");
            }
            finally
            {
                semaphore.Release();
            }
        }

        private static Control GetMoreButton(IEnumerable<MoreMenuItem> moreMenu)
        {
            if (moreMenu == null)
            {
                return null;
            }

            var moreButton = new BaseComponent
            {
                Size = new Size(48, 48).SpToPx(),
                Layout = new LinearLayout
                {
                    LinearOrientation = LinearLayout.Orientation.Horizontal,
                    VerticalAlignment = VerticalAlignment.Center,
                },
            };

            var iconVisual = new ImageVisual
            {
                MixColor = IsLightTheme ? new Color("#17234D") : new Color("#FDFDFD"),
                URL = System.IO.Path.Combine(Tizen.Applications.Application.Current.DirectoryInfo.Resource, "more-menu.svg"),
                FittingMode = FittingModeType.ScaleToFill,
            };
            var icon = new ImageView
            {
                Image = iconVisual.OutputVisualMap,
                Size = new Size(48, 48).SpToPx(),
            };

            moreButton.Add(icon);

            var menuStyle = ThemeManager.GetStyle("Tizen.NUI.Components.Menu") as MenuStyle;
            menuStyle.Content.BackgroundColor = IsLightTheme ? new Color("#FAFAFA") : new Color("#1D1A21");
            menuStyle.Content.CornerRadius = 12.SpToPx();

            var menuItemStyle = ThemeManager.GetStyle("Tizen.NUI.Components.MenuItem") as ButtonStyle;
            menuItemStyle.BackgroundColor = IsLightTheme ? new Color("#FAFAFA") : new Color("#1D1A21");
            menuItemStyle.Size = new Size(324, 64).SpToPx();

            moreButton.Clicked += (s, e) =>
            {
                iconVisual.MixColor = IsLightTheme ? new Color("#FFA166") : new Color("#FF8A00");
                icon.Image = iconVisual.OutputVisualMap;

                var menu = new Menu(menuStyle)
                {
                    Anchor = moreButton,
                    HorizontalPositionToAnchor = Menu.RelativePosition.Center,
                    VerticalPositionToAnchor = Menu.RelativePosition.End,
                };

                menu.RemovedFromWindow += (s, e) =>
                {
                    iconVisual.MixColor = IsLightTheme ? new Color("#17234D") : new Color("#FDFDFD");
                    icon.Image = iconVisual.OutputVisualMap;
                };

                NUIApplication.GetDefaultWindow().GetDefaultNavigator().Popped += (s, e) =>
                {
                    if (menu != null)
                    {
                        menu.Dismiss();
                    }
                };

                NUIApplication.GetDefaultWindow().Resized += (s, e) =>
                {
                    if (menu != null)
                    {
                        menu.Dismiss();
                    }
                };

                var items = new List<MenuItem>();
                foreach (var moreMenuItem in moreMenu)
                {
                    var item = new MenuItem(menuItemStyle)
                    {
                        Text = moreMenuItem.Text,
                        TextColor = IsLightTheme ? new Color("#090E21") : new Color("#FDFDFD"),
                    };

                    if (!String.IsNullOrEmpty(moreMenuItem.IconPath))
                    {
                        item.Layout = new FlexLayout()
                        {
                            Justification = FlexLayout.FlexJustification.SpaceBetween,
                            Direction = FlexLayout.FlexDirection.Row,
                            ItemsAlignment = FlexLayout.AlignmentType.Center
                        };
   
                        // remove buttons icon
                        item.Remove(item.Children[0]);

                        var itemIcon = new ImageView
                        {
                            ResourceUrl = System.IO.Path.Combine(Tizen.Applications.Application.Current.DirectoryInfo.Resource, moreMenuItem.IconPath),
                            Size = new Size(32, 32).SpToPx(),
                        };

                        item.Add(itemIcon);
                    }

                    if (moreMenuItem.Action is null)
                    {
                        item.TextColor = IsLightTheme ? new Color("#83868F") : new Color("#666666");
                    }
                    else
                    {
                        item.TouchEvent += (s, e) =>
                        {
                            var state = e.Touch.GetState(0);
                            if (state == PointStateType.Down)
                            {
                                item.TextColor = IsLightTheme ? new Color("#FF6200") : new Color("#FF8A00");
                            }
                            return false;
                        };

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
