import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:tizen_fs/models/page_node.dart';
import 'package:tizen_fs/styles/app_style.dart';
import 'package:tizen_fs/utils/date_time_utils.dart';

class SetTimePage extends StatefulWidget {
  const SetTimePage({
    super.key,
    required this.node,
    required this.isEnabled,
  });

  final PageNode? node;
  final bool isEnabled;

  @override
  State<SetTimePage> createState() => SetTimePageState();
}

class SetTimePageState extends State<SetTimePage> {
  late DateTime _selectedDateTime;
  late final VoidCallback _dateTimeListener;
  late final VoidCallback _timeFormatListener;

  @override
  void initState() {
    super.initState();
    _selectedDateTime = DateTimeUtils.currentDateTime;
    _dateTimeListener = _handleExternalDateTimeChanged;
    _timeFormatListener = _handleTimeFormatChanged;
    DateTimeUtils.dateTimeListenable.addListener(_dateTimeListener);
    DateTimeUtils.timeFormatListenable.addListener(_timeFormatListener);
  }

  void _handleExternalDateTimeChanged() {
    if (!mounted) return;
    final dt = DateTimeUtils.currentDateTime;
    if (_selectedDateTime.hour == dt.hour &&
        _selectedDateTime.minute == dt.minute) {
      return;
    }
    setState(() {
      _selectedDateTime = dt;
    });
  }

  void _handleTimeFormatChanged() {
    if (!mounted) return;
    setState(() {
      _selectedDateTime = DateTimeUtils.currentDateTime;
    });
  }

  @override
  void dispose() {
    DateTimeUtils.dateTimeListenable.removeListener(_dateTimeListener);
    DateTimeUtils.timeFormatListenable.removeListener(_timeFormatListener);
    super.dispose();
  }

  void _onTimeChanged(DateTime newTime) {
    final current = DateTimeUtils.currentDateTime;
    final updated = DateTime(
      current.year,
      current.month,
      current.day,
      newTime.hour,
      newTime.minute,
    );
    setState(() {
      _selectedDateTime = updated;
    });
    DateTimeUtils.setManualDateTime(updated);
  }

  @override
  Widget build(BuildContext context) {
    final bool enabled = widget.isEnabled;
    final bool use24h = DateTimeUtils.is24HourFormat;

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      spacing: 10,
      children: [
        SizedBox(
          width: enabled ? 600 : 400,
          child: AnimatedPadding(
            duration: $style.times.med,
            padding: enabled
                ? const EdgeInsets.fromLTRB(120, 60, 40, 0)
                : const EdgeInsets.fromLTRB(80, 60, 80, 0),
            child: Align(
              alignment: Alignment.topLeft,
              child: Text(
                widget.node?.title ?? 'Set time',
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
              padding: enabled
                  ? const EdgeInsets.symmetric(horizontal: 80, vertical: 10)
                  : const EdgeInsets.symmetric(horizontal: 40),
              child: AbsorbPointer(
                absorbing: !enabled,
                child: Container(
                  decoration: BoxDecoration(
                    borderRadius: BorderRadius.circular(16),
                    color: Theme.of(context)
                        .colorScheme
                        .surfaceVariant
                        .withOpacity(enabled ? 0.2 : 0.05),
                  ),
                  padding: const EdgeInsets.symmetric(horizontal: 24),
                  child: CupertinoTheme(
                    data: CupertinoTheme.of(context).copyWith(
                      textTheme: CupertinoTextThemeData(
                        dateTimePickerTextStyle: TextStyle(
                          fontSize: 28,
                          color: enabled
                              ? Theme.of(context).colorScheme.onTertiary
                              : Theme.of(context)
                                  .colorScheme
                                  .tertiary
                                  .withOpacity(0.4),
                        ),
                      ),
                    ),
                    child: CupertinoDatePicker(
                      key: ValueKey<String>(
                        'time-${_selectedDateTime.hour}-${_selectedDateTime.minute}-${use24h ? '24' : '12'}',
                      ),
                      mode: CupertinoDatePickerMode.time,
                      initialDateTime: _selectedDateTime,
                      use24hFormat: use24h,
                      onDateTimeChanged: _onTimeChanged,
                    ),
                  ),
                ),
              ),
            ),
          ),
        ),
      ],
    );
  }
}
