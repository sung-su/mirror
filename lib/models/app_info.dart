
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