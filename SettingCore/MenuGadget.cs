using System.Collections.Generic;
using System.Linq;
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
            GadgetManager.Instance.CustomizationChanged += CustomizationChanged;
            return base.OnCreate();
        }

        protected override void OnDestroy()
        {
            GadgetManager.Instance.CustomizationChanged -= CustomizationChanged;
            base.OnDestroy();
        }

        private void CustomizationChanged(object sender, CustomizationChangedEventArgs e)
        {
            string classname = GetType().FullName;

            List<MenuCustomizationItem> items = new List<MenuCustomizationItem>();
            foreach (var c in e.CustomizationItems)
            {
                if (GadgetManager.Instance.DoesMenuPathMatchClassName(c.MenuPath, classname))
                {
                    Logger.Debug($"Received customization for menupath: {c.MenuPath}, order {c.Order} at class: {classname}");
                    items.Add(c);
                }
            }

            if (items.Any())
            {
                OnCustomizationUpdate(items);
            }
        }

        protected virtual void OnCustomizationUpdate(IEnumerable<MenuCustomizationItem> items)
        {
        }

        protected IEnumerable<MenuCustomizationItem> GetCustomization()
        {
            string fullClassName = GetType().FullName;
            return GadgetManager.Instance.GetCustomization(fullClassName);
        }

        public abstract string ProvideTitle();

        public virtual IEnumerable<View> ProvideMoreActions() => null;
        public virtual IEnumerable<MoreMenuItem> ProvideMoreMenu() => null;

        protected void NavigateBack() => GadgetNavigation.NavigateBack();
        protected void NavigateTo(string menuPath) => GadgetNavigation.NavigateTo(menuPath);

        protected bool IsLightTheme => ThemeManager.PlatformThemeId == "org.tizen.default-light-theme";

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
