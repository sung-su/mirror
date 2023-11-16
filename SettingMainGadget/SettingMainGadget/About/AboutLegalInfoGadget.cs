using SettingMainGadget.TextResources;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;

namespace Setting.Menu
{
    public class AboutLegalInfoGadget : SettingCore.MenuGadget
    {
        const string LegalinfoURL = "file:///usr/share/license.html";
        public override string ProvideTitle() => NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_OPEN_SOURCE_LICENCES));

        private View content;
        private WebView view;
        private Loading webViewLoadingIndicator;

        protected override View OnCreate()
        {
            base.OnCreate();

            content = new View()
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
                Layout = new LinearLayout()
                {
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                },
            };

            OnPageAppeared += CreateView;

            return content;
        }

        private void CreateView()
        {
            webViewLoadingIndicator = new Loading();
            webViewLoadingIndicator.Play();
            content.Add(webViewLoadingIndicator);

            view = new WebView()
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
            };
            view.PageLoadFinished += View_PageLoadFinished;
            view.LoadUrl(LegalinfoURL);
        }

        private void View_PageLoadFinished(object sender, Tizen.NUI.WebViewPageLoadEventArgs e)
        {
            content.Add(view);
            webViewLoadingIndicator?.Stop();
            webViewLoadingIndicator?.Unparent();
            webViewLoadingIndicator?.Dispose();
            view.PageLoadFinished -= View_PageLoadFinished;
        }

        protected override void OnDestroy()
        {
            view?.Unparent();
            view?.Dispose();

            base.OnDestroy();
        }
    }
}
