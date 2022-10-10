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
using Tizen.NUI;
using Tizen.NUI.Components;
using Tizen.NUI.BaseComponents;
using Tizen.Applications;

using SettingAppTextResopurces.TextResources;

namespace SettingMain
{
    class SettingContent_LegalInfo : SettingContent_Base
    {
        const string LegalinfoURL = "file:///usr/share/license.html";

        public SettingContent_LegalInfo()
            : base()
        {
            mTitle = Resources.IDS_ST_BODY_OPEN_SOURCE_LICENCES;
        }


        protected override View CreateContent(Window window)
        {
            Tizen.NUI.BaseComponents.WebView view = new Tizen.NUI.BaseComponents.WebView();

            view.LoadUrl(LegalinfoURL);

            return view;
        }
    }
}
