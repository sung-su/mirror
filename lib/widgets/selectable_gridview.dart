import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:tizen_fs/styles/app_style.dart';

class SelectableGridView extends StatefulWidget {
  const SelectableGridView({
    super.key,
    required this.itemCount,
    required this.itemRatio,
    required this.itemBuilder,
    this.padding,
    this.onFocused,
    this.onUnfocused,
    this.onItemSelected
  });

  final int itemCount;
  final double itemRatio;
  final Widget Function(BuildContext, int index, int selectedIndex, Key key) itemBuilder;
  final EdgeInsets? padding;
  final VoidCallback? onFocused;
  final VoidCallback? onUnfocused;
  final Function(int)? onItemSelected;

  @override
  State<SelectableGridView> createState() => SelectableGridViewState();
}

class SelectableGridViewState extends State<SelectableGridView> {
  final FocusNode _focusNode = FocusNode();
  late List<GlobalKey> _itemKeys;

  int _selectedIndex = -1;
  int _lastSelected = 0;

  double _width = 960;
  int get columnCount => (_width < 152) ? 1: (_width - 116) ~/ 162;

  @override
  void initState() {
    super.initState();

    _itemKeys = List.generate(widget.itemCount, (indext) => GlobalKey());

  }

  @override
  void dispose() {
    _focusNode.dispose();
    super.dispose();
  }

  void selectTo(int index) async {
    if (index >= 0 && index < widget.itemCount) {
      int current = await _scrollToSelected(index);
      setState(() {
        if (_selectedIndex != current)
          _selectedIndex = current;  
      });
    }
  }

  Future<int> _scrollToSelected(var index) async {
    final context = _itemKeys[index].currentContext;
    if (context != null) {
      Scrollable.ensureVisible(
        context,
        alignment: 0.1,
        duration: $style.times.fast,
        curve: Curves.easeInOut
      );
      return index;
    }
    else {
      return _selectedIndex;
    }
  }

  KeyEventResult _handleKey(FocusNode node, KeyEvent event) {
    if(event is KeyDownEvent || event is KeyRepeatEvent) {
      int col = _selectedIndex % columnCount;

      if (event.logicalKey == LogicalKeyboardKey.arrowUp) {
        if (_selectedIndex - columnCount >= 0) {
          selectTo(_selectedIndex - columnCount);
          return KeyEventResult.handled;
        }
        else {
          return KeyEventResult.ignored;
        }
      }
      else if (event.logicalKey == LogicalKeyboardKey.arrowDown) {
        if (_selectedIndex + columnCount < widget.itemCount) {
          selectTo(_selectedIndex + columnCount);
          return KeyEventResult.handled;
        } 
      }
      else if (event.logicalKey == LogicalKeyboardKey.arrowRight) {
        if (col < columnCount - 1 && _selectedIndex + 1 < widget.itemCount) {
          selectTo(_selectedIndex + 1);
        }
        return KeyEventResult.handled;
      }
      else if (event.logicalKey == LogicalKeyboardKey.arrowLeft) {
        if (col > 0) {
          selectTo(_selectedIndex - 1);
        }
        return KeyEventResult.handled;
      }
      else if (event.logicalKey == LogicalKeyboardKey.enter || event.logicalKey == LogicalKeyboardKey.select) {
        widget.onItemSelected?.call(_selectedIndex);
        return KeyEventResult.handled;
      }
    }
    return KeyEventResult.ignored;
  }

  void _onFocusChanged(bool hasFocus) {
    if(hasFocus) {
      if(_lastSelected != -1) {
        setState(() {  
          _selectedIndex = _lastSelected;
        });
      }
      widget.onFocused?.call();
    } 
    else {
      _lastSelected = _selectedIndex;
      setState(() {  
        _selectedIndex = -1;
      });
      widget.onUnfocused?.call();
    }
  }

  @override
  Widget build(BuildContext context) {
    return Focus(
      focusNode: _focusNode,
      onKeyEvent: _handleKey,
      onFocusChange: _onFocusChanged,
      child: GridView.builder(
        clipBehavior: Clip.none,
        physics: const NeverScrollableScrollPhysics(),
        padding: widget.padding,
        gridDelegate: SliverGridDelegateWithFixedCrossAxisCount(
          crossAxisCount: columnCount,
          crossAxisSpacing: 10,
          mainAxisSpacing: 20,
          childAspectRatio: widget.itemRatio,
        ),
        itemBuilder: (context, index) {
          return widget.itemBuilder(
            context,
            index,
            _selectedIndex,
            _itemKeys[index],
          );
        },
        itemCount: widget.itemCount,
      ),
    );
  }
}