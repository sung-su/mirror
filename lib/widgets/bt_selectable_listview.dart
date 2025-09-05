import 'dart:ui';

import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:tizen_fs/models/bt_model.dart';
import 'package:tizen_fs/settings/bt_provider.dart';
import 'package:tizen_fs/styles/app_style.dart';
import 'package:tizen_fs/widgets/bt_list_view.dart';

class CategorySelectableListView extends StatefulWidget {
  const CategorySelectableListView({
    super.key,
    required this.itemSource,
    this.padding,
    this.onSelectionChanged,
    this.onAction,
    this.alignment,
    this.scrollDirection,
    this.scrollOffset = 300,
  });

  final Map<String, Object> itemSource;
  final EdgeInsets? padding;
  final double? alignment;
  final Axis? scrollDirection;
  final Function(int)? onSelectionChanged;
  final Function(int)? onAction;
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

  bool _isEnabled = false;

  @override
  void initState() {
    super.initState();
    
    _controller = ScrollController();

    debugPrint('bt_selectable list : get state start');
    _isEnabled = Provider.of<BtModel>(context, listen: false).isEnabled;
    debugPrint('bt_selectable list : get state end');
    // debugPrint('### bt enable: $_isEnabled');

    if(widget.scrollDirection != null) {
      _scrollDirection = widget.scrollDirection!;
    }

    widget.itemSource.forEach((key, list) {
      // debugPrint('#### ${key} :');
      _flattened.add(_Item(isKey: true, item: key));
      if(list is List) {
        for(var item in list) {
          // debugPrint('######## ${item} ???');
          _flattened.add(_Item(isKey: false, item: item));
        }
      }
    });

    // debugPrint("#### itemcount: ${_flattened.length}");
    _itemCount = _flattened.length;


    _itemKeys = List.generate(
      _flattened.length,
      (index) => GlobalKey(),
    );

    debugPrint('bt_selectable list init done');
  }

  // @override
  // void didUpdateWidget(covariant CategorySelectableListView oldWidget) {
  //   // super.didUpdateWidget(oldWidget);
  //   // if (oldWidget.itemCount != itemCount) {
  //   //   _itemKeys = List.generate(
  //   //     _flattened.length,
  //   //     (index) => GlobalKey(),
  //   //   );
  //   // }
  // }

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

  void onAction(int index) {
    debugPrint('onAction item index=$index');
    if(index == 1) {
      setState(() {
        _isEnabled = !_isEnabled;  
      });
    }
    widget.onAction?.call(index);
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
    debugPrint('bt_selectable list build');
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
        controller: _controller,
        itemCount: itemCount,
        itemBuilder: (context, index) {
          final item = _flattened[index];
          if (item.isKey) {
            return Padding(
              padding: const EdgeInsets.symmetric(vertical: 3, horizontal: 30),
              child: Text(
                key : _itemKeys[index],
                item.item,
                style: TextStyle(
                  fontSize: 10,
                  color:Color.fromARGB(117, 151, 154, 160),
                )
              ),
            );
          }
          else if (index == 1 ) {
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
                child: DeviceListMenuItem(
                  name: _flattened[index].item,
                  isON: _isEnabled,
                  isFocused: Focus.of(context).hasFocus && index == selectedIndex,
                  onStateChanged: (state) {
                    // debugPrint('#### onStateChanged=$state, bt call');
                    if(_isEnabled != state) {
                      onAction(index);
                      
                      // setState(() {
                      //   _isEnabled = state;
                      // });
                      // widget.onAction?.call(index);
                    }
                  },
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
                child: DeviceListItem(
                  item: _flattened[index].item,
                  iconData: Icons.bluetooth,  // isPaired ? Icons.bluetooth_connected : Icons.bluetooth
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

class DeviceListMenuItem extends StatelessWidget {
  const DeviceListMenuItem({super.key, this.name = '', required this.isON, required this.isFocused, this.onStateChanged});

  final String name;
  final bool isON;
  final bool isFocused;
  final void Function(bool)? onStateChanged;


  final double titleFontSize = 15;
  final double subtitleFontSize = 11;
  final double innerPadding = 20;
  final double itemHeight = 65;
  final double iconSize = 25;
  
  @override
  Widget build(BuildContext context) {
    debugPrint('build menu item: isON=$isON');
    return SizedBox(
      height: itemHeight,
      child: Container(
        decoration: BoxDecoration(
          borderRadius: BorderRadius.circular(10),
          color:
              isFocused
                  ? Theme.of(context).colorScheme.tertiary
                  : Colors.transparent,
        ),
        child: Padding(
          padding: // left padding of item inside
              EdgeInsets.symmetric(horizontal: innerPadding),
          child: Row(
            mainAxisAlignment: MainAxisAlignment.start,
            spacing: 15,//innerPadding * 0.75, // between icon-text spacing
            children: [
              Text(
                name,
                style: TextStyle(
                  fontSize: titleFontSize,
                  color:
                      isFocused
                          ? Theme.of(context).colorScheme.onTertiary
                          : Theme.of(context).colorScheme.tertiary,
                ),
              ),
              Spacer(),
              Theme(
                data: Theme.of(
                  context,
                ).copyWith(useMaterial3: false),
                child: Switch(
                  value: isON,
                  activeColor: Colors.blue,
                  onChanged: (value) {
                    onStateChanged?.call(value);
                  }
                )
              ),
            ],
          ),
        ),
      ),
    );
  }
}

class DeviceListItem extends StatelessWidget{
  const DeviceListItem({super.key, required this.item, this.iconData, required this.isFocused});

  final String item;
  final IconData? iconData;
  final bool isFocused;

  final double titleFontSize = 15;
  final double subtitleFontSize = 11;
  final double innerPadding = 20;
  final double itemHeight = 65;
  final double iconSize = 25;

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      height: itemHeight,
      child: Container(
        decoration: BoxDecoration(
          borderRadius: BorderRadius.circular(10),
          color:
              isFocused
                  ? Theme.of(context).colorScheme.tertiary
                  : Colors.transparent,
        ),
        child: Padding(
          padding: // left padding of item inside
              EdgeInsets.symmetric(horizontal: innerPadding),
          child: Row(
            mainAxisAlignment: MainAxisAlignment.start,
            spacing: 15,//innerPadding * 0.75, // between icon-text spacing
            children: [
              Icon(
                iconData,
                size: iconSize,
                color: isFocused
                    ? Color(0xF04285F4)
                    : Color(0xF0AEB2B9),
              ),
              // item text
              Column(
                mainAxisAlignment: MainAxisAlignment.center,
                crossAxisAlignment: CrossAxisAlignment.start,
                spacing: 3,
                children: [
                  Text(
                    item,
                    style: TextStyle(
                      fontSize: titleFontSize,
                      color:
                          isFocused
                              ? Theme.of(context).colorScheme.onTertiary
                              : Theme.of(context).colorScheme.tertiary,
                    ),
                  ),
                  Text(
                    '0:0:0:0',
                    style: TextStyle(
                      fontSize: subtitleFontSize,
                      color:Color(0xFF979AA0),
                    ),
                  )
                ],
              ),
            ],
          ),
        ),
      ),
    );
  }
}
