import 'package:flutter/material.dart';

class AppStyle {
  static final AppStyle _instance = AppStyle();
  static AppStyle get instance => _instance;

  final AppColors colors = AppColors();

}

class AppColors {
  final Color primary = Color(0xFF1A1C1E);
  final Color onPrimary = Color.fromARGB(255, 188, 192, 202);
  final Color primaryContainer = Color(0xFF00468A);
  final Color onPrimaryContainer = Color(0xFFD6E3FF);

  final Color secondary = Color(0xFFBDC7DC);
  final Color onSecondary = Color(0xFF273141);
  final Color secondaryContainer = Color(0xFF3E4758);
  final Color onSecondaryContainer = Color(0xFFD9E3F8);

  final Color tertiary = Color(0xFFDCBCE1);
  final Color onTertiary = Color(0xFF3E2845);
  final Color tertiaryContainer = Color(0xFF563E5C);
  final Color onTertiaryContainer = Color(0xFFF9D8FE);

  final Color surface = Color(0xFF1A1C1E);
  final Color onSurface = Color(0xFFE3E2E6);
  final Color surfaceVariant = Color(0xFF43474E);
  final Color onSurfaceVariant = Color(0xFFC4C6CF);

  final Color error = Color(0xFFFFB4AB);
  final Color onError = Color(0xFF690005);
  final Color errorContainer = Color(0xFF93000A);
  final Color onErrorContainer = Color(0xFFFFB4AB);

  final Color border = Color(0xFF8E9099);

  ThemeData toThemeData() {
    /// Create a TextTheme and ColorScheme, that we can use to generate ThemeData
    TextTheme txtTheme = ThemeData.dark().textTheme;
    ColorScheme colorScheme = ColorScheme(
        // Map our custom theme to the Material ColorScheme
        brightness: Brightness.dark,
        primary: primary,
        onPrimary: onPrimary,
        primaryContainer: primaryContainer,
        onPrimaryContainer: onPrimaryContainer,

        secondary: secondary,
        onSecondary: onSecondary,
        secondaryContainer: secondaryContainer,
        onSecondaryContainer: onSecondaryContainer,

        tertiary: tertiary,
        onTertiary: onTertiary,
        tertiaryContainer: tertiaryContainer,
        onTertiaryContainer: onTertiaryContainer,

        surface: surface,
        onSurface: onSurface,
        surfaceContainerHighest: surfaceVariant,
        onSurfaceVariant: onSurfaceVariant,

        error: error,
        onError: onError,
        errorContainer: errorContainer,
        onErrorContainer: onErrorContainer,
        outline: border,
    );
    
    var t = ThemeData.from(textTheme: txtTheme, colorScheme: colorScheme, useMaterial3: true).copyWith(
      textSelectionTheme: TextSelectionThemeData(cursorColor: primary),
      highlightColor: primary,
    );

    /// Return the themeData which MaterialApp can now use
    return t;
  }
}

extension ColorConversion on Color {
  Color withAlphaF(double alpha) {
    return withValues(alpha: alpha);
  }
}