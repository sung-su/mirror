import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:tizen_fs/utils/noscroll_focustraversal_policy.dart';

class NoAutoScrollFocusView extends StatefulWidget {
  @override
  State<NoAutoScrollFocusView> createState() => _NoAutoScrollFocusViewState();
}

class _NoAutoScrollFocusViewState extends State<NoAutoScrollFocusView> {
  final _scrollController = ScrollController();
  final _focusNodes = List.generate(10, (_) => FocusNode());

  @override
  void initState() {
    super.initState();

    WidgetsBinding.instance.addPostFrameCallback((_){
      _focusNodes[0].requestFocus();
    });
  }

  @override
  void dispose() {
    for (var node in _focusNodes) {
      node.dispose();
    }
    _scrollController.dispose();
    super.dispose();
  }

  KeyEventResult _handleKey(FocusNode currentNode, KeyEvent event) {
    if (event is KeyDownEvent) {
      final idx = _focusNodes.indexOf(currentNode);
      if (event.logicalKey == LogicalKeyboardKey.arrowDown && idx < _focusNodes.length - 1) {
        _focusNodes[idx + 1].requestFocus();
        return KeyEventResult.handled;
      } else if (event.logicalKey == LogicalKeyboardKey.arrowUp && idx > 0) {
        _focusNodes[idx - 1].requestFocus();
        return KeyEventResult.handled;
      }
    }
    return KeyEventResult.ignored;
  }

  Widget _buildFocusableItem(int index) {
    return Focus(
      focusNode: _focusNodes[index],
      onKeyEvent: _handleKey,
      child: Builder(builder: (context) {
        final hasFocus = Focus.of(context).hasFocus;
        return Container(
          margin: EdgeInsets.symmetric(vertical: 8, horizontal: 16),
          padding: EdgeInsets.all(24),
          decoration: BoxDecoration(
            color: hasFocus ? Colors.grey.shade900 : Colors.grey.shade800,
            border: Border.all(
              color: hasFocus ? Colors.red : Colors.transparent,
              width: 2
            ),
            borderRadius: BorderRadius.circular(12),
          ),
          child: Text('Item $index', style: TextStyle(fontSize: 18)),
        );
      }),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        automaticallyImplyLeading: false,
        backgroundColor: Colors.transparent,
        title: Text("Disabled Auto Scroll on Focus"),
      ),
      body: FocusTraversalGroup(
        policy: NoScrollFocusTraversalPolicy(() => false),
        child: ListView.builder(
          controller: _scrollController,
          itemCount: _focusNodes.length,
          itemBuilder: (_, index) => _buildFocusableItem(index),
        ),
      ),
    );
  }
}