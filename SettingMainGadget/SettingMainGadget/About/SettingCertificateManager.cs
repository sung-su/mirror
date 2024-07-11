using SettingCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Interop;
using static Interop.CertSvc;
using static Tizen.Pims.Calendar.CalendarTypes;

namespace SettingMainGadget.About
{
    class SettingCertificateManager
    {
        public static certificateMetadata EnteredCertificateMetadata { get; set; }

        private static List<certificateMetadata> allRootCert = new List<certificateMetadata> ();
        private static List<certificateMetadata> vpnUserCert = new List<certificateMetadata> ();
        private static List<certificateMetadata> wifiUserCert = new List<certificateMetadata> ();
        private static List<certificateMetadata> emailUserCert = new List<certificateMetadata> ();

        public static void LoadCertificateData()
        {
            int result = CertSvc.InstanceNew(out IntPtr instance);
            if ((CertSvc.ErrorCode)result != CertSvc.ErrorCode.CERTSVC_SUCCESS)
            {
                Logger.Warn($"Unable to create cert instance, result: {result}");
                return;
            }

            // All rootCA
            allRootCert = GetRootCertList(instance, storeType: CertSvc.CertStoreType.ALL_STORE);

            // VPN UserCA
            vpnUserCert = GetUserCertList(instance, CertStoreType.VPN_STORE);

            // Wi-Fi UserCA
            wifiUserCert = GetUserCertList(instance, CertStoreType.WIFI_STORE);

            // Email UserCA
            emailUserCert = GetUserCertList(instance, CertStoreType.EMAIL_STORE);

            CertSvc.InstanceFree(instance);
        }

        public static List<certificateMetadata> GetAllRootCertList()
        {
            return allRootCert;
        }

        public static List<certificateMetadata> GetVPNUserCertList()
        {
            return vpnUserCert;
        }

        public static List<certificateMetadata> GetWiFiUserCertList()
        {
            return wifiUserCert;
        }

        public static List<certificateMetadata> GetEmailUserCertList()
        {
            return emailUserCert;
        }

        private static List<certificateMetadata> GetRootCertList(IntPtr instance, CertStoreType storeType)
        {
            int result = CertSvc.GetPKCS12CertificateListFromStore(instance, storeType, 0, out IntPtr storeCertList, out int length);
            if ((CertSvc.ErrorCode)result != CertSvc.ErrorCode.CERTSVC_SUCCESS)
            {
                Logger.Warn($"Unable to get root certificates, result: {result}");
                result = CertSvc.FreePKCS12CertificateListFromStore(instance, out storeCertList);
                if ((CertSvc.ErrorCode)result != CertSvc.ErrorCode.CERTSVC_SUCCESS)
                {
                    Logger.Warn($"Unable to free certlist, result: {result}");
                }
                CertSvc.InstanceFree(instance);
                return new List<certificateMetadata>();
            }

            List<certificateMetadata> certList = CertSvc.GetCertLists(instance, storeCertList, length);

            result = CertSvc.FreePKCS12CertificateListFromStore(instance, out storeCertList);
            if ((CertSvc.ErrorCode)result != CertSvc.ErrorCode.CERTSVC_SUCCESS)
            {
                Logger.Warn("Unable to free root certlist, result: " + result.ToString());
            }

            return certList;
        }

        private static List<certificateMetadata> GetUserCertList(IntPtr instance, CertStoreType storeType)
        {
            int result = CertSvc.GetPKCS12UserCertificateListFromStore(instance, storeType, out IntPtr storeCertList, out int length);
            if ((CertSvc.ErrorCode)result != CertSvc.ErrorCode.CERTSVC_SUCCESS)
            {
                Logger.Warn($"Unable to get user certificates, result: {result}");
                result = CertSvc.FreePKCS12CertificateListFromStore(instance, out storeCertList);
                if ((CertSvc.ErrorCode)result != CertSvc.ErrorCode.CERTSVC_SUCCESS)
                {
                    Logger.Warn($"Unable to free certlist, result: {result}");
                }
                CertSvc.InstanceFree(instance);
                return new List<certificateMetadata>();
            }

            List<certificateMetadata> certList = CertSvc.GetCertLists(instance, storeCertList, length);

            result = CertSvc.FreePKCS12CertificateListFromStore(instance, out storeCertList);
            if ((CertSvc.ErrorCode)result != CertSvc.ErrorCode.CERTSVC_SUCCESS)
            {
                Logger.Warn("Unable to free user certlist, result: " + result.ToString());
            }

            return certList;
        }
    }
}
