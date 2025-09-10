import 'package:flutter/material.dart';
import 'package:tizen_fs/models/page_node.dart';

class ProfileActivePage extends StatelessWidget {
  final PageNode node;
  final bool isEnabled;

  const ProfileActivePage({
    super.key,
    required this.node,
    required this.isEnabled,
  });

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 50, horizontal: 50),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        mainAxisAlignment: MainAxisAlignment.end,
        spacing: 10,
        children: [
          Spacer(),
          Text('ⓘ Profile Active', style: TextStyle(fontSize: 14)),
          Padding(
            padding: const EdgeInsets.fromLTRB(20, 0, 0, 0),
            child: Text(
              'Tizen profile is activated',
              style: TextStyle(fontSize: 10),
            ),
          ),
        ],
      ),
    );
  }
}
