import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:tizen_fs/models/page_node.dart';
import 'package:tizen_fs/providers/license_provider.dart';
import 'package:webview_flutter/webview_flutter.dart';

class OpenSourceLicensePage extends StatefulWidget {
  final PageNode node;
  final bool isEnabled;

  const OpenSourceLicensePage({
    super.key,
    required this.node,
    required this.isEnabled,
  });

  @override
  State<OpenSourceLicensePage> createState() => OpenSourceLicensePageState();
}

class OpenSourceLicensePageState extends State<OpenSourceLicensePage> {
  late final LicenseProvider _licenseProvider;
  Future<WebViewWidget?>? _webViewFuture;

  @override
  void initState() {
    super.initState();
    _licenseProvider = Provider.of<LicenseProvider>(context, listen: false);

    WidgetsBinding.instance.addPostFrameCallback((_) {
      _startWebViewInitialization();
    });
  }

  void _startWebViewInitialization() {
    _licenseProvider.resetState();
    setState(() {
      _webViewFuture = _licenseProvider.initializeWebView();
    });
  }

  @override
  void dispose() {
    _webViewFuture = null;
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Container(
        padding: const EdgeInsets.fromLTRB(80, 60, 80, 60),
        width: widget.isEnabled ? 600 : 400,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          spacing: 20,
          children: [
            Text(
              widget.node.title,
              style: const TextStyle(fontSize: 35),
            ),
            Expanded(
              child: FutureBuilder<WebViewWidget?>(
                future: _webViewFuture,
                builder: (context, snapshot) {
                  if (snapshot.hasData) {
                    return snapshot.data!;
                  } else {
                    return const Center(child: CircularProgressIndicator());
                  }
                },
              ),
            ),
          ],
        ),
      ),
    );
  }
}
