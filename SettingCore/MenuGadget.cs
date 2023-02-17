using Tizen.NUI;

namespace SettingCore
{
    public abstract class MenuGadget : NUIGadget
    {
        protected MenuGadget(NUIGadgetType type = NUIGadgetType.Normal) : base(type)
        {
        }

        public abstract string ProvideTitle();

        protected void NavigateBack() => GadgetNavigation.NavigateBack();
        protected void NavigateTo(string fullClassName) => GadgetNavigation.NavigateTo(fullClassName);
    }
}
