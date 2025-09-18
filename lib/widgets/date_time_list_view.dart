import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:tizen_fs/styles/app_style.dart';
import 'package:tizen_fs/utils/date_time_utils.dart';
import 'package:tizen_fs/widgets/focus_selectable.dart';
import 'package:tizen_fs/widgets/selectable_listview.dart';

class DateTimeListView extends StatefulWidget {
  const DateTimeListView({
    super.key,
    required this.isEnabled,
    this.onSelectionChanged,
  });

  final bool isEnabled;
  final Function(int)? onSelectionChanged;

  @override
  State<DateTimeListView> createState() => DateTimeListViewState();
}

class DateTimeListViewState extends State<DateTimeListView>
    with FocusSelectable<DateTimeListView> {
  int _selected = 0;

  final List<String> _menuTitles = [
    'Auto update',
    'Set date',
    'Set time',
    'Timezone',
    '24-hour clock',
  ];

  @override
  LogicalKeyboardKey getNextKey() {
    return LogicalKeyboardKey.arrowDown;
  }

  @override
  LogicalKeyboardKey getPrevKey() {
    return LogicalKeyboardKey.arrowUp;
  }

  void initFocus() {
    focusNode.requestFocus();
  }

  void selectTo(int index) {
    listKey.currentState?.selectTo(index);
  }

  @override
  KeyEventResult onKeyEvent(FocusNode focusNode, KeyEvent event) {
    if (event is KeyDownEvent || event is KeyRepeatEvent) {
      if (event.logicalKey == LogicalKeyboardKey.enter ||
          event.logicalKey == LogicalKeyboardKey.select) {

        if (DateTimeUtils.isAutoUpdated && _selected > 0 && _selected != 4) {
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(
              content: Text('Please turn off auto update'),
            ),
          );
          return KeyEventResult.handled;
        }
        _handleMenuItemSelected(_selected);
        return KeyEventResult.handled;
      }
    }
    return KeyEventResult.ignored;
  }

  void _handleMenuItemSelected(int index) {
    switch (index) {
      case 0:
        DateTimeUtils.setAutoUpdate(!DateTimeUtils.isAutoUpdated);
        setState(() {});
        break;
      case 1:
        //
        break;
      case 2:
        //
        break;
      case 3:
        //
        break;
      case 4:
        DateTimeUtils.setTimeFormat(!DateTimeUtils.is24HourFormat);
        setState(() {});
        break;
    }
  }

  @override
  Widget build(BuildContext context) {
    return Focus(
      focusNode: focusNode,
      onFocusChange: (hasfocus) {
        if (hasfocus) {
          listKey.currentState?.selectTo(_selected);
        } else {
          _selected = listKey.currentState?.selectedIndex ?? 0;
        }
      },
      child: SelectableListView(
        scrollOffset: 260,
        key: listKey,
        padding: const EdgeInsets.symmetric(vertical: 10),
        alignment: 0.5,
        itemCount: _menuTitles.length,
        scrollDirection: Axis.vertical,
        onItemFocused: (selected) {
          _selected = selected;
          widget.onSelectionChanged?.call(selected);
        },
        itemBuilder: (context, index, selectedIndex, key) {
          return AnimatedScale(
            key: key,
            scale:
                Focus.of(context).hasFocus && index == selectedIndex ? 1.0 : .9,
            duration: $style.times.med,
            curve: Curves.easeInOut,
            child: GestureDetector(
              onTap: () {
                if (DateTimeUtils.isAutoUpdated && index > 0 && index != 4) {
                  ScaffoldMessenger.of(context).showSnackBar(
                    const SnackBar(
                      content: Text('Please turn off auto update'),
                    ),
                  );
                  return;
                }
                listKey.currentState?.selectTo(index);
                Focus.of(context).requestFocus();
              },
              child: DateTimeMenuItem(
                title: _menuTitles[index],
                index: index,
                isFocused: Focus.of(context).hasFocus && index == selectedIndex,
                isEnabled: widget.isEnabled,
              ),
            ),
          );
        },
      ),
    );
  }
}

class DateTimeMenuItem extends StatelessWidget {
  const DateTimeMenuItem({
    super.key,
    required this.title,
    required this.index,
    required this.isFocused,
    required this.isEnabled,
  });

  final String title;
  final int index;
  final bool isFocused;
  final bool isEnabled;

  final double titleFontSize = 15;
  final double subtitleFontSize = 11;
  final double innerPadding = 20;
  final double itemHeight = 65;

  String _getStatusText() {
    switch (index) {
      case 0:
        if (!DateTimeUtils.isSupported) {
          return 'Not supported';
        } else if (DateTimeUtils.isAutoUpdated) {
          return 'On';
        } else {
          return 'Off';
        }
      case 1:
        return DateTimeUtils.formatDate(DateTimeUtils.currentDateTime);
      case 2:
        return DateTimeUtils.formatTime(DateTimeUtils.currentDateTime);
      case 3:
        return DateTimeUtils.timezone;
      case 4:
        return DateTimeUtils.is24HourFormat ? 'On' : 'Off';
      default:
        return '';
    }
  }

  bool get isItemEnabled {
    if (DateTimeUtils.isAutoUpdated && index > 0 && index != 4) {
      return false;
    }
    return isEnabled;
  }

  Color _getTextColor(BuildContext context) {
    if (!isItemEnabled) {
      return Colors.grey.withOpacity(0.5);
    }
    return isFocused
        ? Theme.of(context).colorScheme.onTertiary
        : Theme.of(context).colorScheme.tertiary;
  }

  Color _getContainerColor(BuildContext context) {
    if (!isItemEnabled) {
      return Colors.grey.withOpacity(0.1);
    }
    return isFocused
        ? Theme.of(context).colorScheme.tertiary
        : Colors.transparent;
  }

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      height: itemHeight,
      child: Container(
        decoration: BoxDecoration(
          borderRadius: BorderRadius.circular(10),
          color: _getContainerColor(context),
        ),
        child: Padding(
          padding: EdgeInsets.symmetric(horizontal: innerPadding),
          child: Row(
            mainAxisAlignment: MainAxisAlignment.start,
            spacing: 15,
            children: [
              Expanded(
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      title,
                      style: TextStyle(
                        fontSize: titleFontSize,
                        color: _getTextColor(context),
                      ),
                    ),
                    Text(
                      _getStatusText(),
                      style: TextStyle(
                        fontSize: subtitleFontSize,
                        color: _getTextColor(context).withOpacity(0.8),
                      ),
                    ),
                  ],
                ),
              ),
              if (index == 0)
                Theme(
                  data: Theme.of(context).copyWith(useMaterial3: false),
                  child: Padding(
                    padding: isFocused
                        ? const EdgeInsets.only(right: 15)
                        : const EdgeInsets.only(right: 0),
                    child: Switch(
                      value: DateTimeUtils.isAutoUpdated,
                      onChanged: (value) => DateTimeUtils.setAutoUpdate(value),
                      activeColor: Colors.blue,
                    ),
                  ),
                ),
              if (index == 4)
                Theme(
                  data: Theme.of(context).copyWith(useMaterial3: false),
                  child: Padding(
                    padding: isFocused
                        ? const EdgeInsets.only(right: 15)
                        : const EdgeInsets.only(right: 0),
                    child: Switch(
                      value: DateTimeUtils.is24HourFormat,
                      onChanged: (value) => DateTimeUtils.setTimeFormat(value),
                      activeColor: Colors.blue,
                    ),
                  ),
                ),
            ],
          ),
        ),
      ),
    );
  }
}
