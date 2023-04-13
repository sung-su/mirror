using SettingCore.TextResources;
using Tizen.NUI.BaseComponents;

namespace Setting.Menu
{
    public class AboutLegalInfoGadget : SettingCore.MenuGadget
    {
        const string LegalinfoURL = "file:///usr/share/license.html";
        public override string ProvideTitle() => Resources.IDS_ST_BODY_OPEN_SOURCE_LICENCES;

        protected override View OnCreate()
        {
            base.OnCreate();
            WebView view = new WebView();
            view.LoadUrl(LegalinfoURL);

            return view;
        }
    }
}
