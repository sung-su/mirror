using System;
using System.Collections.Generic;
using Tizen.NUI;

namespace SettingCore
{
    public class MainMenuInfo
    {
        public string IconPath { get; private set; }
        public Color IconColor { get; private set; }
        public string Title { get; private set; }
        public Action Action { get; private set; }

        public static IEnumerable<MainMenuInfo> GetReal()
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            var list = new List<MainMenuInfo>();
            var infos = GadgetManager.GetMain();
            foreach (var info in infos)
            {
                NUIGadget gadget;
                try
                {
                    gadget = NUIGadgetManager.Add(info.Pkg.ResourceType, info.ClassName);
                }
                catch
                {
                    Logger.Warn($"real main menu - could not create gadget with ClassName {info.ClassName} and ResourceType {info.Pkg.ResourceType}");
                    continue;
                }

                if (gadget is MainMenuGadget mainMenuGadget)
                {
                    string iconPath = mainMenuGadget.ProvideIconPath();
                    Color iconColor = mainMenuGadget.ProvideIconColor();
                    string title = mainMenuGadget.ProvideTitle();
                    Action action = () =>
                    {
                        Logger.Debug($"navigating to {info.ClassName}, title: {title}");
                        GadgetNavigation.NavigateTo(info.ClassName);
                    };

                    // FIXME: currently menus return relative icon path, so here we make it absolute
                    iconPath = System.IO.Path.Combine(Tizen.Applications.Application.Current.DirectoryInfo.Resource, iconPath);

                    Logger.Debug($"real main menu ({info.ClassName}) title: {title}, icon color: {iconColor.ToHex()}, icon path: {iconPath}");
                    list.Add(new MainMenuInfo
                    {
                        IconPath = iconPath,
                        IconColor = iconColor,
                        Title = title,
                        Action = action,
                    });
                }
                else
                {
                    Logger.Warn($"real main menu - {info.ClassName} is not MainMenuGadget");
                }

                NUIGadgetManager.Remove(gadget);
            }

            stopwatch.Stop();
            Logger.Debug($"real main menu gadgets scanned in {stopwatch.Elapsed}");

            return list;
        }
    }
}
