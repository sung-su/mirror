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
    }
}
