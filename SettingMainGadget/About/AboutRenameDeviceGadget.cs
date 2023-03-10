using SettingAppTextResopurces.TextResources;
using Tizen.NUI.BaseComponents;

namespace Setting.Menu
{
    public class AboutRenameDeviceGadget : SettingCore.MenuGadget
    {
        public override string ProvideTitle() => Resources.IDS_ST_HEADER_RENAME_DEVICE;
    
        protected override View OnCreate()
        {
            base.OnCreate();
            WebView view = new WebView();

            return view;
        }
    }
}
