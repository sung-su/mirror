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
using Tizen.Internals.Errors;
using System.Collections.Generic;
using System.Text;

using System.Runtime.InteropServices;
using static SettingView.Interop.Vconf;

namespace SettingView
{
    /// <summary>
    /// This class provides the API to translate Tizen Error Codes to .NET exceptions.
    /// </summary>
    public static class ExceptionFactory
    {
        /// <summary>
        /// Gets the exception that best corresponds to the given error code.
        /// </summary>
        /// <param name="errorCode">The Tizen Error Code to be translated.</param>
        /// <returns>An exception object.</returns>
        public static Exception GetException(int errorCode)
        {
            var msg = ErrorFacts.GetErrorMessage(errorCode);
            var c = (ErrorCode)errorCode;

            switch (c)
            {
                case ErrorCode.NotSupported:
                    return new NotSupportedException(msg);

                case ErrorCode.OutOfMemory:
                    return new OutOfMemoryException(msg);

                case ErrorCode.InvalidParameter:
                    return new ArgumentException(msg);

                case ErrorCode.InvalidOperation:
                    return new InvalidOperationException(msg);

                case ErrorCode.PermissionDenied:
                    return new UnauthorizedAccessException(msg);

                default:
                    return new Exception(msg);
            }
        }
    }


    /// <summary>
    /// This class provides the API to use Vconf methods.
    /// Vconf is a global configuration registry for the device.
    /// </summary>
    public static class Vconf
    {
        /// <summary>
        /// Delegate for notification callbacks.
        /// </summary>
        /// <param name="node">Pointer to event node.</param>
        /// <param name="userData">Pointer to event user data.</param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void NotificationCallback(IntPtr node, IntPtr userData);

        /// <summary>
        /// Gets a boolean value from Vconf.
        /// </summary>
        /// <param name="key">The key in Vconf.</param>
        /// <returns>A value assigned to the specified key.</returns>
        public static bool GetBool(string key)
        {
            int errorCode = VconfGetBool(key, out bool value);
            if (errorCode != 0)
            {
                throw ExceptionFactory.GetException(errorCode);
            }

            return value;
        }

        /// <summary>
        /// Sets a boolean value in Vconf.
        /// </summary>
        /// <param name="key">The key in Vconf.</param>
        /// <param name="value">The value to be set.</param>
        public static void SetBool(string key, bool value)
        {
            int errorCode = VconfSetBool(key, value);
            if (errorCode != 0)
            {
                throw ExceptionFactory.GetException(errorCode);
            }
        }

        /// <summary>
        /// Gets an integer value from Vconf.
        /// </summary>
        /// <param name="key">The key in Vconf.</param>
        /// <returns>A value assigned to the specified key.</returns>
        public static int GetInt(string key)
        {
            int errorCode = VconfGetInt(key, out int value);
            if (errorCode != 0)
            {
                throw ExceptionFactory.GetException(errorCode);
            }

            return value;
        }

        /// <summary>
        /// Sets an integer value in Vconf.
        /// </summary>
        /// <param name="key">The key in Vconf.</param>
        /// <param name="value">The value to be set.</param>
        public static void SetInt(string key, int value)
        {
            int errorCode = VconfSetInt(key, value);
            if (errorCode != 0)
            {
                throw ExceptionFactory.GetException(errorCode);
            }
        }

        /// <summary>
        /// Gets a string value from Vconf.
        /// </summary>
        /// <param name="key">The key in Vconf.</param>
        /// <returns>A value assigned to the specified key.</returns>
        public static string GetString(string key)
        {
            return VconfGetStr(key);
        }

        /// <summary>
        /// Sets a string value in Vconf.
        /// </summary>
        /// <param name="key">The key in Vconf.</param>
        /// <param name="value">The value to be set.</param>
        public static void SetString(string key, string value)
        {
            int errorCode = VconfSetStr(key, value);
            if (errorCode != 0)
            {
                throw ExceptionFactory.GetException(errorCode);
            }
        }

        /// <summary>
        /// Registers a callback to a KeyChanged event.
        /// </summary>
        /// <param name="key">The key to be observed for changes.</param>
        /// <param name="callback">The callback to be registered.</param>
        /// <param name="userData">Additional data.</param>
        public static void NotifyKeyChanged(string key, NotificationCallback callback, IntPtr? userData = null)
        {
            VconfNotifyKeyChanged(key, callback, userData ?? IntPtr.Zero);
        }

        /// <summary>
        /// Unregisters a callback from a KeyChanged event.
        /// </summary>
        /// <param name="key">The key that was observed for changes.</param>
        /// <param name="callback">The callback to be unregistered.</param>
        public static void IgnoreKeyChanged(string key, NotificationCallback callback)
        {
            VconfIgnoreKeyChanged(key, callback);
        }
    }
}
