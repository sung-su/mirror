using SettingMainGadget.TextResources;
using SettingCore.Views;
using System;
using Tizen.NUI.BaseComponents;
using Tizen.NUI;
using Tizen.NUI.Components;
using SettingCore;
using Tizen;

namespace Setting.Menu.Storage
{
    public class DefaultStorageGadget : SettingCore.MenuGadget
    {
        public override string ProvideTitle() => NUIGadgetResourceManager.GetString(nameof(Resources.IDS_SM_HEADER_DEFAULT_STORAGE_SETTINGS_ABB));

        private const string VconfSharedContent = "db/setting/default_memory/wifi_direct";
        private const string VconfAppInstall = "db/setting/default_memory/install_applications";
        private const string VconfCardStatus = "memory/sysman/mmc";

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

            Vconf.Notify(VconfCardStatus, OnCardStatusChanged);

            return content;
        }

        protected override void OnDestroy()
        {
            Vconf.Ignore(VconfCardStatus, OnCardStatusChanged);

            base.OnDestroy();
        }

        private void CreateItems()
        {
            content.RemoveAllChildren(true);

            GetStorageStatus(out int shared, out int app, out int status);

            // Shared contents
            var headerItem = new TextHeaderListItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_HEADER_SHARED_CONTENT)));
            content.Add(headerItem);

            headerItem = new TextHeaderListItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_SM_BODY_SELECT_THE_DEFAULT_STORAGE_LOCATION_FOR_CONTENT_SHARED_VIA_BLUETOOTH_OR_WI_FI_DIRECT)), true);
            content.Add(headerItem);

            RadioButtonGroup radioButtonStorageGroup = new RadioButtonGroup();

            RadioButtonListItem device = new RadioButtonListItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_DEVICE_STORAGE)));
            device.RadioButton.IsSelected = shared == 0;

            RadioButtonListItem card = new RadioButtonListItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_SD_CARD)));
            // disable storage selection if the card is not mounted 
            if (status != 1)
            {
                card.IsEnabled = false;
            }
            card.RadioButton.IsSelected = shared == 1;

            radioButtonStorageGroup.SelectedChanged += (o, e) =>
            {
                Vconf.SetInt(VconfSharedContent, radioButtonStorageGroup.SelectedIndex);
                Logger.Debug($"Default storage of Shared Content is {radioButtonStorageGroup.GetSelectedItem().Text}.");
            };

            radioButtonStorageGroup.Add(device.RadioButton);
            radioButtonStorageGroup.Add(card.RadioButton);

            content.Add(device);
            content.Add(card);

            // App installation
            headerItem = new TextHeaderListItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_HEADER_APP_INSTALLATION)));
            content.Add(headerItem);

            headerItem = new TextHeaderListItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_SM_BODY_SELECT_THE_DEFAULT_LOCATION_FOR_INSTALLING_APPS_WHERE_APPS_CAN_BE_SAVED_DEPENDS_ON_THE_TYPE_OF_APP_AND_THE_AVAILABILITY_OF_THE_LOCATION)), true);
            content.Add(headerItem);

            RadioButtonGroup radioButtonAppGroup = new RadioButtonGroup();

            RadioButtonListItem deviceStorage = new RadioButtonListItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_DEVICE_STORAGE)));
            deviceStorage.RadioButton.IsSelected = app == 0;

            RadioButtonListItem cardStorage = new RadioButtonListItem(NUIGadgetResourceManager.GetString(nameof(Resources.IDS_ST_BODY_SD_CARD)));
            // disable storage selection if the card is not mounted
            if (status != 1)
            {
                cardStorage.IsEnabled = false;
            }
            cardStorage.RadioButton.IsSelected = app == 1;

            radioButtonAppGroup.SelectedChanged += (o, e) =>
            {
                Vconf.SetInt(VconfAppInstall, radioButtonAppGroup.SelectedIndex);
                Logger.Debug($"Default storage of App instalation is {radioButtonAppGroup.GetSelectedItem().Text}.");
            };

            radioButtonAppGroup.Add(deviceStorage.RadioButton);
            radioButtonAppGroup.Add(cardStorage.RadioButton);

            content.Add(deviceStorage);
            content.Add(cardStorage);
        }

        private void GetStorageStatus(out int shared, out int app, out int status)
        {
            if (!Vconf.TryGetInt(VconfSharedContent, out shared))
            {
                Logger.Warn($"could not get value for {VconfSharedContent}");
            }

            if (!Vconf.TryGetInt(VconfAppInstall, out app))
            {
                Logger.Warn($"could not get value for {VconfAppInstall}");
            }

            if (!Vconf.TryGetInt(VconfCardStatus, out status))
            {
                Logger.Warn($"could not get value for {VconfCardStatus}");
            }

            // set device as default shared content and app instalation storage if card was umounted
            // FIXME : remove it if the change of card status (memory/sysman/mmc) will affect wifi_direct and install_applications
            if (status != 1)
            {
                if (app == 1)
                {
                    Vconf.SetInt(VconfAppInstall, 0);
                    app = 0;
                }

                if (shared == 1)
                {
                    Vconf.SetInt(VconfSharedContent, 0);
                    shared = 0;
                }
            }
        }

        private void OnCardStatusChanged(string key, Type type, dynamic arg)
        {
            // update content when card was removed (0) or mounted (1)
            if (arg == 0 || arg == 1)
            {
                CreateItems();
            }
        }
    }
}
