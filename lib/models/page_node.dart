import 'package:flutter/material.dart';

class PageNode {
  final String id;
  final String title;
  final String? description;
  final IconData? icon;
  final List<PageNode> children;
  final Widget Function(BuildContext context, PageNode node, bool isEnabled)?
  builder;
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
