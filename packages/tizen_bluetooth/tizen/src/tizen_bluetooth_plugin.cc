#include "tizen_bluetooth_plugin.h"

// For getPlatformVersion; remove unless needed for your plugin implementation.
// #include <system_info.h>
#include <bluetooth.h>

#include <flutter/encodable_value.h>
#include <flutter/event_channel.h>
#include <flutter/event_sink.h>
#include <flutter/event_stream_handler_functions.h>
#include <flutter/method_channel.h>
#include <flutter/plugin_registrar.h>
#include <flutter/standard_method_codec.h>

#include "bluetooth_manager.h"

#include <cstdint>
#include <memory>
#include <string>
#include <vector>

#include "log.h"

namespace
{
  typedef flutter::EventChannel<flutter::EncodableValue> FlEventChannel;
  typedef flutter::EventSink<flutter::EncodableValue> FlEventSink;
  typedef flutter::MethodCall<flutter::EncodableValue> FlMethodCall;
  typedef flutter::MethodResult<flutter::EncodableValue> FlMethodResult;
  typedef flutter::MethodChannel<flutter::EncodableValue> FlMethodChannel;
  typedef flutter::StreamHandler<flutter::EncodableValue> FlStreamHandler;
  typedef flutter::StreamHandlerError<flutter::EncodableValue>
      FlStreamHandlerError;

  class DeviceDiscoveryStateChangeEventStreamHandler : public FlStreamHandler
  {
  public:
    DeviceDiscoveryStateChangeEventStreamHandler() {}

  protected:
    std::unique_ptr<FlStreamHandlerError> OnListenInternal(
        const flutter::EncodableValue *arguments,
        std::unique_ptr<FlEventSink> &&events) override
    {
      events_ = std::move(events);

      OnDeviceDiscoveryInfoStateChangedEvent callback =
          [this](int result, int state, DeviceDiscoveryInfo info)
      {
        flutter::EncodableMap map = {
            {flutter::EncodableValue("result"),
             flutter::EncodableValue(result)},
            {flutter::EncodableValue("state"),
             flutter::EncodableValue(state)},
            {flutter::EncodableValue("remoteAddress"),
             flutter::EncodableValue(info.remoteAddress)},
            {flutter::EncodableValue("remoteName"),
             flutter::EncodableValue(info.remoteName)},
            {flutter::EncodableValue("btClass_majorDeviceClass"),
             flutter::EncodableValue(info.btClass.major_device_class)},
            {flutter::EncodableValue("btClass_minorDeviceClass"),
             flutter::EncodableValue(info.btClass.minor_device_class)},
            {flutter::EncodableValue("btClass_majorServiceClassMask"),
             flutter::EncodableValue(info.btClass.major_service_class_mask)},
            {flutter::EncodableValue("rssi"),
             flutter::EncodableValue(info.rssi)},
            {flutter::EncodableValue("isBonded"),
             flutter::EncodableValue(info.isBonded)},
            {flutter::EncodableValue("appearance"),
             flutter::EncodableValue(info.appearance)},
            {flutter::EncodableValue("serviceCount"),
             flutter::EncodableValue(info.serviceCount)},
            {flutter::EncodableValue("serviceUuid"),
             flutter::EncodableValue([&info]()
                                     {
               flutter::EncodableList serviceUuidList;
               for (const auto& uuid : info.serviceUuid) {
                 serviceUuidList.push_back(flutter::EncodableValue(uuid));
               }
               return serviceUuidList; }())},
            {flutter::EncodableValue("manufacturerData"),
             flutter::EncodableValue(info.manufacturerData)},
            //  serviceUuid: map['serviceUuid'] as List<String>,
            //  manufacturerDataLen: map['manufacturerDataLen'] as int,
            // manufacturerData: map['manufacturerData'] as String,

        };
        events_->Success(flutter::EncodableValue(map));
      };

      TizenBluetoothManager &bluetooth_manager = TizenBluetoothManager::GetInstance();

      bluetooth_manager.SetDeviceDiscoveryInfoStateChangedHandler(callback);

      return nullptr;
    }

    std::unique_ptr<FlStreamHandlerError> OnCancelInternal(
        const flutter::EncodableValue *arguments) override
    {
      TizenBluetoothManager &bluetooth_manager = TizenBluetoothManager::GetInstance();

      bluetooth_manager.SetDeviceDiscoveryInfoStateChangedHandler(nullptr);

      events_.reset();
      return nullptr;
    }

  private:
    std::unique_ptr<FlEventSink> events_;
  };

  class TizenBluetoothPlugin : public flutter::Plugin
  {
  public:
    static void RegisterWithRegistrar(flutter::PluginRegistrar *registrar)
    {
      auto channel =
          std::make_unique<flutter::MethodChannel<flutter::EncodableValue>>(
              registrar->messenger(), "tizen/bluetooth",
              &flutter::StandardMethodCodec::GetInstance());

      auto plugin = std::make_unique<TizenBluetoothPlugin>(registrar);

      channel->SetMethodCallHandler(
          [plugin_pointer = plugin.get()](const auto &call, auto result)
          {
            plugin_pointer->HandleMethodCall(call, std::move(result));
          });

      registrar->AddPlugin(std::move(plugin));
    }

    TizenBluetoothPlugin(flutter::PluginRegistrar *registrar)
        : registrar_(registrar) {}

    virtual ~TizenBluetoothPlugin() {}

  private:
    static void bt_adapter_device_discovery_state_changed_callback(int result, bt_adapter_device_discovery_state_e discovery_state, bt_adapter_device_discovery_info_s *discovery_info, void *user_data)
    {
      if (discovery_state == BT_ADAPTER_DEVICE_DISCOVERY_FOUND && discovery_info)
      {
        if (discovery_info->remote_name)
        {
          LOG_ERROR("device_name: %s", (discovery_info->remote_name));
        }
        else
        {
          LOG_ERROR("device_name: no");
        }
        // if (discovery_info->remote_address)
        // {
        //   LOG_ERROR("device remote_address: %s", discovery_info->remote_address);
        // }
      }
    }
    void HandleMethodCall(
        const flutter::MethodCall<flutter::EncodableValue> &method_call,
        std::unique_ptr<flutter::MethodResult<flutter::EncodableValue>> result)
    {
      const auto &method_name = method_call.method_name();

      if (method_name == "bt_initialize")
      {

        // ret = bt_adapter_set_device_discovery_state_changed_cb(bt_adapter_device_discovery_state_changed_callback, nullptr);
        // LOG_ERROR("bt_adapter_set_device_discovery_state_changed_cb(): %s", get_error_message(ret));

        // ret = bt_adapter_start_device_discovery();

        // LOG_ERROR("bt_adapter_start_device_discovery(): %s", get_error_message(ret));

        result->Success();
      }
      else if (method_name == "bt_deinitialize")
      {
        int ret = bt_deinitialize();
        LOG_ERROR("bt_deinitialize() ret: %s", get_error_message(ret));
        if (ret != BT_ERROR_NONE)
        {
          result->Error(std::string(get_error_message(ret)), "Failed to bt_deinitialize().");
          return;
        }
        result->Success();
      }
      else if (method_name == "init_device_discovery_state_changed_cb")
      {

        device_discovery_state_changed_event_channel_ = std::make_unique<FlEventChannel>(
            registrar_->messenger(), "tizen/bluetooth/device_discovery_state_changed",
            &flutter::StandardMethodCodec::GetInstance());
        device_discovery_state_changed_event_channel_->SetStreamHandler(
            std::make_unique<DeviceDiscoveryStateChangeEventStreamHandler>());

        result->Success();
      }

      // else if (method_name == "bt_enable")
      // {
      //   int ret = bt_deinitialize();
      //   LOG_ERROR("bt_deinitialize() ret: %s", get_error_message(ret));
      //   if (ret != BT_ERROR_NONE)
      //   {
      //     result->Error(std::string(get_error_message(ret)), "Failed to bt_deinitialize().");
      //     return;
      //   }
      //   result->Success();
      // }
      else
      {
        result->NotImplemented();
      }
    }

    std::unique_ptr<FlEventChannel> device_discovery_state_changed_event_channel_;
    flutter::PluginRegistrar *registrar_;
  };

} // namespace

void TizenBluetoothPluginRegisterWithRegistrar(
    FlutterDesktopPluginRegistrarRef registrar)
{
  TizenBluetoothPlugin::RegisterWithRegistrar(
      flutter::PluginRegistrarManager::GetInstance()
          ->GetRegistrar<flutter::PluginRegistrar>(registrar));
}
