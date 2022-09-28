using System;
using Tizen.NUI;
using Tizen.NUI.Components;
using Tizen.Applications;

using SettingAppTextResopurces.TextResources;

namespace SettingMain
{
    class SettingContent_LegalInfo : Widget
    {
        protected override void OnCreate(string contentInfo, Window window)
        {
            Bundle bundle = Bundle.Decode(contentInfo);

            window.BackgroundColor = Color.Transparent;

            var appBar = new AppBar()
            {
                Title = Resources.IDS_ST_BODY_OPEN_SOURCE_LICENCES,
                AutoNavigationContent = false,
            };

            var appBarStyle = ThemeManager.GetStyle("Tizen.NUI.Components.AppBar");

            var navigationContent = new Button(((AppBarStyle)appBarStyle).BackButton);
            navigationContent.Clicked += (o, e) =>
            {
                // Update Widget Content by sending message to pop the third page.
                Bundle nextBundle2 = new Bundle();
                nextBundle2.AddItem("WIDGET_ACTION", "POP");
                String encodedBundle2 = nextBundle2.Encode();
                SetContentInfo(encodedBundle2);
            };

            appBar.NavigationContent = navigationContent;

            var page = new ContentPage()
            {
                AppBar = appBar,
            };

            var view = new Tizen.NUI.BaseComponents.WebView();

            string legalinfo_url = "file:///usr/share/license.html";
            view.LoadUrl(legalinfo_url);


            page.Content = view;
            window.Add(page);
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        protected override void OnResume()
        {
            base.OnResume();
        }

        protected override void OnResize(Window window)
        {
            base.OnResize(window);
        }

        protected override void OnTerminate(string contentInfo, TerminationType type)
        {
            base.OnTerminate(contentInfo, type);
        }

        protected override void OnUpdate(string contentInfo, int force)
        {
            base.OnUpdate(contentInfo, force);
        }
    }
}
