import 'package:flutter/services.dart';

class TizenFx {
  static const MethodChannel _ch = MethodChannel('tizenfx');

  static Future<Map<String, double>> getSystemMemoryUsage() async {
    final m = await _ch.invokeMapMethod<String, double>('getSystemMemoryUsage');
    return Map<String, double>.from(m ?? const {});
  }

  static Future<Map<String, int>> getResolution() async {
    final m = await _ch.invokeMapMethod<String, int>('getResolution');
    return Map<String, int>.from(m ?? const {});
  }

  static Future<Map<String, double>> getAboutDeviceInfo() async {
    final m = await _ch.invokeMapMethod<String, double>('getAboutDeviceInfo');
    return Map<String, double>.from(m ?? const {});
  }
}
