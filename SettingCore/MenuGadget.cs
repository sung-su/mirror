using System.Collections.Generic;
using Tizen.NUI;

namespace SettingCore
{
    public abstract class MenuGadget : NUIGadget
    {
        protected MenuGadget(NUIGadgetType type = NUIGadgetType.Normal) : base(type)
        {
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
