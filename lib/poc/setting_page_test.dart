
import 'package:flutter/material.dart';
import 'package:tizen_fs/models/page_node.dart';

class SettingPageTest extends StatelessWidget {
  final PageNode node;
  const SettingPageTest({super.key, required this.node});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: node.builder?.call(context, node, false, (_){}),
    );
  }
}