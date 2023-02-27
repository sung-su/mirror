using System;
using System.Collections.Generic;
using System.Reflection;
using Tizen.NUI;

namespace SettingCore
{
    public class MainMenuInfo
    {
        public string IconPath { get; private set; }
        public Color IconColor { get; private set; }
        public string Title { get; private set; }
        public Action Action { get; private set; }

        public static MainMenuInfo Create(SettingGadgetInfo info)
        {
            string assemblyPath = System.IO.Path.Combine(info.Pkg.ResourcePath, info.Pkg.ExecutableFile);
            System.Reflection.Assembly assembly = null;
            try
            {
                assembly = Assembly.Load(System.IO.File.ReadAllBytes(assemblyPath));
            }
            catch (System.IO.FileLoadException)
            {
                Logger.Warn($"could not open assembly {assemblyPath}");
                return null;
            }

            var mainMenu = assembly.CreateInstance(info.ClassName, true) as MainMenuGadget;
            if (mainMenu == null)
            {
                Logger.Warn($"could not create MainMenuGadget from {info.ClassName} at {assemblyPath}");
                return null;
            }

            string iconPath = mainMenu.ProvideIconPath();
            Color iconColor = mainMenu.ProvideIconColor();
            string title = mainMenu.ProvideTitle();
            Action action = () =>
            {
                Logger.Debug($"navigating to menupath {info.Path}, title: {title}");
                GadgetNavigation.NavigateTo(info.Path);
            };

            // FIXME: currently menus return relative icon path, so here we make it absolute
            iconPath = System.IO.Path.Combine(Tizen.Applications.Application.Current.DirectoryInfo.Resource, iconPath);

            return new MainMenuInfo
            {
                IconPath = iconPath,
                IconColor = iconColor,
                Title = title,
                Action = action,
            };
        }
    }
}
