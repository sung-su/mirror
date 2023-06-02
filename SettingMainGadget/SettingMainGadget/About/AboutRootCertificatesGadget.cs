using SettingCore;
using SettingCore.Views;
using SettingMainGadget;
using SettingMainGadget.About;
using SettingMainGadget.TextResources;
using System.Collections.Generic;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using static Interop.CertSvc;

namespace Setting.Menu.About
{
    public class AboutRootCertificatesGadget : MenuGadget
    {
        public override string ProvideTitle() => NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_TRUSTED_ROOT_CA_CERTIFICATES_ABB));

        private ScrollableBase content;

        private List<certificateMetadata> rootCert = new List<certificateMetadata>();

        protected override View OnCreate()
        {
            base.OnCreate();

            content = new ScrollableBase()
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
                ScrollingDirection = ScrollableBase.Direction.Vertical,
                HideScrollbar = false,
                Layout = new LinearLayout()
                {
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                },
            };

            rootCert = SettingCertificateManager.GetRootCertList();

            CreateItems();

            return content;
        }

        private void CreateItems()
        {
            content.RemoveAllChildren(true);

            foreach (var certificate in rootCert)
            {
                var status = certificate.status == CertStatus.DISABLED ? NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_OFF)) : NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_ON));
                var item = TextListItem.CreatePrimaryTextItemWithSecondaryText(certificate.title, status);
                item.Clicked += (s, e) =>
                {
                    SettingCertificateManager.CertificateMetadata = certificate;
                    NavigateTo(MainMenuProvider.About_CertificateDetails);
                };
                content.Add(item);
            }
        }
    }
}
