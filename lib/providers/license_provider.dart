import 'package:flutter/material.dart';
import 'package:webview_flutter/webview_flutter.dart';
import 'package:webview_flutter_platform_interface/webview_flutter_platform_interface.dart';
import 'package:webview_flutter_tizen/webview_flutter_tizen.dart';

class LicenseProvider with ChangeNotifier {
  WebViewWidget? _webViewWidget;
  WebViewWidget? get webViewWidget => _webViewWidget;

  bool _isLoading = false;
  bool get isLoading => _isLoading;

  LicenseProvider() {}

  Future<WebViewWidget?> initializeWebView() async {
    if (_webViewWidget != null) {
      return _webViewWidget;
    }

    if (WebViewPlatform.instance == null) {
      WebViewPlatform.instance = TizenWebViewPlatform();
    }

    _isLoading = true;

    try {
      final WebViewController controller = WebViewController();
      controller
        ..setJavaScriptMode(JavaScriptMode.unrestricted)
        ..loadRequest(Uri.parse('file:///usr/share/license.html'));

      _webViewWidget = WebViewWidget(controller: controller);
      return _webViewWidget;
    } catch (e) {
      debugPrint("WebView initialization failed: $e");
      _webViewWidget = null;
      return null;
    } finally {
      _isLoading = false;
    }
  }

  void resetState() {
    _webViewWidget = null;
    _isLoading = false;
  }
}
