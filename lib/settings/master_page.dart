import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:tizen_fs/screen/mock_apps_page.dart';
import 'package:tizen_fs/settings/menu.dart';
import 'package:tizen_fs/utils/noscroll_focustraversal_policy.dart';
import 'package:tizen_fs/widgets/selectable_listview.dart';

class MasterPage extends StatefulWidget {
  final String title;
  // final List<MenuItem> items;
  // final int itemCount;
  // final Function(int)? onFocusedItemChanged;
  final bool isEnabled;
  // final int index;

  MasterPage({
    super.key,
    this.title = "",
    // required this.items,
    // required this.itemCount,
    // this.onFocusedItemChanged,
    required this.isEnabled,
    // required this.index,
  });

  @override
  State<MasterPage> createState() => MasterPageState();
}

class MasterPageState extends State<MasterPage> {
  double titleWidth = 300;
  double titleHeight = 50;
  double itemWidth = 300;
  double itemHeight = 50;
  static const double titleFontSize = 32;
  static const double itemFontSize = 16;
  double innerPadding = 8;

  @override
  void initState() {
    super.initState();
  }

  @override
  void didUpdateWidget(covariant MasterPage oldWidget) {
    super.didUpdateWidget(oldWidget);
  }

  @override
  void dispose() {
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return AnimatedContainer(
      duration: Duration(milliseconds: 100),
      child: Container(
        color: widget.isEnabled ? Colors.grey[900] : Colors.grey[800],
        padding: EdgeInsets.fromLTRB(90, 90, 90, 0),
        child: Column(children: [
          Text(widget.title),
          MockList(
            isHorizontal: false,
          ),
        ]),
      ),
    );
  }
}
