import 'package:flutter/material.dart';

class MockItem extends StatefulWidget {
  const MockItem({
    super.key,
    this.onFocus,
  });

  final VoidCallback? onFocus;

  @override
  State<MockItem> createState() => _MockItemState();
}

class _MockItemState extends State<MockItem> {
  final FocusNode _focusNode = FocusNode();

  @override
  void initState() {
    super.initState();
    _focusNode.addListener(_focusChanged);
  }

  void _focusChanged() {
    if (_focusNode.hasFocus) {
      widget.onFocus?.call();
    }
  }

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.fromLTRB(55, 10, 55, 10),
      child: Focus(
          focusNode: _focusNode,
          child: Builder(builder: (buildContext) {
            // create size animation for the container
            return AnimatedContainer(
                duration: const Duration(milliseconds: 100),
                height: Focus.of(buildContext).hasFocus ? 150 : 100,
                decoration: BoxDecoration(
                  color: Focus.of(buildContext).hasFocus
                      ? Colors.purple
                      : Colors.yellow,
                  border: Border.all(color: Colors.black),
                  borderRadius: BorderRadius.circular(10),
                ));
          })),
    );
  }
}