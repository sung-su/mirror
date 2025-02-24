/*
 * Copyright (c) 2016 Samsung Electronics Co., Ltd All Rights Reserved
 *
 * Licensed under the Apache License, Version 2.0 (the License);
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an AS IS BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#ifndef __LOG_H__
#define __LOG_H__

#include <stdio.h>
#include <dlog.h>
#define LOGX(fmt, arg...) \
	({ do { \
		dlog_print(DLOG_INFO, LOG_TAG, fmt, ##arg); \
	} while (0); })

#ifdef  LOG_TAG
#undef  LOG_TAG
#endif
#define LOG_TAG "FLUTTER_RUNNER"

#ifndef _ERR
#define _ERR(fmt, args...) LOGE(fmt "\n", ##args)
#endif

#ifndef _DBG
#define _DBG(fmt, args...) LOGD(fmt "\n", ##args)
#endif

#ifndef _INFO
#define _INFO(fmt, args...) LOGI(fmt "\n", ##args)
#endif

#ifndef _LOGX
#define _LOGX(fmt, args...) LOGX(fmt "\n", ##args)
#endif

#ifndef _SOUT
#define _SOUT(fmt, args...) fprintf(stdout, fmt "\n", ##args)
#endif

#ifndef _SERR
#define _SERR(fmt, args...) fprintf(stderr, fmt "\n", ##args)
#endif

#endif /* __LOG_H__ */
