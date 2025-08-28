
import 'package:flutter/foundation.dart';
import 'package:tizen_fs/native/application_manager.dart';
import 'package:tizen_fs/models/app_info.dart';
import 'package:tizen_fs/native/package_manager.dart';

class AppInfoModel extends ChangeNotifier {
  List<AppInfo> appInfos = [];
  bool _isLoading = false;
  int _selectedIndex = 0;
  bool _initialized = false;

  AppInfoModel() {
    loadInstalledApps();
  }

  void loadInstalledApps() async {
    if(_initialized) return;

    _isLoading = true;
    appInfos = await ApplicationManager.loadApps();
    _isLoading = false;
    _initialized = true;
    notifyListeners();
  }

  int get selectedIndex => _selectedIndex;
  set selectedIndex(int index) {
    _selectedIndex = index;
    notifyListeners();
  }

  int get itemCount => _isLoading ? 0 : appInfos.length;

  AppInfo getAppInfo(int index) {
    if (_isLoading) {
      return AppInfo(
        appId: 'Loading...',
        name: 'Loading...',
        icon: 'Loading...',
        resourcePath: 'Loading...',
      );
    }
    return appInfos[index];
  }

  AppInfo getSelectedAppInfo() {
    if (_isLoading) {
      return AppInfo(
        appId: 'Loading...',
        name: 'Loading...',
        icon: 'Loading...',
        resourcePath: 'Loading...',
      );
    }
    return appInfos[_selectedIndex];
  }

  void removeApp(AppInfo app) async {
    final ret = await PackageManager.uninstallPackage(app.packageName);
    
    if (ret) {
      appInfos.remove(app);
      notifyListeners();
    } 
  }
}
