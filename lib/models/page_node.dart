import 'package:flutter/material.dart';
import 'package:tizen_fs/settings/setting_page_interface.dart';

class PageNode {
  final String id;
  final String title;
  final String? description;
  final IconData? icon;
  final List<PageNode> children;
  final Widget Function(BuildContext context, PageNode node, bool isEnabled, Function(int) itemSelected)? builder;
  final bool isEnd;

  PageNode({
    required this.id,
    required this.title,
    this.description,
    this.icon,
    this.builder,
    this.children = const [],
    this.isEnd = false
  });
}
