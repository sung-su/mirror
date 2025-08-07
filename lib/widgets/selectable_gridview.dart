import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:tizen_fs/styles/app_style.dart';
import 'package:tizen_fs/widgets/media_card.dart';

class SelectableGridView extends StatefulWidget {
  const SelectableGridView({
    super.key,
    required this.itemCount,
    // required this.focusNode,
    // required this.itemBuilder,
    this.padding,
    // this.alignment,
    // this.onSelectionChanged,
    // this.onItemTapped
    this.ScrollController,
    this.onFocused,
    this.onUnfocused
  });

  final ScrollController;
  // final FocusNode focusNode;
  final int itemCount;
  // final Widget Function(BuildContext, int index, int selectedIndex, Key key) itemBuilder;
  final EdgeInsets? padding;
  final VoidCallback? onFocused;
  final VoidCallback? onUnfocused;

  // final double? alignment;
  // final Function(int)? onSelectionChanged;
  // final VoidCallback? onItemTapped;

  @override
  State<SelectableGridView> createState() => SelectableGridViewState();
}

class SelectableGridViewState extends State<SelectableGridView> {
  final FocusNode _focusNode = FocusNode();
  late List<GlobalKey> _itemKeys;

  double _width = 960;
  double _itemWidth = 152;
  double _itemRatio = 16/9;

  double get itemHeight => _itemWidth / _itemRatio + 30;
  // int get widget.itemCount => 52;
  int get columnCount => (_width < 152) ? 1: (_width - 116) ~/ 162;
  int get rowCount => (widget.itemCount % columnCount) > 0 ? (widget.itemCount ~/ columnCount) + 1 : widget.itemCount ~/ columnCount;

  int _selectedIndex = -1;
  int _lastSelected = 0;

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

  void _selectTo(int index) async {
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
        alignment: 0,
        duration: Duration(milliseconds: 100),
        curve: Curves.easeInOut
      );
      return index;
    } else {
      return _selectedIndex;
    }
  }

  KeyEventResult _handleKey(FocusNode node, KeyEvent event) {
    if(event is KeyDownEvent || event is KeyRepeatEvent) {
      int col = _selectedIndex % columnCount;

      if (event.logicalKey == LogicalKeyboardKey.arrowUp) {
        if (_selectedIndex - columnCount >= 0) {
          _selectTo(_selectedIndex - columnCount);
          return KeyEventResult.handled;
        }
        else {
          return KeyEventResult.ignored;
        }
      }
      else if (event.logicalKey == LogicalKeyboardKey.arrowDown) {
        if (_selectedIndex + columnCount < widget.itemCount) {
          _selectTo(_selectedIndex + columnCount);
          return KeyEventResult.handled;
        } 
      }
      else if (event.logicalKey == LogicalKeyboardKey.arrowRight) {
        if (col < columnCount - 1 && _selectedIndex + 1 < widget.itemCount) {
          _selectTo(_selectedIndex + 1);
        }
        return KeyEventResult.handled;
      }
      else if (event.logicalKey == LogicalKeyboardKey.arrowLeft) {
        if (col > 0) {
          _selectTo(_selectedIndex - 1);
        }
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
    return SizedBox(
      width: _width,
      height: (itemHeight * rowCount + 30) < 150 ? 150 : (itemHeight * rowCount + 30),
      child: Container(
        // color: Colors.amber,
        child: Focus(
          focusNode: _focusNode,
          onKeyEvent: _handleKey,
          onFocusChange: _onFocusChanged,
          child: GridView.builder(
            clipBehavior: Clip.none,
            controller: widget.ScrollController,
            padding: widget.padding,
            physics: const ClampingScrollPhysics(),
            gridDelegate: SliverGridDelegateWithFixedCrossAxisCount(
              crossAxisCount: columnCount,
              crossAxisSpacing: 10,
              mainAxisSpacing: 20,
              childAspectRatio: _itemRatio,
            ),
            // TODO
            // itemBuilder: (context, index) {
            //   return widget.itemBuilder(
            //     context,
            //     index,
            //     _selectedIndex,
            //     _itemKeys[index],
            //   );
            // },
            itemBuilder: (context, index) {
              return SizedBox(
                key: _itemKeys[index],
                width: (_width < _itemWidth) ? _width : _itemWidth,
                child: Center(
                  child: MediaCard(
                    width: (_width < _itemWidth) ? _width : _itemWidth,
                    imageUrl: '',
                    content: Container(
                      decoration: BoxDecoration(
                        gradient: $style.gradients.generateLinearGradient(index % 5)
                      ),
                      child: Center(
                        child: Text('App${index+1}'),
                      )
                    ),
                    isSelected: index == _selectedIndex,
                    onRequestSelect: () {
                      _selectTo(index);
                    },
                  ),
                ),
              );
            },
            itemCount: widget.itemCount,
          ),
        ),
      ),
    );
  }
}