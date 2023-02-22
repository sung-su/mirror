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

        public static void NavigateTo(string fullClassName)
        {
            foreach (var info in GadgetManager.GetAll())
            {
                if (info.ClassName == fullClassName)
                {
                    Logger.Debug($"found classname {fullClassName}");
                    try
                    {
                        var gadget = NUIGadgetManager.Add(info.Pkg.ResourceType, info.ClassName) as MenuGadget;

                        var title = gadget.ProvideTitle();
                        var content = gadget.MainView;

                        var page = new ContentPage
                        {
                            AppBar = new AppBar
                            {
                                Title = title,
                                // TODO: use ActionContent here to set button, which opens context menu (if required)
                            },
                            Content = content,
                        };

                        NUIApplication.GetDefaultWindow().GetDefaultNavigator().Push(page);
                        gadgetPages.Add(page, gadget);
                    }
                    catch (System.Exception e) // TODO: add separate catch blocks for specific exceptions?
                    {
                        Logger.Error($"could not create gadget {e}");
                    }
                    return;
                }
            }
            Logger.Warn($"could not find classname {fullClassName}");
        }
    }
}
