import 'package:flutter/material.dart';
import 'package:tizen_fs/models/page_node.dart';

class BluetoothActivePage extends StatelessWidget {
  final PageNode node;
  final bool isEnabled;

  const BluetoothActivePage({super.key, required this.node, required this.isEnabled});

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
          // Text(
          //   'ⓘ Profile Active',
          //   style: TextStyle(
          //     fontSize: 14,
          //   ),
          // ),
          Padding(
            padding: const EdgeInsets.fromLTRB(20,0,0,0),
            child: Text(
              'Your device(Tizen) in currently visible to nearby devcies.',
              style: TextStyle(
                fontSize: 10,
              ),
            ),
          ),
        ],
      ),
    );
  }

}