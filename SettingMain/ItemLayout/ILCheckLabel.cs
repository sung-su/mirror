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
 *   |  |CHECK |   |   TITLE                                                | |
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
 *   |  |CHECK  |   +-------------------------------------------------------------+ |
 *   |  |       |   +-------------------------------------------------------------+ |-------
 *   |  +-------+   | DESCRIPTION                                                 | |
 *   |              |                                                             | |
 *   |              +-------------------------------------------------------------+ |
 *   +------------------------------------------------------------------------------+
*/

namespace SettingMain
{
    internal class ILCheckLabel : ItemLayout
    {
    }
}
