import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:tizen_fs/models/page_node.dart';
import 'package:tizen_fs/styles/app_style.dart';
import 'package:tizen_fs/utils/date_time_utils.dart';
import 'package:tizen_fs/widgets/focus_selectable.dart';
import 'package:tizen_fs/widgets/selectable_listview.dart';

class SetTimezonePage extends StatefulWidget {
  const SetTimezonePage({
    super.key,
    required this.node,
    required this.isEnabled,
  });

  final PageNode? node;
  final bool isEnabled;

  @override
  State<SetTimezonePage> createState() => SetTimezonePageState();
}

class SetTimezonePageState extends State<SetTimezonePage> {
  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      spacing: 10,
      children: [
        // title
        SizedBox(
          width: widget.isEnabled ? 600 : 400,
          child: AnimatedPadding(
            duration: $style.times.med,
            padding: widget.isEnabled
                ? const EdgeInsets.fromLTRB(120, 60, 40, 0)
                : const EdgeInsets.fromLTRB(80, 60, 80, 0),
            child: Align(
              alignment: Alignment.topLeft,
              child: Text(
                widget.node?.title ?? 'Time zone',
                softWrap: true,
                overflow: TextOverflow.visible,
                maxLines: 2,
                style: const TextStyle(fontSize: 35),
              ),
            ),
          ),
        ),
        Expanded(
          child: Align(
            alignment: Alignment.topLeft,
            child: AnimatedPadding(
              duration: $style.times.med,
              padding: widget.isEnabled
                  ? const EdgeInsets.symmetric(horizontal: 80, vertical: 10)
                  : const EdgeInsets.symmetric(horizontal: 40),
              child: AbsorbPointer(
                absorbing: !widget.isEnabled,
                child: const _TimezoneListView(),
              ),
            ),
          ),
        ),
      ],
    );
  }
}

class _TimezoneListView extends StatefulWidget {
  const _TimezoneListView();

  @override
  State<_TimezoneListView> createState() => _TimezoneListViewState();
}

class _TimezoneListViewState extends State<_TimezoneListView>
    with FocusSelectable<_TimezoneListView> {
  late int _selectedIndex;
  late List<String> _timezones;

  @override
  void initState() {
    super.initState();
    _timezones = _buildTimezones();
    _selectedIndex = _timezones.indexOf(DateTimeUtils.timezone);
    if (_selectedIndex < 0) _selectedIndex = 0;
  }

  List<String> _buildTimezones() {
    return const [
      'UTC',
      'Europe/London',
      'Europe/Paris',
      'Europe/Berlin',
      'Europe/Madrid',
      'Europe/Rome',
      'Europe/Moscow',
      'Africa/Cairo',
      'Asia/Jerusalem',
      'Asia/Dubai',
      'Asia/Karachi',
      'Asia/Kolkata',
      'Asia/Dhaka',
      'Asia/Bangkok',
      'Asia/Jakarta',
      'Asia/Shanghai',
      'Asia/Hong_Kong',
      'Asia/Taipei',
      'Asia/Tokyo',
      'Asia/Seoul',
      'Australia/Perth',
      'Australia/Sydney',
      'Pacific/Auckland',
      'America/Halifax',
      'America/New_York',
      'America/Chicago',
      'America/Denver',
      'America/Los_Angeles',
      'America/Anchorage',
      'Pacific/Honolulu',
      'America/Sao_Paulo',
      'America/Mexico_City',
      'America/Bogota',
      'America/Lima',
      'America/Santiago',
    ];
  }

  @override
  LogicalKeyboardKey getNextKey() => LogicalKeyboardKey.arrowDown;
  @override
  LogicalKeyboardKey getPrevKey() => LogicalKeyboardKey.arrowUp;

  void _apply(int index) {
    final tz = _timezones[index];
    if (tz != DateTimeUtils.timezone) {
      DateTimeUtils.setTimezone(tz);
    }
    setState(() {
      _selectedIndex = index;
    });
  }

  @override
  KeyEventResult onKeyEvent(FocusNode focusNode, KeyEvent event) {
    if (event is KeyDownEvent || event is KeyRepeatEvent) {
      if (event.logicalKey == LogicalKeyboardKey.enter ||
          event.logicalKey == LogicalKeyboardKey.select) {
        _apply(selectedIndex);
        return KeyEventResult.handled;
      }
    }
    return KeyEventResult.ignored;
  }

  @override
  Widget build(BuildContext context) {
    return Focus(
      focusNode: focusNode,
      onFocusChange: (hasFocus) {
        if (hasFocus) {
          listKey.currentState?.selectTo(_selectedIndex);
        } else {
          _selectedIndex = listKey.currentState?.selectedIndex ?? _selectedIndex;
        }
      },
      child: SelectableListView(
        key: listKey,
        itemCount: _timezones.length,
        scrollDirection: Axis.vertical,
        scrollOffset: 260,
        padding: const EdgeInsets.symmetric(vertical: 10),
        alignment: 0.5,
        onItemFocused: (i) {
          _selectedIndex = i;
        },
        itemBuilder: (context, index, selectedIndex, key) {
          final bool focused = Focus.of(context).hasFocus && index == selectedIndex;
          final bool checked = _timezones[index] == DateTimeUtils.timezone;
          return AnimatedScale(
            key: key,
            scale: focused ? 1.0 : .9,
            duration: $style.times.med,
            curve: Curves.easeInOut,
            child: GestureDetector(
              onTap: () {
                listKey.currentState?.selectTo(index);
                Focus.of(context).requestFocus();
                _apply(index);
              },
              child: SizedBox(
                height: 65,
                child: Container(
                  decoration: BoxDecoration(
                    borderRadius: BorderRadius.circular(10),
                    color: focused
                        ? Theme.of(context).colorScheme.tertiary
                        : Colors.transparent,
                  ),
                  child: Padding(
                    padding: const EdgeInsets.symmetric(horizontal: 20),
                    child: Row(
                      children: [
                        Expanded(
                          child: Text(
                            _timezones[index],
                            style: TextStyle(
                              fontSize: 15,
                              color: focused
                                  ? Theme.of(context).colorScheme.onTertiary
                                  : Theme.of(context).colorScheme.tertiary,
                            ),
                          ),
                        ),
                        Icon(
                          checked
                              ? Icons.radio_button_checked
                              : Icons.radio_button_unchecked,
                          color: focused
                              ? Theme.of(context).colorScheme.onTertiary
                              : Theme.of(context).colorScheme.tertiary,
                        ),
                      ],
                    ),
                  ),
                ),
              ),
            ),
          );
        },
      ),
    );
  }
}

