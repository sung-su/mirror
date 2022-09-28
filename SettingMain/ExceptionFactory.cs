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
using Tizen.Internals.Errors;

namespace SettingMain
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
}
