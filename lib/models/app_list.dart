import 'package:flutter/material.dart';
import 'package:tizen_fs/models/app_info.dart';

class AppInfoModel extends ChangeNotifier {
  late List<AppInfo> appInfos;
  bool _isLoading = false;
  int _selectedIndex = 0;

  AppInfoModel(this.appInfos);

  AppInfoModel.fromMock(int itemcount) {
    _isLoading = true;
    appInfos = _generateMockContent(itemcount);
    _isLoading = false;
  }

  List<AppInfo> _generateMockContent(int itemcount) {
    return List.generate(
      itemcount,
      (index) => AppInfo(
        appId: '$index',
        name: 'App $index',
        icon: 'icon $index',
        resourcePath: 'resource path $index',
      ),
    );
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

  void removeApp(AppInfo app) {
    notifyListeners();
    appInfos.remove(app);
  }
}
