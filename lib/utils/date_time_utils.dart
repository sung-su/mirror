import 'package:flutter/material.dart';
import 'package:tizen_fs/native/vconf.dart';

class DateTimeUtils {
  static const String VconfAutoDateTimeUpdate = "db/setting/automatic_time_update";
  static const String VconfTimeFormat = "db/menu_widget/regionformat_time1224";
  static const String VconfTimezone = "db/menu_widget/timezone";

  static bool get isAutoUpdated => Vconf.getBool(VconfAutoDateTimeUpdate) ?? false;
  static bool get is24HourFormat => Vconf.getBool(VconfTimeFormat) ?? true;
  static String get timezone => Vconf.getString(VconfTimezone) ?? "Asia/Seoul";
  static DateTime get currentDateTime => DateTime.now();

  static bool setAutoUpdate(bool value) {
    if (isAutoUpdated != value) {
      var ret = Vconf.setBool(VconfAutoDateTimeUpdate, value);
      print("@@ isAutoUpdated set[${value}]=[${ret}]");
      return ret;
    }
    return true;
  }

  static bool setTimeFormat(bool is24Hour) {
    if (is24HourFormat != is24Hour) {
      var ret = Vconf.setBool(VconfTimeFormat, is24Hour);
      print("@@ setTimeFormat set[${is24Hour}]=[${ret}]");
      return ret;
    }
    return true;
  }

  static bool setTimezone(String timezone) {
    if (DateTimeUtils.timezone != timezone) {
      var ret = Vconf.setString(VconfTimezone, timezone);
      print("@@ setTimezone set[${timezone}]=[${ret}]");
      return ret;
    }
    return true;
  }

  static bool setManualDateTime(DateTime dateTime) {
    try {
      // This would typically call native code to set system time
      // For now, we'll just log it
      print("@@ setManualDateTime: ${dateTime.toString()}");
      return true;
    } catch (e) {
      print("Error setting manual datetime: $e");
      return false;
    }
  }

  static String formatTime(DateTime time) {
    if (is24HourFormat) {
      return "${time.hour.toString().padLeft(2, '0')}:${time.minute.toString().padLeft(2, '0')}";
    } else {
      int hour = time.hour % 12;
      if (hour == 0) hour = 12;
      String period = time.hour >= 12 ? "PM" : "AM";
      return "$hour:${time.minute.toString().padLeft(2, '0')} $period";
    }
  }

  static String formatDate(DateTime date) {
    return "${date.year}-${date.month.toString().padLeft(2, '0')}-${date.day.toString().padLeft(2, '0')}";
  }

  static bool get isSupported {
    try {
      return Vconf.getBool(VconfAutoDateTimeUpdate) != null;
    } catch (e) {
      print("DateTimeUtils support check error: $e");
      return false;
    }
  }
}
