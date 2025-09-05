import 'dart:async';

import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:tizen_fs/models/bt_model.dart';
import 'package:tizen_fs/models/page_node.dart';
import 'package:tizen_fs/profiles/profile_popup.dart';
import 'package:tizen_fs/styles/app_style.dart';
import 'package:tizen_fs/widgets/bt_list_view.dart';

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
  GlobalKey<BtDeviceListViewState> _listKey = GlobalKey<BtDeviceListViewState>();

  bool _btEnabled = false;

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
    double titleHeight = 100;
    double titleFontSize = 35;

    return Column(
      spacing: 10,
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        //title
        SizedBox(
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
                child: BtDeviceListView(
                  key: _listKey,
                  onAction: (index) {
                    if (index == 1) {
                      _btEnabled = !_btEnabled;
                      _enableBt(_btEnabled);
                    }
                    else {
                      _showFullScreenPopup(context);
                    }
                  },
                  onSelectionChanged: (selected) {
                    //TODO
                  },
                ),
              ),
            ),
          ),
      ],
    );
  }

  void _enableBt(bool value) {
    if (value) {
      Provider.of<BtModel>(context, listen: false).enable();
    } else {
      Provider.of<BtModel>(context, listen: false).disable();
    } 
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
