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

using Tizen.NUI;
using Tizen.System;

namespace SettingView.Common
{
    public static class DeviceInfo
    {
        private static int width;
        private static int height;
        private static Window.WindowOrientation orientation;

        static DeviceInfo()
        {
            Size displaySize = NUIApplication.GetScreenSize();
            width = (int)displaySize.Width;
            height = (int)displaySize.Height;

            orientation = NUIApplication.GetDefaultWindow().GetCurrentOrientation();
            IsPortrait = orientation == Window.WindowOrientation.Portrait || orientation == Window.WindowOrientation.PortraitInverse;
        }

        public static void UpdateDeviceInfo()
        {
            Window.WindowOrientation currentOrientation = NUIApplication.GetDefaultWindow().GetCurrentOrientation();
            if (orientation == Window.WindowOrientation.Portrait || orientation == Window.WindowOrientation.PortraitInverse)
            {
                if(currentOrientation == Window.WindowOrientation.Landscape || currentOrientation == Window.WindowOrientation.LandscapeInverse)
                {
                    ToggleOrientation();
                }
            }
            else
            {
                if (currentOrientation == Window.WindowOrientation.Portrait || currentOrientation == Window.WindowOrientation.PortraitInverse)
                {
                    ToggleOrientation();
                }
            }
            orientation = currentOrientation;
        }

        private static void ToggleOrientation()
        {
            (width, height) = (height, width);
            IsPortrait = !IsPortrait;
        }

        public static bool IsPortrait { get; private set; }

        public static int DisplayWidth => width;

        public static int DisplayHeight => height;
    }
}
