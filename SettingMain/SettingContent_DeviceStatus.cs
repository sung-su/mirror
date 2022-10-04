using System;
using System.Collections.Generic; // for Dictionary
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.Applications;

using Tizen.System;

using SettingAppTextResopurces.TextResources;
using Tizen.Telephony;
using Tizen.Network;

namespace SettingMain
{
    class SettingContent_DeviceStatus : SettingContent_Base
    {
        public SettingContent_DeviceStatus()
            : base()
        {
            mTitle = Resources.IDS_ST_MBODY_DEVICE_STATUS;
        }

        protected override View CreateContent(Window window)
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
                item = CreateItemWithCheck(Resources.IDS_ST_BODY_PHONE_NUMBER, sim.SubscriberNumber);
                content.Add(item);

                Modem modem = new Modem(enumerator.Current);
                item = CreateItemWithCheck(Resources.IDS_ST_BODY_IMEI, modem.Imei);
                content.Add(item);

                Tizen.Telephony.Manager.Deinit();
            }
#endif


#if false
            item = CreateItemWithCheck(Resources.IDS_ST_MBODY_BLUETOOTH_ADDRESS, Tizen.Network.Bluetooth.BluetoothAdapter.Address);
            content.Add(item);
#endif

#if false
            item = CreateItemWithCheck(Resources.IDS_ST_BODY_WI_FI_MAC_ADDRESS, Tizen.Network.WiFi.WiFiManager.MacAddress);
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
            item = CreateItemWithCheck(Resources.IDS_ST_BODY_STORAGE,
            String.Format("{0:0.0}GB available (Total {1:0.0}GB)", (double)(available / 1000000000.0), (double)(total / 1000000000.0)));
            content.Add(item);



            // To do : Caluacate CPU Usage
            // Tizen.System.ProcessCpuUsage
            item = CreateItemWithCheck(Resources.IDS_ST_BODY_CPU_USAGE, Resources.IDS_ST_HEADER_UNAVAILABLE);
            content.Add(item);

            return content;
        }
    }
}
