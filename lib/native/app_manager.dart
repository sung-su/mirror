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
    await AppControl(
      appId: appId,
      operation: AppControlOperations.defaultOperation,
    ).sendLaunchRequest();
    return true;
  }

  static Future<void> uninstallPackage(String pkgName) async {
    await PackageManager.uninstall(pkgName);
  }
}
