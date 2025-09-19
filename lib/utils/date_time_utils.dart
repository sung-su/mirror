import 'package:flutter/foundation.dart';
import 'package:tizen_fs/native/vconf.dart';

class DateTimeUtils {
  static const String vconfAutoDateTimeUpdate = 'db/setting/automatic_time_update';
  static const String vconfTimeFormat = 'db/menu_widget/regionformat_time1224';
  static const String vconfTimezone = 'db/menu_widget/timezone';

  static final ValueNotifier<DateTime> _dateTimeNotifier =
      ValueNotifier<DateTime>(DateTime.now());
  static final ValueNotifier<String> _timezoneNotifier =
      ValueNotifier<String>(Vconf.getString(vconfTimezone) ?? 'Asia/Seoul');
  static final ValueNotifier<bool> _timeFormatNotifier =
      ValueNotifier<bool>(Vconf.getBool(vconfTimeFormat) ?? true);

  static ValueListenable<DateTime> get dateTimeListenable => _dateTimeNotifier;
  static ValueListenable<String> get timezoneListenable => _timezoneNotifier;
  static ValueListenable<bool> get timeFormatListenable => _timeFormatNotifier;

  static bool get isAutoUpdated => Vconf.getBool(vconfAutoDateTimeUpdate) ?? false;
  static bool get is24HourFormat => _timeFormatNotifier.value;
  static String get timezone => _timezoneNotifier.value;
  static DateTime get currentDateTime => _dateTimeNotifier.value;

  static bool setAutoUpdate(bool value) {
    if (isAutoUpdated != value) {
      final ret = Vconf.setBool(vconfAutoDateTimeUpdate, value);
      if (ret) {
        // when auto-update toggles, refresh current time from system
        _dateTimeNotifier.value = DateTime.now();
      }
      return ret;
    }
    return true;
  }

  static bool setTimeFormat(bool is24Hour) {
    if (_timeFormatNotifier.value != is24Hour) {
      final ret = Vconf.setBool(vconfTimeFormat, is24Hour);
      if (ret) {
        _timeFormatNotifier.value = is24Hour;
      }
      return ret;
    }
    return true;
  }

  static bool setTimezone(String timezone) {
    if (_timezoneNotifier.value != timezone) {
      final ret = Vconf.setString(vconfTimezone, timezone);
      if (ret) {
        _timezoneNotifier.value = timezone;
      }
      return ret;
    }
    return true;
  }

  static bool setManualDateTime(DateTime dateTime) {
    try {
      _dateTimeNotifier.value = dateTime;
      return true;
    } catch (e) {
      return false;
    }
  }

  static String formatTime(DateTime time) {
    if (is24HourFormat) {
      return '${time.hour.toString().padLeft(2, '0')}:${time.minute.toString().padLeft(2, '0')}';
    } else {
      int hour = time.hour % 12;
      if (hour == 0) hour = 12;
      final period = time.hour >= 12 ? 'PM' : 'AM';
      return '$hour:${time.minute.toString().padLeft(2, '0')} $period';
    }
  }

  static String formatDate(DateTime date) {
    return '${date.year}-${date.month.toString().padLeft(2, '0')}-${date.day.toString().padLeft(2, '0')}';
  }

  static bool get isSupported {
    try {
      return Vconf.getBool(vconfAutoDateTimeUpdate) != null;
    } catch (e) {
      return false;
    }
  }
}
