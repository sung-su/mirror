import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:tizen_fs/providers/backdrop_provider.dart';
import 'mock_item.dart';

class EmptyPage extends StatelessWidget {
  const EmptyPage({super.key});

  @override
  Widget build(BuildContext context) {
    return Container(
        child: Center(
          child: Text(
            'Empty Page',
            style: const TextStyle(color: Colors.white, fontSize: 24)
          )
        )
      );
  }
}

class ListPage extends StatefulWidget {
  const ListPage({super.key, required this.scrollController});

  final ScrollController scrollController;

  @override
  State<ListPage> createState() => _ListPageState();
}

class _ListPageState extends State<ListPage> {

  @override
  void initState() {
    WidgetsBinding.instance.addPostFrameCallback((_) {
      Provider.of<BackdropProvider>(context, listen: false).updateBackdrop(null);
    });
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      child: Column(
        children: [
          MockItem(),
          MockItem(),
          MockItem(),
          MockItem(),
          MockItem(),
          MockItem(),
          MockItem(),
          MockItem(),
          MockItem(),
          MockItem(),
          MockItem(),
          MockItem(),
          MockItem(),
        ]
      )
    );
  }
}