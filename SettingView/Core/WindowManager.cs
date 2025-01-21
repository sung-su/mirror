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

using SettingView.Common;
using Tizen.NUI;
using SettingCore;


namespace SettingView.Core
{
    public static class WindowManager
    {
        private static Window window;

        static WindowManager()
        {
            window = NUIApplication.GetDefaultWindow();
        }

        public static void UpdateWindowPositionSize()
        {
            DeviceInfo.UpdateDeviceInfo();

            int positionX, positionY;
            int width, height;

            float bottomMargin = 0.1f;
            float widthRatio = 0.45f;
            float heightRatio = 0.5f;

            width = (int)(DeviceInfo.DisplayWidth * widthRatio);
            height = (int)(DeviceInfo.DisplayHeight * (1-bottomMargin) * heightRatio);

            positionX = ((DeviceInfo.IsPortrait? DeviceInfo.DisplayHeight : DeviceInfo.DisplayWidth) - width) / 2;
            positionY = ((DeviceInfo.IsPortrait? DeviceInfo.DisplayWidth : DeviceInfo.DisplayHeight) - height) / 2;
            positionY -= (int)((DeviceInfo.IsPortrait ? DeviceInfo.DisplayWidth : DeviceInfo.DisplayHeight) * bottomMargin);

            if (DeviceInfo.IsPortrait)
            {
                (width, height) = (height, width);
                (positionX, positionY) = (positionY, positionX);
            }

            window.WindowPositionSize = new Rectangle(positionX, positionY, width, height);

            Logger.Debug("width is: " + window.WindowSize.Width);
            Logger.Debug("height is: " + window.WindowSize.Height);
            Logger.Debug("position X is: " + window.WindowPosition.X);
            Logger.Debug("position Y is: " + window.WindowPosition.Y);
        }
    }
}
