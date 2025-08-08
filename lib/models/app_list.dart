import 'package:flutter/material.dart';

class AppInfo {
  final String appId;
  final String name;
  final String icon;
  
  final String resourcePath;

  AppInfo({
    required this.appId,
    required this.name,
    required this.icon,
    required this.resourcePath,
  });

  static List<AppInfo> generateMockContent() {
    return List.generate(
      5,
      (index) => AppInfo(
        appId: 'appid $index.',
        name: 'App $index',
        icon: 'icon $index',
        resourcePath: 'resource path $index',
      ),
    );
  }
}

class AppInfoModel extends ChangeNotifier {
  late List<AppInfo> appInfos;
  bool _isLoading = false;
  int _selectedIndex = 0;

  AppInfoModel(this.appInfos);

  AppInfoModel.fromMock() {
    _isLoading = true;
    appInfos = AppInfo.generateMockContent();
    _isLoading = false;
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
}
