import 'package:flutter/material.dart';
import 'package:tizen_fs/models/page_node.dart';
import 'package:tizen_fs/styles/app_style.dart';
import 'package:tizen_fs/widgets/setting_list_view.dart';

class SettingPage extends StatefulWidget {
  const SettingPage({
    super.key,
    required this.node,
    required this.isEnabled,
    required this.onSelectionChanged,
  });

  final PageNode? node;
  final bool isEnabled;
  final Function(int)? onSelectionChanged;

  @override
  State<SettingPage> createState() => SettingPageState();
}

class SettingPageState extends State<SettingPage>
    with AutomaticKeepAliveClientMixin {
  GlobalKey<SettingListViewState> _listKey = GlobalKey<SettingListViewState>();

  double _opacity = 0;

  @override
  bool get wantKeepAlive => widget.isEnabled;

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      setState(() {
        _opacity = widget.isEnabled ? 1.0 : 0.7;
      });
    });
    if (widget.isEnabled) {
      WidgetsBinding.instance.addPostFrameCallback((_) {
        initFocus();
      });
    }
  }

  void initFocus() {
    _listKey.currentState?.initFocus();
  }

  void hidePage() {
    setState(() {
      _opacity = 0;
    });
  }

  @override
  void didUpdateWidget(covariant SettingPage oldWidget) {
    super.didUpdateWidget(oldWidget);
    if (widget.isEnabled) {
      initFocus();
    }
    setState(() {
      _opacity = widget.isEnabled ? 1.0 : 0.7;
    });
  }

  @override
  Widget build(BuildContext context) {
    super.build(context);

    if (widget.node == null) {
      return Container(color: Theme.of(context).colorScheme.onTertiary);
    }

    if (widget.node!.builder != null) {
      return Container(
        color:
            widget.isEnabled
                ? Theme.of(context).colorScheme.surface
                : Theme.of(context).colorScheme.onTertiary,
        child: AnimatedOpacity(
          duration: $style.times.fast,
          opacity: _opacity,
          curve: Curves.easeInOut,
          child: widget.node!.builder?.call(
            context,
            widget.node!,
            widget.isEnabled,
          ),
        ),
      );
    }

    double titleHeight = 100;
    double titleFontSize = 35;

    return Container(
      color:
          widget.isEnabled
              ? Theme.of(context).colorScheme.surface
              : Theme.of(context).colorScheme.onTertiary,
      child: AnimatedOpacity(
        opacity: _opacity,
        duration: $style.times.med,
        curve: Curves.easeInOut,
        child: Column(
          children: [
            //title
            Padding(
              padding: // title up/left padding
                  widget.isEnabled
                      ? EdgeInsets.fromLTRB(120, 20, 0, 0)
                      : EdgeInsets.fromLTRB(80, 20, 0, 0),
              child: SizedBox(
                height: titleHeight,
                child: Align(
                  alignment: Alignment.bottomLeft,
                  child: Container(
                    child: Text(
                      widget.node?.title ?? '',
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
                  child: Padding(
                    padding: // item left/right padding
                        widget.isEnabled
                            ? const EdgeInsets.symmetric(horizontal: 80, vertical: 10)
                            : const EdgeInsets.symmetric(horizontal: 40),
                    child: SettingListView(
                      key: _listKey,
                      node: widget.node!,
                      onSelectionChanged: (selected) {
                        widget.onSelectionChanged?.call(selected);
                      },
                    ),
                  ),
                ),
              ),
          ],
        ),
      ),
    );
  }
}
