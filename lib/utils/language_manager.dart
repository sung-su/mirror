import 'dart:developer' as developer;

import 'package:tizen_fs/native/vconf.dart';

class LanguageManager {
  LanguageManager._();

  static const String _logTag = 'LanguageManager';

  static const String vconfRegionAutomatic = 'db/setting/region_automatic';
  static const String vconfLanguageAutomatic = 'db/setting/lang_automatic';
  static const String vconfWidgetLanguage = 'db/menu_widget/language';

  static final List<DisplayLanguageInfo> _languageList = <DisplayLanguageInfo>[
    const DisplayLanguageInfo(
      locale: 'en_US',
      name: 'English (United States)',
      language: 'English (US)',
      mcc: '310,311,313,316',
    ),
    const DisplayLanguageInfo(
      locale: 'ko_KR',
      name: 'Korean',
      language: 'Korean',
      mcc: '450',
    ),
  ];

  static List<DisplayLanguageInfo> get languageList => List.unmodifiable(_languageList);
  static List<String> get names => _languageList.map((e) => e.name).toList(growable: false);
  static List<String> get locales => _languageList.map((e) => e.locale).toList(growable: false);

  static String get currentLocale => _getDisplayLanguage();
  static int get currentIndex => _getDisplayLanguageIndex();

  static String get currentName {
    final index = currentIndex;
    if (index >= 0) {
      return _languageList[index].name;
    }
    return 'N/A';
  }

  static int get count => _languageList.length;

  static Future<void> applyLanguage(int index) async {
    if (index < 0 || index >= _languageList.length) {
      developer.log('applyLanguage: index out of range ($index)', name: _logTag);
      return;
    }
    final locale = _languageList[index].locale;
    await _setDisplayLanguage(locale);
  }

  static String _getDisplayLanguage() {
    final raw = Vconf.getString(vconfWidgetLanguage);
    if (raw == null || raw.isEmpty) {
      developer.log('Could not get value for $vconfWidgetLanguage', name: _logTag);
      return _languageList.first.locale;
    }
    final tokens = raw.split('.');
    return tokens.isEmpty ? raw : tokens.first;
  }

  static int _getDisplayLanguageIndex() {
    final locale = _getDisplayLanguage();
    return _languageList.indexWhere((info) => info.locale == locale);
  }

  static Future<void> _setDisplayLanguage(String locale) async {
    if (!Vconf.setBool(vconfLanguageAutomatic, false)) {
      developer.log('Failed to set $vconfLanguageAutomatic', name: _logTag);
    }

    if (!Vconf.setString(vconfWidgetLanguage, locale)) {
      developer.log('Failed to set $vconfWidgetLanguage to $locale', name: _logTag);
    }

    final regionAutomatic = Vconf.getBool(vconfRegionAutomatic);
    if (regionAutomatic == true) {
      developer.log(
        'Region automatic is enabled. Additional system setting updates may be required.',
        name: _logTag,
      );
    }
  }
}

class DisplayLanguageInfo {
  const DisplayLanguageInfo({
    required this.locale,
    required this.name,
    required this.language,
    required this.mcc,
  });

  final String locale;
  final String name;
  final String language;
  final String mcc;
}





