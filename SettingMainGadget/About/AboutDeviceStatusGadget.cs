using SettingCore.TextResources;
using SettingCore;
using SettingCore.Customization;
using SettingCore.Views;
using SettingMainGadget;
using System;
using System.Collections.Generic;
using System.Linq;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using Tizen.System;
using Tizen.Telephony;

namespace Setting.Menu
{
    public class AboutDeviceStatusGadget : SettingCore.MenuGadget
    {
        private SystemCpuUsage systemCpuUsage;
        private TextListItem cpuUsage;
        private Timer timer;
        private View content;
        private Sections sections = new Sections();

        public override string ProvideTitle() => Resources.IDS_ST_MBODY_DEVICE_STATUS;
    
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

            CreateView();

            return content;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            timer.Tick -= timerTick;
            timer.Stop();
        }

        private void CreateView()
        {
            sections.RemoveAllSectionsFromView(content);
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
                Logger.Warn($"Could not get bluetooth address.");
            }

            var btAddress = TextListItem.CreatePrimaryTextItemWithSecondaryText(Resources.IDS_ST_MBODY_BLUETOOTH_ADDRESS, addressBT);
            if (btAddress != null)
            {
                sections.Add(MainMenuProvider.About_DeviceStatus_bt_address, btAddress);
            }
#endif

#if true
            string addressMac = Resources.IDS_ST_HEADER_UNAVAILABLE;
            if (Tizen.Network.WiFi.WiFiManager.IsActive)
                addressMac = Tizen.Network.WiFi.WiFiManager.MacAddress;
            else
                addressMac = Resources.IDS_ST_SBODY_DISABLED;

            var wifiMacAddress = TextListItem.CreatePrimaryTextItemWithSecondaryText(Resources.IDS_ST_BODY_WI_FI_MAC_ADDRESS, addressMac);
            if (wifiMacAddress != null)
            {
                sections.Add(MainMenuProvider.About_DeviceStatus_wifi_mac_address, wifiMacAddress);

            }
#endif
            IEnumerator<Storage> storages = StorageManager.Storages.GetEnumerator();
            ulong total = 0, available = 0;
            while (storages.MoveNext())
            {
                Storage deviceStorage = storages.Current;
                total += deviceStorage.TotalSpace;
                available += deviceStorage.AvailableSpace;
            }
            var storage = TextListItem.CreatePrimaryTextItemWithSecondaryText(Resources.IDS_ST_BODY_STORAGE,
            String.Format("{0:0.0}GB available (Total {1:0.0}GB)", (double)(available / 1000000000.0), (double)(total / 1000000000.0)));
            if (storage != null)
            {
                sections.Add(MainMenuProvider.About_DeviceStatus_storage, storage);
            }

#if true
            systemCpuUsage = new SystemCpuUsage();
            cpuUsage = TextListItem.CreatePrimaryTextItemWithSecondaryText(Resources.IDS_ST_BODY_CPU_USAGE, getCpuUsageFormated());
            if (cpuUsage != null)
            {
                sections.Add(MainMenuProvider.About_DeviceStatus_cpu_usage, cpuUsage);
                startTimer();
            }
#endif

            // add only visible sections to content view in required order
            var customization = GetCustomization().OrderBy(c => c.Order);
            foreach (var cust in customization)
            {
                string visibility = cust.IsVisible ? "visible" : "hidden";
                Logger.Verbose($"Customization: {cust.MenuPath} - {visibility} - {cust.Order}");
                if (cust.IsVisible && sections.TryGetValue(cust.MenuPath, out View row))
                {
                    content.Add(row);
                }
            }
        }

        protected override void OnCustomizationUpdate(IEnumerable<MenuCustomizationItem> items)
        {
            Logger.Verbose($"{nameof(AboutGadget)} got customization with {items.Count()} items. Recreating view.");
            CreateView();
        }

        private string getCpuUsageFormated()
        {
            string system = $"{systemCpuUsage.System.ToString("0.#")}%";
            Logger.Verbose($"CPU usage: {system}");
            return system;
        }

        private void startTimer()
        {
            timer = new Timer(1000);
            timer.Tick += timerTick;
            timer.Start();
        }

        private bool timerTick(object source, Timer.TickEventArgs e)
        {
            systemCpuUsage.Update();
            var system = getCpuUsageFormated();
            cpuUsage.Secondary = system;

            return true;
        }
    }
}
