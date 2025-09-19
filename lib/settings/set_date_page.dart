import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:tizen_fs/models/page_node.dart';
import 'package:tizen_fs/styles/app_style.dart';
import 'package:tizen_fs/utils/date_time_utils.dart';

class SetDatePage extends StatefulWidget {
  const SetDatePage({
    super.key,
    required this.node,
    required this.isEnabled,
  });

  final PageNode? node;
  final bool isEnabled;

  @override
  State<SetDatePage> createState() => SetDatePageState();
}

class SetDatePageState extends State<SetDatePage> {
  late DateTime _selectedDate;
  late final VoidCallback _dateTimeListener;

  @override
  void initState() {
    super.initState();
    _selectedDate = DateTimeUtils.currentDateTime;
    _dateTimeListener = _handleExternalDateChanged;
    DateTimeUtils.dateTimeListenable.addListener(_dateTimeListener);
  }

  void _handleExternalDateChanged() {
    if (!mounted) return;
    final dt = DateTimeUtils.currentDateTime;
    if (_selectedDate.year == dt.year &&
        _selectedDate.month == dt.month &&
        _selectedDate.day == dt.day) {
      return;
    }
    setState(() {
      _selectedDate = dt;
    });
  }

  @override
  void dispose() {
    DateTimeUtils.dateTimeListenable.removeListener(_dateTimeListener);
    super.dispose();
  }

  void _onDateChanged(DateTime newDate) {
    final current = DateTimeUtils.currentDateTime;
    final updated = DateTime(
      newDate.year,
      newDate.month,
      newDate.day,
      current.hour,
      current.minute,
    );
    setState(() {
      _selectedDate = updated;
    });
    DateTimeUtils.setManualDateTime(updated);
  }

  @override
  Widget build(BuildContext context) {
    final bool enabled = widget.isEnabled;
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
                widget.node?.title ?? 'Set date',
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
                        'date-${_selectedDate.year}-${_selectedDate.month}-${_selectedDate.day}',
                      ),
                      mode: CupertinoDatePickerMode.date,
                      initialDateTime: _selectedDate,
                      minimumYear: 1970,
                      maximumYear: 2100,
                      use24hFormat: DateTimeUtils.is24HourFormat,
                      onDateTimeChanged: _onDateChanged,
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
