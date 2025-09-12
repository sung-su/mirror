import 'dart:async';
import 'package:flutter/foundation.dart';
import 'package:tizen_app_control/tizen_app_control.dart';
import 'package:tizen_fs/locator.dart';
import 'package:tizen_fs/models/app_data_model.dart';
import 'package:tizen_package_manager/tizen_package_manager.dart';

class AppControlOperations {
  static String defaultOperation =
      'http://tizen.org/apcontrol/operation/default';
}

class ApplicationManager {
  static void init() {
    PackageManager.onInstallProgressChanged.listen((event) {
      if (event.progress == 100) {
        getIt<AppDataModel>().loadInstalledApps();
      }
    });

    PackageManager.onUninstallProgressChanged.listen((event) {
      if (event.progress == 100) {
        getIt<AppDataModel>().loadInstalledApps();
      }
    });
  }

  static Future<bool> launch(String appId) async {
    final completer = Completer<bool>();
    final timer = Timer(const Duration(seconds: 5), () {
      if (!completer.isCompleted) {
        debugPrint(
          'AppControl reply not received within timeout - appId: $appId',
        );
        completer.complete(false);
      }
    });

    try {
      await AppControl(
        appId: appId,
        operation: AppControlOperations.defaultOperation,
      ).sendLaunchRequest();
      debugPrint('AppControl send succeeded.');
      completer.complete(true);
    } catch (e) {
      if (!completer.isCompleted) {
        debugPrint('AppControl send failed: $e');
        completer.complete(false);
      }
    }

    final result = await completer.future;
    timer.cancel();
    return result;
  }

  static Future<void> uninstallPackage(String pkgName) async {
    await PackageManager.uninstall(pkgName);
  }
}
