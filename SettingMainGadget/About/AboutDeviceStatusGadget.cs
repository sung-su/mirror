using SettingAppTextResopurces.TextResources;
using SettingMain;
using System;
using System.Collections.Generic;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.System;
using Tizen.Telephony;

namespace Setting.Menu
{
    public class AboutDeviceStatusGadget : SettingCore.MenuGadget
    {
        public override string ProvideTitle() => Resources.IDS_ST_MBODY_DEVICE_STATUS;
    
        protected override View OnCreate()
        {
            base.OnCreate();

            return CreateContent();
        }

        private View CreateContent()
        {
            // Content of the page which scrolls items vertically.
            var content = new ScrollableBase()
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

            DefaultLinearItem item = null;
#if false
            IEnumerable<SlotHandle> handlelist = Tizen.Telephony.Manager.Init();
            if (handlelist != null)
            {
                IEnumerator<SlotHandle> enumerator = handlelist.GetEnumerator();

                Sim sim = new Sim(enumerator.Current);
                if (sim != null)
                {
                    item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_PHONE_NUMBER, sim.SubscriberNumber);
                    content.Add(item);
                }

                Modem modem = new Modem(enumerator.Current);
                if (modem != null)
                {
                    item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_IMEI, modem.Imei);
                    content.Add(item);
                }

                Tizen.Telephony.Manager.Deinit();
            }
#endif


#if true
            string addressBT = Resources.IDS_ST_HEADER_UNAVAILABLE;
            try
            {
                if (Tizen.Network.Bluetooth.BluetoothAdapter.IsBluetoothEnabled)
                    addressBT = Tizen.Network.Bluetooth.BluetoothAdapter.Address;
                else
                    addressBT = Resources.IDS_ST_SBODY_DISABLED;
            }
            catch (Exception e)
            {
            }

            item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_MBODY_BLUETOOTH_ADDRESS, addressBT);
            content.Add(item);
#endif

#if true
            string addressMac = Resources.IDS_ST_HEADER_UNAVAILABLE;
            if (Tizen.Network.WiFi.WiFiManager.IsActive)
                addressMac = Tizen.Network.WiFi.WiFiManager.MacAddress;
            else
                addressMac = Resources.IDS_ST_SBODY_DISABLED;

            item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_WI_FI_MAC_ADDRESS, addressMac);
            content.Add(item);
#endif
            IEnumerator<Storage> storages = StorageManager.Storages.GetEnumerator();
            ulong total = 0, available = 0;
            while (storages.MoveNext())
            {
                Storage storage = storages.Current;
                total += storage.TotalSpace;
                available += storage.AvailableSpace;
            }
            item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_STORAGE,
            String.Format("{0:0.0}GB available (Total {1:0.0}GB)", (double)(available / 1000000000.0), (double)(total / 1000000000.0)));
            content.Add(item);


#if false
            // To do : Caluacate CPU Usage
            // Tizen.System.ProcessCpuUsage
            item = SettingItemCreator.CreateItemWithCheck(Resources.IDS_ST_BODY_CPU_USAGE, Resources.IDS_ST_HEADER_UNAVAILABLE);
            content.Add(item);
#endif
            return content;
        }

    }
}
