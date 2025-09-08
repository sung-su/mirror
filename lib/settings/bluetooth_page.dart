import 'package:flutter/material.dart';
import 'package:tizen_fs/models/page_node.dart';
import 'package:tizen_fs/profiles/profile_popup.dart';
import 'package:tizen_fs/styles/app_style.dart';
import 'package:tizen_fs/widgets/device_list_view.dart';
import 'package:tizen_interop/6.0/tizen.dart';

class BluetoothPage extends StatefulWidget {
  const BluetoothPage({
    super.key,
    required this.node,
    required this.isEnabled,
    required this.onSelectionChanged,
  });

  final PageNode? node;
  final bool isEnabled;
  final Function(int)? onSelectionChanged;

  @override
  State<BluetoothPage> createState() => BluetoothPageState();
}

class BluetoothPageState extends State<BluetoothPage> {

  GlobalKey<DeviceListViewState> _listKey = GlobalKey<DeviceListViewState>();
  final categories = {
    "Paired devices ": ["AAA", "BBB", "CCC"],
    "Available devcies": ["QLED", "55\" Neo QLED", "AI Home REference", "MR Music Frame", "43\" Neo QLED"],
  };

  @override
  void initState() {
    super.initState();

    if (widget.isEnabled) {
      WidgetsBinding.instance.addPostFrameCallback((_) {
        initFocus();
      });
    }
  }

  @override
  void initFocus() {
    _listKey.currentState?.initFocus();
  }

  @override
  void didUpdateWidget(covariant BluetoothPage oldWidget) {
    super.didUpdateWidget(oldWidget);
    if (widget.isEnabled) {
      initFocus();
    }
  }

  @override
  Widget build(BuildContext context) {

    // 아이템 카테고리 하나로 합쳐서 
    final List<DeviceListItem> entries = [];
    for (var entry in categories.entries) {
      entries.add(DeviceListItem.header(entry.key));
      for (var item in entry.value) {
        entries.add(DeviceListItem.item(item));
      }
    }

    double titleHeight = 100;
    double titleFontSize = 35;

    return Column(
      spacing: 10,
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        //title
        Container(
          color: Colors.blue,
          child: SizedBox(
            height: titleHeight,
            width: 400,
            child: AnimatedPadding(
              duration: $style.times.med,
              padding: // title up/left padding
                  widget.isEnabled
                      ? EdgeInsets.fromLTRB(120, 60, 40, 0)
                      : EdgeInsets.fromLTRB(80, 60, 80, 0),
              child: Align(
                alignment: Alignment.topLeft,
                child: Text(
                  widget.node?.title ?? '',
                  softWrap: true,
                  overflow: TextOverflow.visible,
                  maxLines: 2,
                  style: TextStyle(fontSize: titleFontSize),
                ),
              ),
            ),
          ),
        ),
        //list
        if (!widget.node!.children.isEmpty)
          Expanded(
            child: Align(
              alignment: Alignment.topLeft,
              child: AnimatedPadding(
              duration: $style.times.med,
                padding: // item left/right padding
                    widget.isEnabled
                        ? const EdgeInsets.symmetric(horizontal: 80, vertical: 10)
                        : const EdgeInsets.symmetric(horizontal: 40),
                child: DeviceListView(
                  key: _listKey,
                  itemSource: entries,
                  onSelectionChanged: (selected) {
                    debugPrint('### selected=$selected');
                    // widget.onSelectionChanged?.call(selected);
                    _showFullScreenPopup(context);

                  },
                ),
              ),
            ),
          ),
      ],
    );
  }
  void _showFullScreenPopup (BuildContext context) {
    showGeneralDialog(
      context: context,
      barrierDismissible: true,
      barrierLabel: '',
      transitionDuration: const Duration(milliseconds: 80),
      pageBuilder: (context, animation, secondaryAnimation) {
        return CreateProfilePopup();
      },
      transitionBuilder: (context, animation, secondaryAnimation, child) {
        return FadeTransition(
          opacity: animation,
          child: child,
        );
      },
    );
  }

}
