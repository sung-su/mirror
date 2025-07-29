import 'package:flutter/material.dart';
import 'package:flutter_reorderable_grid_view/widgets/widgets.dart';

class GridTestPage extends StatefulWidget {
  const GridTestPage({Key? key}) : super(key: key);

  @override
  _GridTestPageState createState() => _GridTestPageState();
}

class _GridTestPageState extends State<GridTestPage> {
  final _scrollController = ScrollController();
  final _gridViewKey = GlobalKey();
  
  var _fruits = <String>["apple", "banana", "strawberry", "mango", "blueberry", "peach"];

  @override
  Widget build(BuildContext context) {
    final generatedChildren = List.generate(
      _fruits.length,
      (index) => SizedBox(
        key: Key(_fruits.elementAt(index)),
        width: 200,
        height: 70,
        child: Container(
          color: Colors.pink,
          child: Text(
            _fruits.elementAt(index),
          ),
        ),
      ),
    );

    return Scaffold(
      body: Padding(
        padding: const EdgeInsets.fromLTRB(50, 10, 50, 10),
        child: ReorderableBuilder(
          children: generatedChildren,
          scrollController: _scrollController,
          onReorder: (ReorderedListFunction reorderedListFunction) {
            setState(() {
              _fruits = reorderedListFunction(_fruits) as List<String>;
            });
          },
          builder: (children) {
            return GridView(
              key: _gridViewKey,
              controller: _scrollController,
              children: children,
              gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
                crossAxisCount: 5,
                mainAxisSpacing: 20,
                crossAxisSpacing: 20,
                childAspectRatio: 1.5
              ),
            );
          },
        ),
      ),
    );
  }
}