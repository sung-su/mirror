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

  final Map<String, Object> itemSource;
  final EdgeInsets? padding;
  final double? alignment;
  final Axis? scrollDirection;
  final Function(int)? onSelectionChanged;
  final double scrollOffset;

  @override
  State<CategorySelectableListView> createState() => CategorySelectableListViewState();
}

class _Item {
  final bool isKey;
  final String item;

  _Item({required this.isKey, required this.item});
}

class CategorySelectableListViewState extends State<CategorySelectableListView> {
  late final ScrollController _controller;
  late List<GlobalKey> _itemKeys;

  int _selectedIndex = 0;
  Axis _scrollDirection = Axis.horizontal;

  final _flattened = <_Item>[];

  int _itemCount = 0;
  int get itemCount => _itemCount;

  @override
  void initState() {
    super.initState();
    
    _controller = ScrollController();

    if(widget.scrollDirection != null) {
      _scrollDirection = widget.scrollDirection!;
    }

    widget.itemSource.forEach((key, list) {
      debugPrint('#### ${key} :');
      _flattened.add(_Item(isKey: true, item: key));
      if(list is List) {
        for(var item in list) {
          debugPrint('######## ${item} ???');
          _flattened.add(_Item(isKey: false, item: item));
        }
      }
    });

    debugPrint("#### itemcount: ${_flattened.length}");
    _itemCount = _flattened.length;

    // _flattened..forEach((key, values) {
    //   _flattened.add(_Item(isKey: true, item: key));
    //   for (final v in values) {
    //     _flattened.add(_Item(isKey: false, item: v));
    //   }
    // });

    _itemKeys = List.generate(
      _flattened.length,
      (index) => GlobalKey(),
    );




        // key와 value를 하나의 List<String>으로 풀어서 연결
    // final flattened = <_Item>[];


  }

  @override
  void didUpdateWidget(covariant CategorySelectableListView oldWidget) {
    // super.didUpdateWidget(oldWidget);
    // if (oldWidget.itemCount != itemCount) {
    //   _itemKeys = List.generate(
    //     _flattened.length,
    //     (index) => GlobalKey(),
    //   );
    // }
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

    if(_flattened[index] != null && _flattened[index].isKey) {
      index++; 
    }
    final int previousIndex = _selectedIndex;
    _selectedIndex = index;
    return _scrollToSelected(100, previousIndex);
  }

  Future<int> next({bool fast = false}) async {
    debugPrint('### selectable listview next :next=$_selectedIndex, widget.itemCount - 1=${itemCount - 1}');
    int end = _flattened.length - 1;
    if (_flattened[end] != null && _flattened[end].isKey) {
      end--;
    }
    if (_selectedIndex < end) {
      int previousIndex = _selectedIndex;
      _selectedIndex++;
      if(_flattened[_selectedIndex].isKey) {
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
    if (_flattened[start] != null && _flattened[start].isKey) {
      start++;
    }
    if (_selectedIndex > start) {
      int previousIndex = _selectedIndex;
      _selectedIndex--;
      if(_flattened[_selectedIndex].isKey) {
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

    debugPrint('### selectable listview: _flattened.length=${_flattened.length}');
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
        itemCount: itemCount,
        itemBuilder: (context, index) {
          final item = _flattened[index];
          if (item.isKey) {
            return Padding(
              padding: const EdgeInsets.symmetric(vertical: 10),
              child: Text(
                key : _itemKeys[index],
                item.item,
                style: TextStyle(
                  fontSize: Focus.of(context).hasFocus && index == selectedIndex ? 20 : 15,
                  color:Color.fromARGB(117, 151, 154, 160),
                )
              ),
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
                  item: _flattened[index].item,
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
