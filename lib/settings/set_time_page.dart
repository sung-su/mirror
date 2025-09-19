import 'package:flutter/material.dart';
import 'package:tizen_fs/models/page_node.dart';
import 'package:tizen_fs/styles/app_style.dart';
import 'package:tizen_fs/utils/date_time_utils.dart';
import 'package:tizen_fs/widgets/spinner_list.dart';

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
  late int _hour24; // 0..23
  late int _minute; // 0..59

  @override
  void initState() {
    super.initState();
    final now = DateTimeUtils.currentDateTime;
    _hour24 = now.hour;
    _minute = now.minute;
  }

  List<String> _hours24() =>
      List<String>.generate(24, (i) => i.toString().padLeft(2, '0'));
  List<String> _hours12() =>
      List<String>.generate(12, (i) => (i + 1).toString().padLeft(2, '0'));
  List<String> _minutes() =>
      List<String>.generate(60, (i) => i.toString().padLeft(2, '0'));

  int _hourTo12(int h24) {
    int h = h24 % 12;
    return h == 0 ? 12 : h;
  }

  bool _isPM(int h24) => h24 >= 12;

  void _submitTime() {
    final now = DateTimeUtils.currentDateTime;
    final newDt = DateTime(now.year, now.month, now.day, _hour24, _minute);
    DateTimeUtils.setManualDateTime(newDt);
    setState(() {});
  }

  @override
  Widget build(BuildContext context) {
    final bool is24h = DateTimeUtils.is24HourFormat;
    final int hourIndex24 = _hour24.clamp(0, 23);
    final int hourIndex12 = (_hourTo12(_hour24) - 1).clamp(0, 11);
    final int minuteIndex = _minute.clamp(0, 59);
    final int ampmIndex = _isPM(_hour24) ? 1 : 0;

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
              padding: widget.isEnabled
                  ? const EdgeInsets.symmetric(horizontal: 80, vertical: 10)
                  : const EdgeInsets.symmetric(horizontal: 40),
              child: AbsorbPointer(
                absorbing: !widget.isEnabled,
                child: Row(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    SpinnerList(
                      label: is24h ? 'Hour' : 'Hour',
                      items: is24h ? _hours24() : _hours12(),
                      initialIndex: is24h ? hourIndex24 : hourIndex12,
                      onSubmitted: (index) {
                        setState(() {
                          if (is24h) {
                            _hour24 = index;
                          } else {
                            final ampm = _isPM(_hour24);
                            int base = index + 1; // 1..12
                            if (ampm) {
                              _hour24 = (base % 12) + 12; // PM
                            } else {
                              _hour24 = base % 12; // AM
                            }
                          }
                        });
                        _submitTime();
                      },
                    ),
                    const SizedBox(width: 20),
                    SpinnerList(
                      label: 'Minute',
                      items: _minutes(),
                      initialIndex: minuteIndex,
                      onSubmitted: (index) {
                        setState(() {
                          _minute = index;
                        });
                        _submitTime();
                      },
                    ),
                    if (!is24h) ...[
                      const SizedBox(width: 20),
                      SpinnerList(
                        label: 'AM/PM',
                        items: const ['AM', 'PM'],
                        initialIndex: ampmIndex,
                        onSubmitted: (index) {
                          setState(() {
                            final hour12 = _hourTo12(_hour24);
                            if (index == 0) {
                              // AM
                              _hour24 = hour12 % 12;
                            } else {
                              // PM
                              _hour24 = (hour12 % 12) + 12;
                            }
                          });
                          _submitTime();
                        },
                      ),
                    ],
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

