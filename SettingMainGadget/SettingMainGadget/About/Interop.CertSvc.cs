/*
 *  Copyright (c) 2022 Samsung Electronics Co., Ltd All Rights Reserved
 *
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License
 */

using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Linq;
using SettingMainGadget.TextResources;
using SettingCore;

internal static partial class Interop
{
    internal static partial class CertSvc
    {
        internal static string LogTag = "InteropTest.CertSvc";
        internal enum ErrorCode
        {
            CERTSVC_TRUE = 1,
            CERTSVC_FALSE = 0,
            CERTSVC_SUCCESS = 1,
            CERTSVC_FAIL = 0,
            CERTSVC_BAD_ALLOC = -2,
            CERTSVC_WRONG_ARGUMENT = -4,
            CERTSVC_INVALID_ALGORITHM = -5,
            CERTSVC_INVALID_SIGNATURE = -6,
            CERTSVC_IO_ERROR = -7,
            CERTSVC_INVALID_PASSWORD = -8,
            CERTSVC_DUPLICATED_ALIAS = -9,
            CERTSVC_ALIAS_DOES_NOT_EXIST = -10,
            CERTSVC_INVALID_STORE_TYPE = -11,
            CERTSVC_INVALID_STATUS = -12,
            CERTSVC_INVALID_CERTIFICATE = -13,
        };
        internal enum CertStoreType
        {
            NONE_STORE = 0,
            VPN_STORE = 1 << 0,
            WIFI_STORE = 1 << 1,
            EMAIL_STORE = 1 << 2,
            SYSTEM_STORE = 1 << 3,
            ALL_STORE = VPN_STORE | WIFI_STORE | EMAIL_STORE | SYSTEM_STORE,
        };
        internal enum CertificateField
        {
            CERTSVC_SUBJECT,
            [Description("Common name")]
            CERTSVC_SUBJECT_COMMON_NAME,
            CERTSVC_SUBJECT_COUNTRY_NAME,
            CERTSVC_SUBJECT_STATE_NAME,
            CERTSVC_SUBJECT_LOCALITY_NAME,
            [Description("Organization")]
            CERTSVC_SUBJECT_ORGANIZATION_NAME,
            CERTSVC_SUBJECT_ORGANIZATION_UNIT_NAME,
            CERTSVC_SUBJECT_EMAIL_ADDRESS,
            /*    CERTSVC_SUBJECT_UID, */
            CERTSVC_ISSUER,
            [Description("Common name")]
            CERTSVC_ISSUER_COMMON_NAME,
            CERTSVC_ISSUER_COUNTRY_NAME,
            CERTSVC_ISSUER_STATE_NAME,
            CERTSVC_ISSUER_LOCALITY_NAME,
            [Description("Organization")]
            CERTSVC_ISSUER_ORGANIZATION_NAME,
            CERTSVC_ISSUER_ORGANIZATION_UNIT_NAME,
            CERTSVC_ISSUER_EMAIL_ADDRESS,
            /*    CERTSVC_ISSUER_UID, */
            [Description("Version")]
            CERTSVC_VERSION,
            [Description("Serial number")]
            CERTSVC_SERIAL_NUMBER,
            [Description("Key usage")]
            CERTSVC_KEY_USAGE,
            [Description("Public key")]
            CERTSVC_KEY,
            CERTSVC_KEY_ALGO,
            [Description("Signature algorithm")]
            CERTSVC_SIGNATURE_ALGORITHM,
        };
        public static string GetCertFieldDescription(CertificateField field)
        {
            Type genericEnumType = field.GetType();
            MemberInfo[] memberInfo = genericEnumType.GetMember(field.ToString());
            if ((memberInfo != null && memberInfo.Length > 0))
            {
                var _Attribs = memberInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
                if ((_Attribs != null && _Attribs.Count() > 0))
                {
                    return ((System.ComponentModel.DescriptionAttribute)_Attribs.ElementAt(0)).Description;
                }
            }
            return field.ToString();
        }

        internal enum CertStatus
        {
            DISABLED = 0,
            ENABLED = 1,
        };
        [StructLayout(LayoutKind.Sequential)]
        internal struct StoreCertList
        {
            internal IntPtr gname;
            internal IntPtr title;
            internal CertStatus status;
            internal CertStoreType storeType;
            internal IntPtr next;
        }
        internal class certificateMetadata
        {
            internal string gname;
            internal string title;
            internal CertStatus status;
            internal CertStoreType storeType;
            internal string[] fields;
            internal DateTime before;
            internal DateTime after;
            internal int rootCa;
            private bool gotMetadata = false;
            public certificateMetadata()
            {
                fields = new string[Enum.GetNames(typeof(CertificateField)).Length];
            }
            public string GetField(CertificateField field)
            {
                try
                {
                    return fields[(int)field];
                }
                catch (Exception ex)
                {
                    Logger.Error($"CertificateField: {ex.Message}");
                    return fields[(int)field];
                } 
            }
            public string GetFieldTitle(CertificateField field)
            {
                switch (field)
                {
                    case CertificateField.CERTSVC_SUBJECT_COMMON_NAME:
                        return Resources.IDS_ST_BODY_COMMON_NAME_C; /*Common name*/
                    case CertificateField.CERTSVC_SUBJECT_ORGANIZATION_NAME:
                        return Resources.IDS_ST_BODY_ORGANISATION_C; /*Organization*/
                    case CertificateField.CERTSVC_ISSUER_COMMON_NAME:
                        return Resources.IDS_ST_BODY_ISSUER_COLON; /*Common name*/
                    case CertificateField.CERTSVC_ISSUER_ORGANIZATION_NAME:
                        return Resources.IDS_ST_BODY_ORGANISATION_C; /*Organization*/
                    case CertificateField.CERTSVC_VERSION:
                        return Resources.IDS_ST_BODY_VERSION_C; /*Version*/
                    case CertificateField.CERTSVC_SERIAL_NUMBER:
                        return Resources.IDS_ST_BODY_SERIAL_NUMBER_COLON; /*Serial number*/
                    case CertificateField.CERTSVC_KEY_USAGE:
                        return Resources.IDS_ST_BODY_KEY_USAGE_C; /*Key usage*/
                    case CertificateField.CERTSVC_KEY:
                        return Resources.IDS_ST_BODY_PUBLIC_KEY_C; /*Public key*/
                    case CertificateField.CERTSVC_SIGNATURE_ALGORITHM:
                        return Resources.IDS_ST_BODY_SIGNATURE_ALGORITHM_C;  /*Signature algorithm*/

                    default:
                        break;
                }
                return GetCertFieldDescription(field);
            }
            public void GetMetadata()
            {
                if (gotMetadata == true)
                {
                    return;
                }
                InstanceNew(out IntPtr instance);
                if (instance == null)
                {
                    Tizen.Log.Debug(LogTag, "Unable to create new instance");
                    return;
                }
                ErrorCode err = (ErrorCode)GetPKCS12CertificateFromStore(instance, storeType, gname, out Certificate cout);
                if (err != ErrorCode.CERTSVC_SUCCESS)
                {
                    Tizen.Log.Debug(LogTag, "Unable to GetPKCS12CertificateFromStore, err: " + err + ", instance: "
                        + instance + " gname: " + gname + " store_type: " + storeType);
                    InstanceFree(instance);
                    return;
                }

                err = (ErrorCode)GetCertificateNotAfter(cout, out long time);
                if (err != ErrorCode.CERTSVC_SUCCESS)
                {
                    Tizen.Log.Debug(LogTag, "Unable to GetCertificateNotAfter, , err: " + err + ", instance: "
                    + instance + " gname: " + gname + " store_type: " + storeType);
                    InstanceFree(instance);
                    return;
                }
                after = new DateTime(1970, 1, 1).ToLocalTime().AddSeconds(time);
                err = (ErrorCode)GetCertificateNotBefore(cout, out time);
                if (err != ErrorCode.CERTSVC_SUCCESS)
                {
                    Tizen.Log.Debug(LogTag, "Unable to GetCertificateNotBefore, , err: " + err + ", instance: "
                    + instance + " gname: " + gname + " store_type: " + storeType);
                    InstanceFree(instance);
                    return;
                }
                before = new DateTime(1970, 1, 1).ToLocalTime().AddSeconds(time);
                err = (ErrorCode)GetCertificateIsRootCa(cout, out int isRootCa);
                if (err != ErrorCode.CERTSVC_SUCCESS)
                {
                    Tizen.Log.Debug(LogTag, "Unable to GetCertificateIsRootCa, , err: " + err + ", instance: "
                    + instance + " gname: " + gname + " store_type: " + storeType);
                    InstanceFree(instance);
                    return;
                }
                rootCa = isRootCa;
                foreach (CertificateField certificateField in Enum.GetValues(typeof(CertificateField)))
                {
                    err = (ErrorCode)GetCertificateStringField(cout, certificateField, out CertSvcString certSvcString);
                    if (err != ErrorCode.CERTSVC_SUCCESS)
                    {
                        Tizen.Log.Debug(LogTag, "Unable to GetCertificateStringField, , err: " + err + ", instance: "
                        + instance + " gname: " + gname + " store_type: " + storeType);
                        continue;
                    }
                    string stringField = Marshal.PtrToStringUTF8(certSvcString.privateHandler);
                    fields[(int)certificateField] = stringField;
                }
                gotMetadata = true;
                InstanceFree(instance);
            }

        }
        internal struct StoreCertHandle
        {
            string name;
            string title;
            CertStatus status;
            CertStoreType storeType;
        }
        internal struct Certificate
        {
            internal int privateHandler;
            internal Instance Instance;
        }

        internal struct Instance
        {
            internal IntPtr privatePtr;
        }

        internal struct CertSvcString
        {
            internal IntPtr privateHandler;
            internal int privateLength;
            internal Instance Instance;
        }
        /*
        // cert UI:
        Owner:
            Common name:
            Organization:
        Issuer:
            Common name:
            Organization:
        Certificate Information:
            Version:
            Valid from:
            Valid to:
            Serial number:
            Signature algorithm:
            Key Usage:
            Certification authority:
            Public Key:

        */

        internal static List<certificateMetadata> GetCertLists(IntPtr instance, IntPtr certList, int length)
        {
            IntPtr cur = certList;
            var list = new List<certificateMetadata>();
            while (cur != IntPtr.Zero)
            {
                StoreCertList item = Marshal.PtrToStructure<Interop.CertSvc.StoreCertList>(cur);
                certificateMetadata cert = new certificateMetadata()
                {
                    status = item.status,
                    gname = Marshal.PtrToStringUTF8(item.gname),
                    title = Marshal.PtrToStringUTF8(item.title),
                    storeType = item.storeType
                };
                cur = item.next;
                list.Add(cert);
            }
            Tizen.Log.Debug(LogTag, "CertList size: " + list.Count);
            return list;
        }

        // int certsvc_instance_new(CertSvcInstance *instance);
        [DllImport(Libraries.CertSvc, EntryPoint = "certsvc_instance_new")]
        internal static extern int InstanceNew(out IntPtr instance);
        // void certsvc_instance_free(CertSvcInstance instance);
        [DllImport(Libraries.CertSvc, EntryPoint = "certsvc_instance_free")]
        internal static extern void InstanceFree(IntPtr instance);
        // int certsvc_pkcs12_get_certificate_list_from_store(CertSvcInstance instance, CertStoreType storeType, int is_root_app, CertSvcStoreCertList **certlist, size_t *length);
        [DllImport(Libraries.CertSvc, EntryPoint = "certsvc_pkcs12_get_certificate_list_from_store")]
        internal static extern int GetPKCS12CertificateListFromStore(IntPtr instance, CertStoreType storeType, int isRootApp, out IntPtr certList, out int length);
        // int certsvc_pkcs12_free_certificate_list_from_store(CertSvcInstance instance, CertSvcStoreCertList **certlist);
        [DllImport(Libraries.CertSvc, EntryPoint = "certsvc_pkcs12_free_certificate_list_loaded_from_store")]
        internal static extern int FreePKCS12CertificateListFromStore(IntPtr instance, out IntPtr certList);
        // int certsvc_pkcs12_get_certificate_from_store(CertSvcInstance instance, CertStoreType storeType, const char* gname, CertSvcCertificate *certificate);
        [DllImport(Libraries.CertSvc, EntryPoint = "certsvc_pkcs12_get_certificate_from_store")]
        internal static extern int GetPKCS12CertificateFromStore(IntPtr instance, CertStoreType storeType, IntPtr gname, out Certificate certificate);
        // int certsvc_pkcs12_get_certificate_from_store(CertSvcInstance instance, CertStoreType storeType, const char* gname, CertSvcCertificate *certificate);
        [DllImport(Libraries.CertSvc, EntryPoint = "certsvc_pkcs12_get_certificate_from_store")]
        internal static extern int GetPKCS12CertificateFromStore(IntPtr instance, CertStoreType storeType, [MarshalAs(UnmanagedType.LPStr)] string gname, out Certificate certificate);

        // int certsvc_certificate_get_string_field(CertSvcCertificate certificate, CertSvcCertificateField field, CertSvcString *buffer);
        [DllImport(Libraries.CertSvc, EntryPoint = "certsvc_certificate_get_string_field")]
        internal static extern int GetCertificateStringField(Certificate certificate, CertificateField field, out CertSvcString buffer);
        // int certsvc_certificate_get_not_after(CertSvcCertificate certificate, time_t *result);
        [DllImport(Libraries.CertSvc, EntryPoint = "certsvc_certificate_get_not_after")]
        internal static extern int GetCertificateNotAfter(Certificate certificate, out long result);
        // int certsvc_certificate_get_not_before(CertSvcCertificate certificate, time_t *result);
        [DllImport(Libraries.CertSvc, EntryPoint = "certsvc_certificate_get_not_before")]
        internal static extern int GetCertificateNotBefore(Certificate certificate, out long result);
        // int certsvc_certificate_is_root_ca(CertSvcCertificate certificate, int *result);
        [DllImport(Libraries.CertSvc, EntryPoint = "certsvc_certificate_is_root_ca")]
        internal static extern int GetCertificateIsRootCa(Certificate certificate, out int status);

        /* END USER API */
        // int certsvc_pkcs12_get_end_user_certificate_list_from_store(CertSvcInstance instance, CertStoreType storeType, CertSvcStoreCertList **certlist, size_t *length);
        [DllImport(Libraries.CertSvc, EntryPoint = "certsvc_pkcs12_get_end_user_certificate_list_from_store")]
        internal static extern int GetPKCS12UserCertificateListFromStore(IntPtr instance, CertStoreType storeType, out IntPtr certList, out int length);

    }
}