import 'dart:io';

import 'package:flutter/material.dart';
import 'package:tizen_fs/models/page_node.dart';
import 'package:device_info_plus_tizen/device_info_plus_tizen.dart';
import 'package:tizen_fs/settings/tizenfx.dart';
import 'package:flutter/services.dart' show rootBundle;

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
  late Future<String> _lisence;

  Future<String> loadLicense() async {
    return await rootBundle.loadString('assets/LICENSE');
  }

  Future<String> readLicenseHtml() async {
    return await File('/usr/share/license.html').readAsString();
  }

  String _title = "Open source license";
  double _titleFontSize = 35;
  double _titleHeight = 100;

  @override
  void initState() {
    super.initState();
    _lisence = loadLicense();
  }

  @override
  void didUpdateWidget(var oldWidget) {
    super.didUpdateWidget(oldWidget);
  }

  @override
  Widget build(BuildContext context) {
    final textPainter = TextPainter(
      text: TextSpan(text: _title, style: TextStyle(fontSize: _titleFontSize)),
      textDirection: TextDirection.ltr,
      maxLines: 2,
    )..layout(maxWidth: 240);

    final neededHeight = textPainter.size.height - 25;

    return FutureBuilder<String>(
      future: _lisence,
      builder: (context, snapshot) {
        if (snapshot.connectionState == ConnectionState.done) {
          final info = snapshot.data;
          return Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            spacing: 20,
            children: [
              SizedBox(
                height:
                    widget.isEnabled
                        ? _titleHeight
                        : neededHeight + 50 < _titleHeight
                        ? _titleHeight
                        : _titleHeight + neededHeight,
                width: widget.isEnabled ? 600 : 400,
                child: Padding(
                  padding: EdgeInsets.fromLTRB(80, 60, 80, 0),
                  child: Align(
                    alignment: Alignment.topLeft,
                    child: Text(
                      _title,
                      softWrap: true,
                      overflow: TextOverflow.visible,
                      maxLines: 2,
                      style: TextStyle(fontSize: _titleFontSize),
                    ),
                  ),
                ),
              ),
              Expanded(
                child: SingleChildScrollView (
                  scrollDirection: Axis.vertical,
                  child: Padding(
                    padding:
                      widget.isEnabled
                      ? EdgeInsets.fromLTRB(80, 10, 80, 0)
                      : EdgeInsets.fromLTRB(80, 10, 220, 0),
                    child: Text(
                      info ?? "Unknown",
                      style: TextStyle(fontSize: 11, color: Color(0xFF979AA0)),
                    ),
                  ),
                ),
              ),
            ],
          );
        } else {
          return SizedBox(
            height:
                widget.isEnabled
                    ? _titleHeight
                    : neededHeight + 50 < _titleHeight
                    ? _titleHeight
                    : _titleHeight + neededHeight,
            width: widget.isEnabled ? 600 : 400,
            child: Padding(
              padding: EdgeInsets.fromLTRB(80, 60, 80, 0),
              child: Align(
                alignment: Alignment.topLeft,
                child: Text(
                  _title,
                  softWrap: true,
                  overflow: TextOverflow.visible,
                  maxLines: 2,
                  style: TextStyle(fontSize: _titleFontSize),
                ),
              ),
            ),
          );
        }
      },
    );
  }
}
