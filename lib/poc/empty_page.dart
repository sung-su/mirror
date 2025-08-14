import 'package:flutter/material.dart';
import 'package:tizen_fs/models/page_node.dart';

class EmptyPage extends StatelessWidget {
  final PageNode node;
  final bool isEnabled;
  const EmptyPage({super.key, required this.node, required this.isEnabled});

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Padding(
          padding: isEnabled
                    ? EdgeInsets.fromLTRB(120, 70, 0, 0)
                    : EdgeInsets.fromLTRB(80, 70, 0, 0),
          child: Text(node.title, style: TextStyle(fontSize: 24)),
        ),
      ],
    );
  }
}
