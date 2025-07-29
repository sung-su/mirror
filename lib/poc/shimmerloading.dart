import 'package:flutter/material.dart';
import 'package:flutter_reorderable_grid_view/widgets/reorderable_builder.dart';

const _shimmerGradient = LinearGradient(
  colors: [Color(0xFFEBEBF4), Color(0xFFF4F4F4), Color(0xFFEBEBF4)],
  stops: [0.1, 0.3, 0.4],
  begin: Alignment(-1.0, -0.3),
  end: Alignment(1.0, 0.3),
  tileMode: TileMode.clamp,
);

class ShimmerLoading extends StatefulWidget {
  const ShimmerLoading({
    super.key,
    required this.isLoading,
    required this.child,
  });

  final bool isLoading;
  final Widget child;

  @override
  State<ShimmerLoading> createState() => _ShimmerLoadingState();
}

class _ShimmerLoadingState extends State<ShimmerLoading> {
  @override
  Widget build(BuildContext context) {
    if (!widget.isLoading) {
      return widget.child;
    }

    return ShaderMask(
      blendMode: BlendMode.srcATop,
      shaderCallback: (bounds) {
        return _shimmerGradient.createShader(bounds);
      },
      child: widget.child,
    );
  }
}

class ShimmerLoadingPage extends StatefulWidget {
  const ShimmerLoadingPage({Key? key}) : super(key: key);

  @override
  _ShimmerLoadingPage createState() => _ShimmerLoadingPage();
}

class _ShimmerLoadingPage extends State<ShimmerLoadingPage> {

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: ShimmerLoading(isLoading: true, child: TestView())
    );
  }
}

class TestView extends StatefulWidget {
  @override
  State<TestView> createState() => _TestViewState();
}

class _TestViewState extends State<TestView> {
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

    // TODO: implement build
    return  Padding(
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
      );
  }
}