import 'package:flutter/material.dart';
import 'package:tizen_fs/models/page_node.dart';
import 'package:tizen_fs/styles/app_style.dart';
import 'package:tizen_fs/utils/date_time_utils.dart';
import 'package:tizen_fs/widgets/spinner_list.dart';

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
  late int _year;
  late int _month;
  late int _day;

  late List<int> _years;

  @override
  void initState() {
    super.initState();
    final now = DateTimeUtils.currentDateTime;
    _year = now.year;
    _month = now.month;
    _day = now.day;
    _years = List<int>.generate(131, (i) => 1970 + i); // 1970..2100
  }

  int _daysInMonth(int year, int month) {
    if (month == 12) {
      return DateTime(year + 1, 1, 0).day;
    }
    return DateTime(year, month + 1, 0).day;
  }

  List<String> _buildYearItems() => _years.map((y) => y.toString()).toList();
  List<String> _buildMonthItems() =>
      List<String>.generate(12, (i) => (i + 1).toString().padLeft(2, '0'));
  List<String> _buildDayItems() {
    final maxDay = _daysInMonth(_year, _month);
    return List<String>.generate(maxDay, (i) => (i + 1).toString().padLeft(2, '0'));
  }

  void _submitDate() {
    final now = DateTimeUtils.currentDateTime;
    final newDt = DateTime(_year, _month, _day, now.hour, now.minute);
    DateTimeUtils.setManualDateTime(newDt);
    setState(() {});
  }

  @override
  Widget build(BuildContext context) {
    final yearIndex = _years.indexOf(_year).clamp(0, _years.length - 1);
    final monthIndex = (_month - 1).clamp(0, 11);
    final maxDay = _daysInMonth(_year, _month);
    if (_day > maxDay) _day = maxDay;
    final dayIndex = (_day - 1).clamp(0, maxDay - 1);

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
              padding: widget.isEnabled
                  ? const EdgeInsets.symmetric(horizontal: 80, vertical: 10)
                  : const EdgeInsets.symmetric(horizontal: 40),
              child: AbsorbPointer(
                absorbing: !widget.isEnabled,
                child: Row(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    SpinnerList(
                      label: 'Year',
                      items: _buildYearItems(),
                      initialIndex: yearIndex,
                      onSubmitted: (index) {
                        setState(() {
                          _year = _years[index];
                        });
                        _submitDate();
                      },
                    ),
                    const SizedBox(width: 20),
                    SpinnerList(
                      label: 'Month',
                      items: _buildMonthItems(),
                      initialIndex: monthIndex,
                      onSubmitted: (index) {
                        setState(() {
                          _month = index + 1;
                          final max = _daysInMonth(_year, _month);
                          if (_day > max) _day = max;
                        });
                        _submitDate();
                      },
                    ),
                    const SizedBox(width: 20),
                    SpinnerList(
                      label: 'Day',
                      items: _buildDayItems(),
                      initialIndex: dayIndex,
                      onSubmitted: (index) {
                        setState(() {
                          _day = index + 1;
                        });
                        _submitDate();
                      },
                    ),
                  ],
                ),
              ),
            ),
          ),
        ),
      ],
    );
  }
}

