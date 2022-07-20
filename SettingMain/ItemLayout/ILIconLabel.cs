/*
 * Copyright (c) 2017 Samsung Electronics Co., Ltd All Rights Reserved
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Tizen.NUI;
using Tizen.NUI.BaseComponents;

/*
 * The Item Layout has two modes depending on the number of text labels inserted into it.
 * Structure is depicted below.
 *
 *   +------------------------------------------------------------------------+ ------
 *   |                                                                        |        -> Item Margin
 *   |  +------+   +--------------------------------------------------------+ | ------
 *   |  |      |   |                                                        | |
 *   |  | ICON |   |   TITLE                                                | |
 *   |  |      |   |                                                        | |
 *   |  +------+   +--------------------------------------------------------+ |
 *   +------------------------------------------------------------------------+
 *   |             |
 *   |             |
 *   |             |
 *   Text Left Margin
 *   |                                                                      |
 *                               Text Right Margin
 * 
 *   +------------------------------------------------------------------------------+-------
 *   |              +-------------------------------------------------------------+ |
 *   |  +-------+   | TITLE                                                       | |        Description Top Margin
 *   |  |       |   |                                                             | |  
 *   |  | ICON  |   +-------------------------------------------------------------+ |
 *   |  |       |   +-------------------------------------------------------------+ |-------
 *   |  +-------+   | DESCRIPTION                                                 | |
 *   |              |                                                             | |
 *   |              +-------------------------------------------------------------+ |
 *   +------------------------------------------------------------------------------+
*/

namespace SettingMain
{
    /// <summary>
    /// The custom layout sample implementation. This class creates layout for Item Object as it is depicted above.
    /// The custom layout must be derived from LayoutGroup and override the two methods, OnMeasure and OnLayout.
    /// OnMeasure and OnLayout methods must be extended and called during the measuring and layout phases respectively.
    /// </summary>
    internal class ILIconLabel : ItemLayout
    {
    }

}
