using Tizen.NUI;

namespace SettingCore
{
    public abstract class MainMenuGadget : MenuGadget
    {
        protected MainMenuGadget(NUIGadgetType type = NUIGadgetType.Normal) : base(type) { }

        public abstract string ProvideIconPath();
        public abstract Color ProvideIconColor();
    }
}
