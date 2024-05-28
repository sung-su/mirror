using SettingCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;

namespace Setting.Menu.About
{
    internal class AboutUserCertificatesGadget : MenuGadget
    {
        private View content;
        ScrollableBase vpnTabContent, wifiTabContent, emailTabContent;

        private MoreMenuItem installMenuItem, uninstallMenuItem;

        public override string ProvideTitle() => "User certificates";

        public override IEnumerable<MoreMenuItem> ProvideMoreMenu() => Moremenu();

        private List<MoreMenuItem> Moremenu()
        {
            Logger.Debug("UserCertificateMoreMenu()");

            installMenuItem = new MoreMenuItem()
            {
                Text = "Install",

                Action = () => {
                    // Do action
                }
            };
            uninstallMenuItem = new MoreMenuItem()
            {
                Text = "Uninstall",

                Action = () => {
                    // Do action
                }
            };

            return new List<MoreMenuItem> { installMenuItem, uninstallMenuItem };
        }

        protected override View OnCreate()
        {
            Logger.Debug("OnCreate()");

            base.OnCreate();

            content = new View()
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
                Layout = new LinearLayout()
                {
                   LinearOrientation = LinearLayout.Orientation.Vertical,
                },
            };

            AddMenuTabs();

            OnPageAppeared += AddTabsContent;

            return content;
        }

        private void AddMenuTabs()
        {
            Logger.Debug("AddMenuTabs()");

            var tabView = new TabView()
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
            };

            var tabButtonStyle = ThemeManager.GetStyle("Tizen.NUI.Components.TabButton") as TabButtonStyle;
            tabButtonStyle.Padding = new Extents(2, 2, 16, 16).SpToPx();
            tabButtonStyle.Icon.Size = new Size(2, -1).SpToPx();

            // VPN tab
            var vpnTabButton = new TabButton(tabButtonStyle)
            {
                // TODO: Add string resouce
                Text = "VPN",
            };
            vpnTabContent = TabView();

            // Wi-Fi tab
            var wifiTabButton = new TabButton(tabButtonStyle)
            {
                // TODO: Add string resouce
                Text = "Wi-Fi",
            };
            wifiTabContent = TabView();

            // Email tab
            var emailTabButton = new TabButton(tabButtonStyle)
            {
                // TODO: Add string resouce
                Text = "Email",
            };
            emailTabContent = TabView();

            tabView.AddTab(vpnTabButton, vpnTabContent);
            tabView.AddTab(wifiTabButton, wifiTabContent);
            tabView.AddTab(emailTabButton, emailTabContent);

            content.Add(tabView);
        }

        private async void AddTabsContent()
        {
            Logger.Debug("AddTabsContent()");

            OnPageAppeared -= AddTabsContent;

            AddVpnCertifiates();
            AddWiFiCertificates();
            AddEmailCertifiates();
        }

        private ScrollableBase TabView()
        {
            return new ScrollableBase()
             {
                 WidthSpecification = LayoutParamPolicies.MatchParent,
                 HeightSpecification = LayoutParamPolicies.MatchParent,
                 ScrollingDirection = ScrollableBase.Direction.Vertical,
                 HideScrollbar = false,
                 Layout = new LinearLayout()
                 {
                     LinearOrientation = LinearLayout.Orientation.Vertical,
                     VerticalAlignment = VerticalAlignment.Center,
                     HorizontalAlignment = HorizontalAlignment.Center,
                 },
             };
        }

        private void AddVpnCertifiates()
        {
            // TODO: Add implementation
        }

        private void AddWiFiCertificates()
        {
            // TODO: Add implementation
        }

        private void AddEmailCertifiates()
        {
            // TODO: Add implementation
        }

        protected override void OnDestroy()
        {
            Logger.Debug("OnDestroy()");

            base.OnDestroy();
        }
    }
}
