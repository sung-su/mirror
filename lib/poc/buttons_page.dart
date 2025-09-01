
import 'package:flutter/material.dart';
import 'package:tizen_fs/models/page_node.dart';

class ButtonsPage extends StatelessWidget {
  final PageNode node;
  const ButtonsPage({super.key, required this.node});

  @override
  Widget build(BuildContext context) {
    return ListView(
      padding: const EdgeInsets.all(20),
      children: [
        Text(node.title, style: TextStyle(fontSize: 24)),
        const SizedBox(height: 20),
        ...node.children.map((child) => ElevatedButton(
              onPressed: () {
                Navigator.push(
                  context,
                  MaterialPageRoute(builder: (_) => Scaffold(body: child.builder?.call(context, child, false, (_){})))
                );
              },
              child: Text(child.title),
            )),
      ],
    );
  }
}
