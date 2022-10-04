/*
 * Copyright (c) 2020 Samsung Electronics Co., Ltd.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Runtime.InteropServices;
using static SettingMain.Vconf;

namespace SettingMain
{
    internal static partial class Interop
    {
        internal static partial class Alarm
        {
            private const string LibraryAlarm = "libalarm.so.0";

            [DllImport(LibraryAlarm, EntryPoint = "alarmmgr_set_systime64")]
            internal static extern int Alarmmgr_SetSystime(Int64 utc_timestamp);

        }
    }
}
