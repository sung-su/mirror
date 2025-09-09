import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:tizen_fs/models/page_node.dart';
import 'package:tizen_fs/providers/wifi_provider.dart';
import 'package:tizen_fs/styles/app_style.dart';
import 'package:tizen_fs/widgets/wifi_list_view.dart';

class WifiPage extends StatefulWidget {
  const WifiPage({
    super.key,
    required this.node,
    required this.isEnabled,
    required this.onSelectionChanged,
  });

  final PageNode? node;
  final bool isEnabled;
  final Function(int)? onSelectionChanged;

  @override
  State<WifiPage> createState() => WifiPageState();
}

class WifiPageState extends State<WifiPage> {
  final GlobalKey<WifiListViewState> _listKey = GlobalKey<WifiListViewState>();

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
  void didUpdateWidget(covariant WifiPage oldWidget) {
    super.didUpdateWidget(oldWidget);
    if (widget.isEnabled) {
      initFocus();
      final wifiProvider = Provider.of<WifiProvider>(context, listen: false);
      wifiProvider.scanAndRefresh();
    }
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      spacing: 10,
      children: [
        //title
        SizedBox(
          width: widget.isEnabled ? 600 : 400,
          child: AnimatedPadding(
            duration: $style.times.med,
            padding: // title up/left padding
                widget.isEnabled
                    ? EdgeInsets.fromLTRB(120, 60, 40, 0)
                    : EdgeInsets.fromLTRB(80, 60, 80, 0),
            child: Align(
              alignment: Alignment.topLeft,
              child: Text(
                widget.node?.title ?? "",
                softWrap: true,
                overflow: TextOverflow.visible,
                maxLines: 2,
                style: TextStyle(fontSize: 35),
              ),
            ),
          ),
        ),
        Expanded(
          child: Align(
            alignment: Alignment.topLeft,
            child: AnimatedPadding(
              duration: $style.times.med,
              padding: // item left/right padding
                  widget.isEnabled
                      ? const EdgeInsets.symmetric(horizontal: 80, vertical: 10)
                      : const EdgeInsets.symmetric(horizontal: 40),
              child: WifiListView(
                key: _listKey,
                isEnabled: widget.isEnabled,
                onSelectionChanged: (selected) {
                  print("@ selected[${selected}]");
                  widget.onSelectionChanged?.call(selected);
                },
              ),
            ),
          ),
        ),
      ],
    );
  }
}
