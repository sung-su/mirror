using System;
using Tizen.NUI;
using Tizen.NUI.Components;
using Tizen.NUI.BaseComponents;
using Tizen.Applications;

using SettingAppTextResopurces.TextResources;

namespace SettingMain
{
    class SettingContent_LegalInfo : SettingContent_Base
    {
        const string LegalinfoURL = "file:///usr/share/license.html";

        public SettingContent_LegalInfo()
            : base()
        {
            mTitle = Resources.IDS_ST_BODY_OPEN_SOURCE_LICENCES;
        }


        protected override View CreateContent(Window window)
        {
            Tizen.NUI.BaseComponents.WebView view = new Tizen.NUI.BaseComponents.WebView();

            view.LoadUrl(LegalinfoURL);

            return view;
        }
    }
}
