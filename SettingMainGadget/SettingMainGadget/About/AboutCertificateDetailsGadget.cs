using SettingCore;
using SettingCore.Views;
using SettingMainGadget.About;
using SettingMainGadget.TextResources;
using System.Linq;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using static Interop.CertSvc;

namespace Setting.Menu.About
{
    public class AboutCertificateDetailsGadget : MenuGadget
    {
        public override string ProvideTitle() => NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_CERTIFICATE_INFORMATION));

        private ScrollableBase content;
        private bool showPublicKey;

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

            CreateItems();

            return content;
        }

        private void CreateItems()
        {
            content.RemoveAllChildren(true);

            SettingCertificateManager.CertificateMetadata.GetMetadata();
            var cert = SettingCertificateManager.CertificateMetadata;

            /* Use Certificate */
            var useCertItem = new SwitchListItem("Use certificate", isSelected: cert.status == CertStatus.ENABLED);
            useCertItem.Clicked += (o, s) =>
            {
                // TODO : add function to activate or deactivate the certificate 
            };
            content.Add(useCertItem);

            /* Owner Certificate */
            content.Add(new TextHeaderListItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_SCP_BODY_OWNER_ABB))));

            var commonNameItem = TextListItem.CreatePrimaryTextItemWithSecondaryText(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_COMMON_NAME_C)), cert.GetField(CertificateField.CERTSVC_SUBJECT_COMMON_NAME));
            var orgItem = TextListItem.CreatePrimaryTextItemWithSecondaryText(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_ORGANISATION_C)), cert.GetField(CertificateField.CERTSVC_SUBJECT_ORGANIZATION_NAME));
            content.Add(commonNameItem);
            content.Add(orgItem);

            /* Issuer Certificate */
            content.Add(new TextHeaderListItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_ISSUER))));

            var issuerCommonNameItem = TextListItem.CreatePrimaryTextItemWithSecondaryText(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_ISSUER_COLON)), cert.GetField(CertificateField.CERTSVC_ISSUER_COMMON_NAME));
            var issuerOrgItem = TextListItem.CreatePrimaryTextItemWithSecondaryText(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_ORGANISATION_C)), cert.GetField(CertificateField.CERTSVC_ISSUER_ORGANIZATION_NAME));
            content.Add(issuerCommonNameItem);
            content.Add(issuerOrgItem);

            /* Issuer Certificate information */
            content.Add(new TextHeaderListItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_CERTIFICATE_INFORMATION))));

            var issuerVersionItem = TextListItem.CreatePrimaryTextItemWithSecondaryText(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_VERSION_C)), cert.GetField(CertificateField.CERTSVC_VERSION));            
            var issuerValidFromItem = TextListItem.CreatePrimaryTextItemWithSecondaryText(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_VALID_FROM_C)), cert.before.ToString("U"));
            var issuerValidToItem = TextListItem.CreatePrimaryTextItemWithSecondaryText(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_VALID_TO_C)), cert.after.ToString("U"));
            var issuerSerialItem = TextListItem.CreatePrimaryTextItemWithSecondaryText(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_SERIAL_NUMBER_COLON)), cert.GetField(CertificateField.CERTSVC_SERIAL_NUMBER));
            var issuerSignatureItem = TextListItem.CreatePrimaryTextItemWithSecondaryText(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_SIGNATURE_ALGORITHM_C)), cert.GetField(CertificateField.CERTSVC_SIGNATURE_ALGORITHM));
            var issuerKeyItem = TextListItem.CreatePrimaryTextItemWithSecondaryText(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_KEY_USAGE_C)), cert.GetField(CertificateField.CERTSVC_KEY_USAGE));
            var issuerAuthorityItem = TextListItem.CreatePrimaryTextItemWithSecondaryText(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_KEY_USAGE_C)), cert.rootCa == 1 ? NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_TRUE)) : NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_FALSE)));
            var issuerPublicKeyItem = TextListItem.CreatePrimaryTextItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_PUBLIC_KEY_C)));

            issuerPublicKeyItem.Clicked += (o, s) =>
            {
                if (!showPublicKey)
                {
                    var publicKey = new TextLabel()
                    {
                        Text = cert.GetField(CertificateField.CERTSVC_KEY),
                        TextColor = new Color("#090E21"),
                        MultiLine = true,
                        WidthSpecification = LayoutParamPolicies.MatchParent,
                        Margin = new Extents(24, 24, 0, 0).SpToPx(),
                        PixelSize = 20.SpToPx(),
                    };

                    publicKey.Relayout += (s, e) =>
                    {
                        var scrollAnimation = new Animation(100);
                        scrollAnimation.AnimateTo(content.ContentContainer, "PositionY", -issuerPublicKeyItem.PositionY);
                        scrollAnimation.Play();
                        scrollAnimation.Finished += (s, e) =>
                        {
                            scrollAnimation.Dispose();
                        };
                    };

                    content.Add(publicKey);
                }
                else
                {
                    content.Remove(content.ContentContainer.Children.Last());
                }

                showPublicKey = !showPublicKey;
            };

            content.Add(issuerVersionItem);
            content.Add(issuerValidFromItem);
            content.Add(issuerValidToItem);
            content.Add(issuerSerialItem);
            content.Add(issuerSignatureItem);
            content.Add(issuerKeyItem);
            content.Add(issuerAuthorityItem);
            content.Add(issuerPublicKeyItem);
        }
    }
}
