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
using static SettingMain.Vconf;

namespace SettingMain
{
    internal static partial class Interop
    {
        internal static partial class Vconf
        {
            private const string LibraryVconf = "libvconf.so.0";

            [DllImport(LibraryVconf, EntryPoint = "vconf_get_bool")]
            internal static extern int VconfGetBool(string key, out bool val);

            [DllImport(LibraryVconf, EntryPoint = "vconf_set_bool")]
            internal static extern int VconfSetBool(string key, bool intval);

            [DllImport(LibraryVconf, EntryPoint = "vconf_get_int")]
            internal static extern int VconfGetInt(string key, out int val);

            [DllImport(LibraryVconf, EntryPoint = "vconf_set_int")]
            internal static extern int VconfSetInt(string key, int intval);

            [DllImport(LibraryVconf, EntryPoint = "vconf_get_str")]
            internal static extern string VconfGetStr(string key);

            [DllImport(LibraryVconf, EntryPoint = "vconf_set_str")]
            internal static extern int VconfSetStr(string key, string value);

            [DllImport(LibraryVconf, EntryPoint = "vconf_notify_key_changed")]
            internal static extern void VconfNotifyKeyChanged(string key, NotificationCallback callback, IntPtr userData);

            [DllImport(LibraryVconf, EntryPoint = "vconf_ignore_key_changed")]
            internal static extern void VconfIgnoreKeyChanged(string key, NotificationCallback callback);
        }
    }
}
