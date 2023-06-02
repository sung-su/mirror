using SettingMainGadget.TextResources;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using SettingCore.Views;
using SettingMainGadget;

namespace Setting.Menu.About
{
    public class AboutManageCertificatesGadget : SettingCore.MenuGadget
    {
        public override string ProvideTitle() => NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_HEADER_MANAGE_CERTIFICATES_ABB));

        private ScrollableBase content;

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

            var rootCertItem = TextListItem.CreatePrimaryTextItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_TRUSTED_ROOT_CA_CERTIFICATES_ABB)));
            rootCertItem.Clicked += (o, e) =>
            {
                NavigateTo(MainMenuProvider.About_RootCertificates);
            };
            content.Add(rootCertItem);

            var userCertItem = TextListItem.CreatePrimaryTextItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_USER_CERTIFICATES)));
            userCertItem.Clicked += (o, e) =>
            {
                // TODO : NavigateTo(UserCert);
            };
            content.Add(userCertItem);
        }
    }
}
