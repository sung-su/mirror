// Copyright 2022 Samsung Electronics Co., Ltd. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

#include "bluetooth_manager.h"

#include "log.h"

TizenBluetoothManager::TizenBluetoothManager()
{
}

TizenBluetoothManager::~TizenBluetoothManager()
{
}

static void bt_adapter_device_discovery_state_changed_callback(int result, bt_adapter_device_discovery_state_e discovery_state, bt_adapter_device_discovery_info_s *discovery_info, void *user_data)
{
  auto *self = static_cast<TizenBluetoothManager *>(user_data);

  if (result != 0 && self->device_discovery_info_state_changed_callback_ != nullptr)
    return;

  DeviceDiscoveryInfo info = DeviceDiscoveryInfo();
  if (discovery_state == BT_ADAPTER_DEVICE_DISCOVERY_STARTED || discovery_state == BT_ADAPTER_DEVICE_DISCOVERY_FINISHED)
  {
    self->device_discovery_info_state_changed_callback_(result, (int)discovery_state, info);
  }
  else if (discovery_state == BT_ADAPTER_DEVICE_DISCOVERY_FOUND)
  {
    if (discovery_info->remote_name)
      info.remoteName = discovery_info->remote_name;
    if (discovery_info->remote_address)
      info.remoteAddress = discovery_info->remote_address;
    info.btClass = discovery_info->bt_class;
    info.rssi = discovery_info->rssi;
    info.isBonded = discovery_info->is_bonded;
    info.appearance = discovery_info->appearance;
    // char **service_uuid = discovery_info->service_uuid;
    // info.serviceCount = discovery_info->service_count;
    // for (int i = 0; i < discovery_info->service_count; i++)
    // {
    //   serviceUuid.push_back(service_uuid[i]);
    // }
    // info.manufacturerData = strndup(discovery_info->manufacturer_data, discovery_info->manufacturer_data_len);

    self->device_discovery_info_state_changed_callback_(result, (int)discovery_state, info);
  }
}

void TizenBluetoothManager::SetDeviceDiscoveryInfoStateChangedHandler(OnDeviceDiscoveryInfoStateChangedEvent on_event)
{
  device_discovery_info_state_changed_callback_ = on_event;

  int ret = bt_adapter_set_device_discovery_state_changed_cb(bt_adapter_device_discovery_state_changed_callback, this);
  LOG_ERROR("bt_adapter_set_device_discovery_state_changed_cb(): %s", get_error_message(ret));
}