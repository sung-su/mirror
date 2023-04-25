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
            MainMenuGadget mainMenu;
            try
            {
                mainMenu = NUIGadgetManager.Add(info.Pkg.ResourceType, info.ClassName) as MainMenuGadget;
            }
            catch (System.Exception e)
            {
                Logger.Warn($"could not create MainMenuGadget from {info.ClassName} at {assemblyPath}");
                Logger.Error(e.Message);
                return null;
            }
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
            NUIGadgetManager.Remove(mainMenu);

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
