import 'package:flutter/material.dart';
import 'package:tizen_fs/models/page_node.dart';
import 'package:tizen_fs/settings/open_source_license_popup.dart';
import 'package:tizen_fs/styles/app_style.dart';
import 'package:tizen_fs/widgets/custom_info_list_view.dart';

class AboutDevicePage extends StatefulWidget {
  const AboutDevicePage({
    super.key,
    required this.node,
    required this.isEnabled,
    required this.onSelectionChanged,
  });

  final PageNode? node;
  final bool isEnabled;
  final Function(int)? onSelectionChanged;

  @override
  State<AboutDevicePage> createState() => AboutDevicePageState();
}

class AboutDevicePageState extends State<AboutDevicePage> {
  GlobalKey<CustomInfoListViewState> _listKey =
      GlobalKey<CustomInfoListViewState>();

  List<CustomInfo> menu = [
    CustomInfo("Device info"),
    CustomInfo("Open source license", popup: OpenSourceLicensePopup()),
    CustomInfo("Manage certificates"),
  ];

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
  void didUpdateWidget(covariant AboutDevicePage oldWidget) {
    super.didUpdateWidget(oldWidget);
    if (widget.isEnabled) {
      initFocus();
    }
  }

  @override
  Widget build(BuildContext context) {
    double titleHeight = 100;
    double titleFontSize = 35;

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
                "About device",
                softWrap: true,
                overflow: TextOverflow.visible,
                maxLines: 2,
                style: TextStyle(fontSize: titleFontSize),
              ),
            ),
          ),
        ),
        if (!widget.node!.children.isEmpty)
          Expanded(
            child: Align(
              alignment: Alignment.topLeft,
              child: AnimatedPadding(
                duration: $style.times.med,
                padding: // item left/right padding
                    widget.isEnabled
                        ? const EdgeInsets.symmetric(
                          horizontal: 80,
                          vertical: 10,
                        )
                        : const EdgeInsets.symmetric(horizontal: 40),
                child: CustomInfoListView(
                  key: _listKey,
                  itemSource: menu,
                  onFocusChanged: (focusd) {
                    print("@ focusd[${focusd}]");
                    widget.onSelectionChanged?.call(focusd);
                  },
                  onSelectionChanged: (selected) {
                    print("@ selected[${selected}]");
                    menu[selected].popup != null
                        ? _showFullScreenPopup(context, menu[selected].popup!)
                        : null;
                  },
                ),
              ),
            ),
          ),
      ],
    );
  }

  void _showFullScreenPopup(BuildContext context, Widget widget) {
    showGeneralDialog(
      context: context,
      barrierDismissible: true,
      barrierLabel: '',
      transitionDuration: const Duration(milliseconds: 80),
      pageBuilder: (context, animation, secondaryAnimation) {
        return widget;
      },
      transitionBuilder: (context, animation, secondaryAnimation, child) {
        return FadeTransition(opacity: animation, child: child);
      },
    );
  }
}
