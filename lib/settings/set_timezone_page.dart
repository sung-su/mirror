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
  final GlobalKey<_TimezoneListViewState> _listKey =
      GlobalKey<_TimezoneListViewState>();

  @override
  void initState() {
    super.initState();
    if (widget.isEnabled) {
      _scheduleInitialFocus();
    }
  }

  void _scheduleInitialFocus() {
    void attempt() {
      if (!mounted || !widget.isEnabled) return;
      final state = _listKey.currentState;
      if (state == null) {
        WidgetsBinding.instance.addPostFrameCallback((_) => attempt());
        return;
      }
      state.initFocus();
    }

    WidgetsBinding.instance.addPostFrameCallback((_) => attempt());
  }

  @override
  void didUpdateWidget(covariant SetTimezonePage oldWidget) {
    super.didUpdateWidget(oldWidget);
    if (widget.isEnabled && !oldWidget.isEnabled) {
      _scheduleInitialFocus();
    }
    if (!widget.isEnabled && oldWidget.isEnabled) {
      _listKey.currentState?.releaseFocus();
    }
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      spacing: 10,
      children: [
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
                child: _TimezoneListView(key: _listKey),
              ),
            ),
          ),
        ),
      ],
    );
  }
}

class _TimezoneListView extends StatefulWidget {
  const _TimezoneListView({super.key});

  @override
  State<_TimezoneListView> createState() => _TimezoneListViewState();
}

class _TimezoneListViewState extends State<_TimezoneListView>
    with FocusSelectable<_TimezoneListView> {
  late final List<String> _timezones;
  late final VoidCallback _timezoneListener;

  int _selectedIndex = 0;
  int _focusedIndex = 0;

  @override
  void initState() {
    super.initState();
    _timezones = _buildTimezones();
    _selectedIndex = _resolveCurrentTimezone();
    _focusedIndex = _selectedIndex;
    _timezoneListener = _handleTimezoneUpdated;
    DateTimeUtils.timezoneListenable.addListener(_timezoneListener);
    _ensureSelection(jump: true);
  }

  @override
  void dispose() {
    DateTimeUtils.timezoneListenable.removeListener(_timezoneListener);
    super.dispose();
  }

  void _handleTimezoneUpdated() {
    final idx = _resolveCurrentTimezone();
    if (idx != _selectedIndex) {
      setState(() {
        _selectedIndex = idx;
        _focusedIndex = idx;
      });
      _ensureSelection(jump: true);
    }
  }

  int _resolveCurrentTimezone() {
    final current = DateTimeUtils.timezone;
    final idx = _timezones.indexOf(current);
    return idx >= 0 ? idx : 0;
  }

  void _ensureSelection({bool jump = false}) {
    WidgetsBinding.instance.addPostFrameCallback((_) async {
      if (!mounted) return;
      final state = listKey.currentState;
      if (state == null) {
        _ensureSelection(jump: jump);
        return;
      }
      if (jump) {
        state.selectedIndex = _focusedIndex;
      } else {
        await state.selectTo(_focusedIndex);
      }
      if (state.selectedIndex != _focusedIndex) {
        state.selectedIndex = _focusedIndex;
      }
    });
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

  void initFocus() {
    _selectedIndex = _resolveCurrentTimezone();
    _focusedIndex = _selectedIndex;
    focusNode.requestFocus();
    _ensureSelection(jump: true);
  }

  void releaseFocus() {
    _focusedIndex = _selectedIndex;
  }

  @override
  LogicalKeyboardKey getNextKey() => LogicalKeyboardKey.arrowDown;
  @override
  LogicalKeyboardKey getPrevKey() => LogicalKeyboardKey.arrowUp;

  void _apply(int index) {
    _focusedIndex = index;
    setState(() {
      _selectedIndex = index;
    });
    final tz = _timezones[index];
    if (tz != DateTimeUtils.timezone) {
      DateTimeUtils.setTimezone(tz);
    }
    _ensureSelection();
  }

  @override
  KeyEventResult onKeyEvent(FocusNode focusNode, KeyEvent event) {
    if (event is KeyDownEvent || event is KeyRepeatEvent) {
      if (event.logicalKey == LogicalKeyboardKey.enter ||
          event.logicalKey == LogicalKeyboardKey.select) {
        _apply(_focusedIndex);
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
          _ensureSelection(jump: true);
        } else {
          _focusedIndex = listKey.currentState?.selectedIndex ?? _focusedIndex;
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
          _focusedIndex = i;
        },
        onItemSelected: (i) {
          _focusedIndex = i;
        },
        itemBuilder: (context, index, selectedIndex, key) {
          final bool focused =
              focusNode.hasFocus && index == selectedIndex;
          final bool checked = index == _selectedIndex;
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
