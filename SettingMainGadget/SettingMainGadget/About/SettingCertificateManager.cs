using SettingCore;
using System;
using System.Collections.Generic;
using static Interop;
using static Interop.CertSvc;

namespace SettingMainGadget.About
{
    class SettingCertificateManager
    {
        public static certificateMetadata EnteredCertificateMetadata { get; set; }

        public static List<certificateMetadata> GetRootCertList()
        {
            int result = CertSvc.InstanceNew(out IntPtr instance);
            if ((CertSvc.ErrorCode)result != CertSvc.ErrorCode.CERTSVC_SUCCESS)
            {
                Logger.Warn($"Unable to create cert instance, result: {result}");
                return new List<certificateMetadata>();
            }

            result = CertSvc.GetPKCS12CertificateListFromStore(instance, CertSvc.CertStoreType.ALL_STORE, 0, out IntPtr storeCertList, out int length);
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

            return CertSvc.GetCertLists(instance, storeCertList, length);
        }
    }
}
