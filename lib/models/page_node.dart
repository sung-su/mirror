
import 'package:flutter/material.dart';

class PageNode {
  final String id;
  final String title;
  final List<PageNode> children;
  final Widget Function(BuildContext context, PageNode node) builder;

  PageNode({
    required this.id,
    required this.title,
    required this.builder,
    this.children = const [],
  });
}