import 'dart:ui';

import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:tizen_fs/models/bt_model.dart';
import 'package:tizen_fs/styles/app_style.dart';

class CategorySelectableListView extends StatefulWidget {
  const CategorySelectableListView({
    super.key,
    this.padding,
    this.onSelectionChanged,
    this.onAction,
    this.alignment,
    this.scrollDirection,
    this.scrollOffset = 300,
  });

  final EdgeInsets? padding;
  final double? alignment;
  final Axis? scrollDirection;
  final Function(int)? onSelectionChanged;
  final Function(int)? onAction;
  final double scrollOffset;

  @override
  State<CategorySelectableListView> createState() => CategorySelectableListViewState();
}

class CategorySelectableListViewState extends State<CategorySelectableListView> {
  late final ScrollController _controller;
  late List<GlobalKey> _itemKeys;

  int _selectedIndex = 0;
  Axis _scrollDirection = Axis.horizontal;

  List<Item> _items = [];

  int _itemCount = 0;
  int get itemCount => _itemCount;

  bool _isEnabled = false;

  @override
  void initState() {
    super.initState();
    
    _controller = ScrollController();

    if(widget.scrollDirection != null) {
      _scrollDirection = widget.scrollDirection!;
    }

    WidgetsBinding.instance.addPostFrameCallback((_) {
      setState(() {
        _isEnabled = Provider.of<BtModel>(context, listen: false).isEnabled;
      });
    });
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
    debugPrint('_scrollToSelected, fallbackSelection=$fallbackSelection, _itemKeys[_selectedIndex].currentContext==null?${_itemKeys[_selectedIndex].currentContext == null}');
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
    if(index == 1) {
      final isBusy = Provider.of<BtModel>(context, listen: false).isBusy;
      debugPrint('onAction : isBusy=$isBusy, _isEnabled=$_isEnabled');
      if(!isBusy) {
        setState(() {
          _isEnabled = !_isEnabled;
        });
      }
    }

    widget.onAction?.call(index);
  }

  void forceScrollTo(int index) {
    _controller.jumpTo((index - 3) * 65);
    WidgetsBinding.instance.addPostFrameCallback((_) async
    {
      await selectTo(index);
    });
  }

  Future<int> selectTo(int index) {
    if (index < 0 || index >= itemCount) {
      throw RangeError('Index out of range: $index');
    }

    if(_items[index] != null && _items[index].isKey) {
      index++; 
    }
    final int previousIndex = _selectedIndex;
    _selectedIndex = index;
    return _scrollToSelected(100, previousIndex);
  }

  Future<int> next({bool fast = false}) async {
    int end = _items.length - 1;
    if (_items[end] != null && _items[end].isKey) {
      end--;
    }
    if (_selectedIndex < end) {
      int previousIndex = _selectedIndex;
      _selectedIndex++;
      if(_items[_selectedIndex].isKey) {
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
    if (_items[start] != null && _items[start].isKey) {
      start++;
    }
    if (_selectedIndex > start) {
      int previousIndex = _selectedIndex;
      _selectedIndex--;
      if(_items[_selectedIndex].isKey) {
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
    _items = Provider.of<BtModel>(context).data;
    
    _itemCount = _items.length;

    _itemKeys = List.generate(
      _items.length,
      (index) => GlobalKey(),
    );

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
          final item = _items[index];
          if (item.isKey) {
            return Padding(
              padding: const EdgeInsets.symmetric(vertical: 3, horizontal: 30),
              child: Text(
                key : _itemKeys[index],
                (item.item as String),
                style: TextStyle(
                  fontSize: 10,
                  color: Color.fromARGB(117, 151, 154, 160),
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
                  name: _items[index].item as String ?? '',
                  isON: _isEnabled,
                  isFocused: Focus.of(context).hasFocus && index == selectedIndex,
                  onStateChanged: (state) {
                    if(_isEnabled != state) {
                      onAction(index);
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
                  item: _items[index].item as BtDevice,
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

class DeviceListMenuItem extends StatefulWidget {
  const DeviceListMenuItem({super.key, this.name = '', required this.isON, required this.isFocused, this.onStateChanged});

  final String name;
  final bool isON;
  final bool isFocused;
  final void Function(bool)? onStateChanged;

  @override
  State<DeviceListMenuItem> createState() => _DeviceListMenuItemState();
}

class _DeviceListMenuItemState extends State<DeviceListMenuItem> {
  final double titleFontSize = 15;
  final double subtitleFontSize = 11;
  final double innerPadding = 20;
  final double itemHeight = 65;
  final double iconSize = 25;

  @override
  Widget build(BuildContext context) {
    final isOperationRunning = context.watch<BtModel>().isBusy;

    return SizedBox(
      height: itemHeight,
      child: Container(
        decoration: BoxDecoration(
          borderRadius: BorderRadius.circular(10),
          color:
              widget.isFocused
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
                widget.name,
                style: TextStyle(
                  fontSize: titleFontSize,
                  color:
                      widget.isFocused
                          ? Theme.of(context).colorScheme.onTertiary
                          : Theme.of(context).colorScheme.tertiary,
                ),
              ),
              Spacer(),
              Theme(
                data: Theme.of(
                  context,
                ).copyWith(useMaterial3: false),
                child: Stack(
                  children: [
                    if(isOperationRunning)
                    SizedBox(
                      width: 30,
                      height: 30,
                      child: CircularProgressIndicator(
                        color: Colors.blue.withAlphaF(0.5),
                      )
                    ),
                    if(!isOperationRunning)
                    Switch(
                      value: widget.isON,
                      activeColor: Colors.blue,
                      onChanged: (value) {
                        widget.onStateChanged?.call(value);
                      }
                    ),
                  ]
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

  final BtDevice item;
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
                    item.remoteName,
                    style: TextStyle(
                      fontSize: titleFontSize,
                      color:
                          isFocused
                              ? Theme.of(context).colorScheme.onTertiary
                              : Theme.of(context).colorScheme.tertiary,
                    ),
                  ),
                  Text(
                    item.isConnected ? 'Connected' : (item.isBonded ? 'Paired' : (item.remoteAddress)) ,
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
