import 'dart:ui';

import 'package:flutter/material.dart';
import 'package:tizen_fs/styles/app_style.dart';
import 'package:tizen_fs/widgets/device_list_view.dart';

class CategorySelectableListView extends StatefulWidget {
  const CategorySelectableListView({
    super.key,
    required this.itemSource,
    this.padding,
    this.onSelectionChanged,
    this.alignment,
    this.scrollDirection,
    this.scrollOffset = 300,
  });

  final List<DeviceListItem> itemSource;
  final EdgeInsets? padding;
  final double? alignment;
  final Axis? scrollDirection;
  final Function(int)? onSelectionChanged;
  final double scrollOffset;

  @override
  State<CategorySelectableListView> createState() => CategorySelectableListViewState();
}

class CategorySelectableListViewState extends State<CategorySelectableListView> {
  late final ScrollController _controller;
  late List<GlobalKey> _itemKeys;

  int _selectedIndex = 0;
  Axis _scrollDirection = Axis.horizontal;

  @override
  void initState() {
    super.initState();
    
    _controller = ScrollController();

    if(widget.scrollDirection != null) {
      _scrollDirection = widget.scrollDirection!;
    }

    _itemKeys = List.generate(
      widget.itemSource.length,
      (index) => GlobalKey(),
    );

    debugPrint('init itemcount = $itemCount');
  }

  @override
  void didUpdateWidget(covariant CategorySelectableListView oldWidget) {
    super.didUpdateWidget(oldWidget);
    if (oldWidget.itemSource.length != widget.itemSource.length) {
      _itemKeys = List.generate(
        widget.itemSource.length,
        (index) => GlobalKey(),
      );
    }
  }

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  int get selectedIndex => _selectedIndex;
  set selectedIndex(int index) {
    final int previousIndex = _selectedIndex;
    setState(() {
      _selectedIndex = index;
    });
    _scrollToSelected(_selectedIndex, previousIndex);
  }

  int get itemCount => widget.itemSource.length;

  Future<int> _scrollToSelected(int duration, int fallbackSelection) async {
    debugPrint('_scrollToSelected, $_selectedIndex');

    if (_itemKeys[_selectedIndex].currentContext != null) {
      int current = _selectedIndex;
      final RenderBox box = _itemKeys[_selectedIndex].currentContext!.findRenderObject() as RenderBox;
      final Offset position = box.localToGlobal(Offset.zero);
      if(position.dy.isNaN) return _selectedIndex;

      setState(() {});
      final double offset = widget.scrollOffset;
      widget.onSelectionChanged?.call(_selectedIndex);
      await _controller.animateTo(
        position.dy + _controller.offset - offset,
        duration: $style.times.med,
        curve: Curves.easeInOut,
      );
      return current;
    } else {
      _selectedIndex = fallbackSelection; // restore previous selection
      return fallbackSelection;
    }
  }

  Future<int> selectTo(int index) {
    if (index < 0 || index >= itemCount) {
      throw RangeError('Index out of range: $index');
    }

    if(widget.itemSource[index] != null && widget.itemSource[index].isHeader) {
      index++; 
    }
    final int previousIndex = _selectedIndex;
    _selectedIndex = index;
    return _scrollToSelected(100, previousIndex);
  }

  Future<int> next({bool fast = false}) async {
    debugPrint('### selectable listview next :next=$_selectedIndex, widget.itemCount - 1=${itemCount - 1}');
    int end = widget.itemSource.length - 1;
    if (widget.itemSource[end] != null && widget.itemSource[end].isHeader) {
      end--;
    }
    if (_selectedIndex < end) {
      int previousIndex = _selectedIndex;
      _selectedIndex++;
      if(widget.itemSource[_selectedIndex].isHeader) {
        previousIndex = _selectedIndex;
        _selectedIndex++;
      }
        
      return await _scrollToSelected(fast ? 10 : 100, previousIndex);
    } else {
      return _selectedIndex;
    }
  }

  Future<int> previous({bool fast = false}) async {
    debugPrint('### selectable listview next :previous=$_selectedIndex');
    int start = 0;
    if (widget.itemSource[start] != null && widget.itemSource[start].isHeader) {
      start++;
    }
    if (_selectedIndex > start) {
      int previousIndex = _selectedIndex;
      _selectedIndex--;
      if(widget.itemSource[_selectedIndex].isHeader) {
        previousIndex = _selectedIndex;
        _selectedIndex--;
      }
        
      return await _scrollToSelected(fast ? 10 : 100, previousIndex);
    } else {
      return _selectedIndex;
    }
  }

  @override
  Widget build(BuildContext context) {

    debugPrint('### selectable listview: widget.itemSource.length=${widget.itemSource.length}');
    debugPrint('### selectable listview: itemCount=${itemCount}');

    return ScrollConfiguration(
      behavior: ScrollBehavior().copyWith(
        scrollbars: false,
        overscroll: false,
        dragDevices: {
          PointerDeviceKind.mouse,
          PointerDeviceKind.touch
        }
      ),
      child: ListView.builder(
        padding: widget.padding,
        scrollDirection: _scrollDirection,
        // clipBehavior: Clip.none,
        controller: _controller,
        itemCount: widget.itemSource.length,
        itemBuilder: (context, index) {
          final item = widget.itemSource[index];
          if (item.isHeader) {
            return Text(
              key : _itemKeys[index],
              item.label,
              style: TextStyle(
                fontSize: Focus.of(context).hasFocus && index == selectedIndex ? 20 : 15,
                color:Color.fromARGB(117, 151, 154, 160),
              )
            );
          }
          else {
            return AnimatedScale(
              key : _itemKeys[index],
              scale: Focus.of(context).hasFocus && index == selectedIndex ? 1.0 : .9,
              duration: $style.times.med,
              curve: Curves.easeInOut,
              child: GestureDetector(
                onTap: () {
                  // listKey.currentState?.selectTo(index);
                  selectTo(index);
                  Focus.of(context).requestFocus();
                },
                child: DeviceItemListView(
                  item: widget.itemSource[index],
                  isFocused: Focus.of(context).hasFocus && index == selectedIndex
                  )
              ),
            );
          }
        }
      ),
    );
  }
}
