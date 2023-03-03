using SettingCore.Customization;
using System.Collections.Generic;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;

namespace SettingCore
{
    public abstract class MenuGadget : NUIGadget
    {
        protected MenuGadget(NUIGadgetType type = NUIGadgetType.Normal) : base(type)
        {
        }

        protected override View OnCreate()
        {
            GadgetManager.CustomizationChanged += CustomizationChanged;
            return base.OnCreate();
        }

        protected override void OnDestroy()
        {
            GadgetManager.CustomizationChanged -= CustomizationChanged;
            base.OnDestroy();
        }

        private void CustomizationChanged(object sender, Customization.CustomizationChangedEventArgs e)
        {
            string classname = GetType().FullName;

            if (GadgetManager.DoesMenuPathMatchClassName(e.MenuPath, classname))
            {
                Logger.Verbose($"Received customization for menupath: {e.MenuPath}, order {e.Order} at class: {classname}");
                OnCustomizationUpdate(new MenuCustomizationItem(e.MenuPath, e.Order));
            }
        }

        protected virtual void OnCustomizationUpdate(MenuCustomizationItem item)
        {
        }

        protected IEnumerable<MenuCustomizationItem> GetCustomization()
        {
            string fullClassName = GetType().FullName;
            return GadgetManager.GetCustomization(fullClassName);
        }

        public abstract string ProvideTitle();

        public virtual IEnumerable<MoreMenuItem> ProvideMoreMenu() => null;

        protected void NavigateBack() => GadgetNavigation.NavigateBack();
        protected void NavigateTo(string menuPath) => GadgetNavigation.NavigateTo(menuPath);

        protected string GetResourcePath(string relativeFilePath)
        {
            string callingAssemblyName = System.Reflection.Assembly.GetCallingAssembly().GetName().Name;
            string absoluteDirPath = System.IO.Path.Combine(Tizen.Applications.Application.Current.DirectoryInfo.Resource, "mount/allowed/", callingAssemblyName);

            // remove leading slash
            relativeFilePath = relativeFilePath.TrimStart('/');

            return System.IO.Path.Combine(absoluteDirPath, relativeFilePath);
        }
    }
}
