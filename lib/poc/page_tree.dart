import 'package:flutter/material.dart';
import 'package:tizen_fs/models/page_node.dart';

class PageTreeTest extends StatelessWidget {
  final PageNode node;
  const PageTreeTest({super.key, required this.node});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: node.builder?.call(context, node),
    );
  }
}

final PageNode rootNode = PageNode(
  id: 'page_tree',
  title: 'Page Tree Test',
  builder: (context, node) => RootPageWidget(node: node), 
  children: [
    PageNode(
      id: 'page1',
      title: 'Page 1',
      builder: (context, node) => Page1Widget(node: node),
      children: [
        PageNode(
          id: 'page1-1',
          title: 'Page 1-1',
          builder: (context, node) => LeafPageWidget(node: node),
        ),
        PageNode(
          id: 'page1-2',
          title: 'Page 1-2',
          builder: (context, node) => LeafPageWidget(node: node),
        ),
      ],
    ),
    PageNode(
      id: 'page2',
      title: 'Page 2',
      builder: (context, node) => Page2Widget(node: node),
      children: [
        PageNode(
          id: 'page2-1',
          title: 'Page 2-1',
          builder: (context, node) => LeafPageWidget(node: node),
        ),
      ],
    ),
    PageNode(
      id: 'page3',
      title: 'Page 3',
      builder: (context, node) => Page3Widget(node: node),
      children: [
        PageNode(
          id: 'page3-1',
          title: 'Page 3-1',
          builder: (context, node) => LeafPageWidget(node: node),
        ),
      ],
    ),
  ],
);

class RootPageWidget extends StatelessWidget {
  final PageNode node;
  const RootPageWidget({super.key, required this.node});

  @override
  Widget build(BuildContext context) {
    return ListView(
      padding: const EdgeInsets.all(20),
      children: [
        Text(node.title, style: TextStyle(fontSize: 24)),
        const SizedBox(height: 20),
        ...node.children.map((child) => ElevatedButton(
          onPressed: () {
            Navigator.push(context, MaterialPageRoute(builder: (_) => PageTreeTest(node: child)));
          },
          child: Text(child.title),
        )),
      ],
    );
  }
}

class Page1Widget extends StatelessWidget {
  final PageNode node;
  const Page1Widget({super.key, required this.node});

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
                  MaterialPageRoute(builder: (_) => PageTreeTest(node: child)),
                );
              },
              child: Text(child.title),
            )),
      ],
    );
  }
}

class Page2Widget extends StatelessWidget {
  final PageNode node;
  const Page2Widget({super.key, required this.node});

  @override
  Widget build(BuildContext context) {
    return Center(
      child: Column(mainAxisSize: MainAxisSize.min, 
        children: [
          Text(node.title, style: TextStyle(fontSize: 24)),
          const SizedBox(height: 15),
          if (node.children.isNotEmpty)
            ElevatedButton(
              onPressed: () {
                Navigator.push(
                  context,
                  MaterialPageRoute(builder: (_) => PageTreeTest(node: node.children[0])),
                );
              },
              child: Text('Go to ${node.children[0].title}'),
            )
          else
            const Text('No child pages'),
      ]),
    );
  }
}

class Page3Widget extends StatelessWidget {
  final PageNode node;
  const Page3Widget({super.key, required this.node});

  @override
  Widget build(BuildContext context) {
    return Center(
      child: Column(mainAxisSize: MainAxisSize.min, children: [
        const FlutterLogo(size: 100),
        const SizedBox(height: 10),
        Text(node.title, style: TextStyle(fontSize: 24)),
        ...node.children.map((child) => Padding(
              padding: const EdgeInsets.symmetric(vertical: 5),
              child: ElevatedButton(
                onPressed: () {
                  Navigator.push(
                    context,
                    MaterialPageRoute(builder: (_) => PageTreeTest(node: child)),
                  );
                },
                child: Text(child.title),
              ),
            )),
      ]),
    );
  }
}

class LeafPageWidget extends StatelessWidget {
  final PageNode node;

  const LeafPageWidget({super.key, required this.node});

  @override
  Widget build(BuildContext context) {
    return Center(
      child: Text(node.title, style: TextStyle(fontSize: 24)),
    );
  }
}