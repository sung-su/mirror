import 'package:flutter/foundation.dart';
import 'package:tizen_app_manager/app_manager.dart';
import 'package:tizen_fs/native/app_manager.dart';
import 'package:tizen_fs/models/app_data.dart';
import 'package:tizen_fs/router.dart';

class AppDataModel extends ChangeNotifier {
  List<AppData> appInfos = [];
  bool _isLoading = false;
  int _selectedIndex = 0;
  bool _initialized = false;

  AppDataModel() {
    initialize();
    ApplicationManager.init();
  }

  void initialize() async {
    if (_initialized) return;

    await loadInstalledApps();

    _initialized = true;
  }

  Future<void> loadInstalledApps() async {
    _isLoading = true;

    await _loadApps();

    _isLoading = false;

    notifyListeners();

    if (appInfos.length == 0) {
      AppRouter.router.push(ScreenPaths.main);
    }
  }

  int get selectedIndex => _selectedIndex;
  set selectedIndex(int index) {
    _selectedIndex = index;
    notifyListeners();
  }

  int get itemCount => _isLoading ? 0 : appInfos.length;

  AppData getAppData(int index) {
    if (_isLoading) {
      return AppData(
        appId: 'Loading...',
        name: 'Loading...',
        icon: 'Loading...',
        resourcePath: 'Loading...',
      );
    }
    return appInfos[index];
  }

  AppData getSelectedAppData() {
    if (_isLoading) {
      return AppData(
        appId: 'Loading...',
        name: 'Loading...',
        icon: 'Loading...',
        resourcePath: 'Loading...',
      );
    }
    return appInfos[_selectedIndex];
  }

  Future<void> _loadApps() async {
    appInfos.clear();

    final apps = await AppManager.getInstalledApps();
    for (AppInfo app in apps) {
      if (!app.isNoDisplay && app.appId.contains('hello')) {
        appInfos.add(
          AppData(
            appId: app.appId,
            name: app.label,
            packageName: app.packageId,
            icon: app.iconPath ?? 'assets/images/default_icon.png',
            resourcePath: app.sharedResourcePath,
            appType: app.appType,
          ),
        );
      }
    }
  }

  bool _applaunching = false;
  bool get isAppLaunching => _applaunching;
  set isAppLaunching(bool value) {
    _applaunching = value;
    notifyListeners();
  }

  Future<void> launchApp(String appid) async {
    isAppLaunching = true;

    await ApplicationManager.launch(appid);

    isAppLaunching = false;
  }
}
