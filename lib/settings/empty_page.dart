import 'package:flutter/material.dart';
import 'package:tizen_fs/models/page_node.dart';

class EmptyPage extends StatelessWidget {
  final PageNode node;

  const EmptyPage({super.key, required this.node});

  @override
  Widget build(BuildContext context) {
    return Center(
      child: Text(node.title, style: TextStyle(fontSize: 24)),
    );
  }
}