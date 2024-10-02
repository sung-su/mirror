using SettingCore;
using SettingCore.Views;
using SettingMainGadget;
using SettingMainGadget.About;
using SettingMainGadget.TextResources;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tizen.Applications;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.Security.SecureRepository;
using static Interop.CertSvc;

namespace Setting.Menu.About
{
    internal class AboutUserCertificatesGadget : MenuGadget
    {
        private View content;
        private ScrollableBase vpnTabContent, wifiTabContent, emailTabContent;

        private MoreMenuItem installMenuItem, uninstallMenuItem;
        private TabButtonStyle tabButtonStyle;
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

            tabButtonStyle = ThemeManager.GetStyle("Tizen.NUI.Components.TabButton") as TabButtonStyle;
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

        private void AddTabsContent()
        {
            Logger.Debug("AddTabsContent()");

            OnPageAppeared -= AddTabsContent;

            // Add VPN Cert Tab content
            AddTabContent(SettingCertificateManager.GetVPNUserCertList(), tabContent: vpnTabContent);

            // Add Wi-Fi Cert Tab content
            AddTabContent(SettingCertificateManager.GetWiFiUserCertList(), tabContent: wifiTabContent);

            // Add Email Cert Tab content
            AddTabContent(SettingCertificateManager.GetEmailUserCertList(), tabContent: emailTabContent);
        }

        private async void AddTabContent(List<certificateMetadata> certList, ScrollableBase tabContent)
        {
            tabContent.RemoveAllChildren(true);

            if (certList.Count == 0)
            {
                await AddEmptyTabContent(content: tabContent);
                return;
            }

            foreach (var certificate in certList)
            {
                await AddCertificateItem(certificate, content: tabContent);
            }
        }

        private Task AddEmptyTabContent(ScrollableBase content)
        {
            return Task.Run(async () =>
            {
                await CoreApplication.Post(() =>
                {
                    var noCertLabel = new TextLabel
                    {
                        WidthSpecification = LayoutParamPolicies.MatchParent,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Text = "No certificates", // TODO: Add DID
                        PixelSize = 24.SpToPx(),
                        Margin = new Extents(16, 16, 0, 0).SpToPx(),
                        TextColor = Color.Black,
                    };

                    var infoLabel = new TextLabel
                    {
                        WidthSpecification = LayoutParamPolicies.MatchParent,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        MultiLine = true,
                        Ellipsis = true,
                        Text = "After you install certificates, they will be shown here.",
                        PixelSize = 24.SpToPx(),
                        Margin = new Extents(16, 16, 0, 0).SpToPx(),
                        TextColor = Color.Black,
                    };

                    content.Add(noCertLabel);
                    content.Add(infoLabel);

                    return true;
                });
            });
        }

        private Task AddCertificateItem(certificateMetadata certificate, ScrollableBase content)
        {
            return Task.Run(async () =>
            {
                await CoreApplication.Post(() =>
                {
                    var status = certificate.status == CertStatus.DISABLED ? NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_OFF)) 
                    : NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_ON));

                    var item = TextListItem.CreatePrimaryTextItemWithSecondaryText(certificate.title, status);
                    item.Clicked += (s, e) =>
                    {
                        SettingCertificateManager.EnteredCertificateMetadata = certificate;
                        NavigateTo(MainMenuProvider.About_CertificateDetails);
                    };
                    content.Add(item);
                    return true;
                });
            });
        }

        protected override void OnDestroy()
        {
            Logger.Debug("OnDestroy()");

            base.OnDestroy();
            tabButtonStyle.dispose();
        }
    }
}
