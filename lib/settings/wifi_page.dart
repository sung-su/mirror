import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:provider/provider.dart';
import 'package:tizen_fs/models/page_node.dart';
import 'package:tizen_fs/providers/wifi_provider.dart';
import 'package:tizen_fs/settings/wifi_password_popup.dart';
import 'package:tizen_fs/styles/app_style.dart';
import 'package:tizen_fs/widgets/focus_selectable.dart';
import 'package:tizen_fs/widgets/selectable_listview.dart';

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
                node: widget.node!,
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

class WifiListView extends StatefulWidget {
  const WifiListView({
    super.key,
    required this.node,
    required this.isEnabled,
    this.onSelectionChanged,
  });

  final PageNode node;
  final bool isEnabled;
  final Function(int)? onSelectionChanged;

  @override
  State<WifiListView> createState() => WifiListViewState();
}

class WifiListViewState extends State<WifiListView>
    with FocusSelectable<WifiListView> {
  int _selected = 0;

  @override
  LogicalKeyboardKey getNextKey() {
    return LogicalKeyboardKey.arrowDown;
  }

  @override
  LogicalKeyboardKey getPrevKey() {
    return LogicalKeyboardKey.arrowUp;
  }

  void initFocus() {
    focusNode.requestFocus();
  }

  void selectTo(int index) {
    listKey.currentState?.selectTo(index);
  }

  void _showWifiPasswordPopup(WifiAP ap) {
    final wifiProvider = Provider.of<WifiProvider>(context, listen: false);
    showGeneralDialog(
      context: context,
      barrierDismissible: true,
      barrierLabel: "Close",
      barrierColor: Colors.transparent,
      transitionDuration: $style.times.pageTransition,
      pageBuilder: (BuildContext buildContext, Animation animation, Animation secondaryAnimation) {
        return WifiPasswordPopup(
          ap: ap,
          onConnect: (password) async {
            await wifiProvider.connectToAp(ap.essid, password: password);
          },
        );
      },
      transitionBuilder: (context, animation, secondaryAnimation, child) {
        return FadeTransition(
          opacity: animation,
          child: child,
        );
      },
    );
  }

  @override
  KeyEventResult onKeyEvent(FocusNode focusNode, KeyEvent event) {
    if (event is KeyDownEvent || event is KeyRepeatEvent) {
      if (event.logicalKey == LogicalKeyboardKey.enter ||
          event.logicalKey == LogicalKeyboardKey.select) {
        final wifiProvider = Provider.of<WifiProvider>(context, listen: false);
        if (_selected == 0) {
          if (wifiProvider.isActivated) {
            wifiProvider.wifiOff();
          } else {
            wifiProvider.wifiOn();
          }
        } else if (_selected > 0 && wifiProvider.isActivated) {
          final apIndex = _selected - 1;
          if (apIndex < wifiProvider.apList.length) {
            final selectedAp = wifiProvider.apList[apIndex];
            if (selectedAp == wifiProvider.connectedAp) {
              wifiProvider.disconnectFromCurrentAp();
            } else {
              _showWifiPasswordPopup(selectedAp);
            }
          }
        }
        return KeyEventResult.handled;
      }
    }
    return KeyEventResult.ignored;
  }

  @override
  Widget build(BuildContext context) {
    return Consumer<WifiProvider>(
      builder: (context, wifiProvider, child) {
        if (wifiProvider.isActivated &&
            !wifiProvider.isScanning &&
            wifiProvider.apList.isEmpty) {
          WidgetsBinding.instance.addPostFrameCallback((_) {
            wifiProvider.scanAndRefresh();
          });
        }

        final itemCount =
            1 + (wifiProvider.isActivated ? wifiProvider.apList.length : 0);

        return Focus(
          focusNode: focusNode,
          onFocusChange: (hasfocus) {
            if (hasfocus) {
              listKey.currentState?.selectTo(_selected);
            } else {
              _selected = listKey.currentState?.selectedIndex ?? 0;
            }
          },
          child: SelectableListView(
            scrollOffset: 260,
            key: listKey,
            padding: const EdgeInsets.symmetric(vertical: 10),
            alignment: 0.5,
            itemCount: itemCount,
            scrollDirection: Axis.vertical,
            onSelectionChanged: (selected) {
              _selected = selected;
              widget.onSelectionChanged?.call(selected);
            },
            itemBuilder: (context, index, selectedIndex, key) {
              return AnimatedScale(
                key: key,
                scale:
                    Focus.of(context).hasFocus && index == selectedIndex
                        ? 1.0
                        : .9,
                duration: $style.times.med,
                curve: Curves.easeInOut,
                child: GestureDetector(
                  onTap: () {
                    listKey.currentState?.selectTo(index);
                    Focus.of(context).requestFocus();
                  },
                  child:
                      index == 0
                          ? WifiSwitchItem(
                            isFocused:
                                Focus.of(context).hasFocus &&
                                index == selectedIndex,
                            isEnabled: widget.isEnabled,
                            wifiProvider: wifiProvider,
                          )
                          : WifiApItem(
                            isFocused:
                                Focus.of(context).hasFocus &&
                                index == selectedIndex,
                            isEnabled: widget.isEnabled,
                            wifiProvider: wifiProvider,
                            apIndex: index - 1,
                          ),
                ),
              );
            },
          ),
        );
      },
    );
  }
}

class WifiSwitchItem extends StatelessWidget {
  const WifiSwitchItem({
    super.key,
    required this.isFocused,
    required this.isEnabled,
    required this.wifiProvider,
  });

  final bool isFocused;
  final bool isEnabled;
  final WifiProvider wifiProvider;

  final double titleFontSize = 15;
  final double subtitleFontSize = 11;
  final double innerPadding = 20;
  final double itemHeight = 65;
  final double iconSize = 25;

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      height: itemHeight,
      child: Container(
        decoration: BoxDecoration(
          borderRadius: BorderRadius.circular(10),
          color:
              isFocused
                  ? Theme.of(context).colorScheme.tertiary
                  : Colors.transparent,
        ),
        child: Padding(
          padding: EdgeInsets.symmetric(horizontal: innerPadding),
          child: Row(
            mainAxisAlignment: MainAxisAlignment.start,
            spacing: 15,
            children: [
              Expanded(
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      'WiFi',
                      style: TextStyle(
                        fontSize: titleFontSize,
                        color:
                            isFocused
                                ? Theme.of(context).colorScheme.onTertiary
                                : Theme.of(context).colorScheme.tertiary,
                      ),
                    ),
                  ],
                ),
              ),
              Theme(
                data: Theme.of(context).copyWith(useMaterial3: false),
                child: Padding(
                  padding:
                      isEnabled
                          ? const EdgeInsets.only(right: 0)
                          : const EdgeInsets.only(right: 120),
                  child: Switch(
                    value: wifiProvider.isActivated,
                    onChanged: (value) async {
                      if (value) {
                        await wifiProvider.wifiOn();
                      } else {
                        await wifiProvider.wifiOff();
                      }
                    },
                    activeColor: Color(0xF04285F4),
                    inactiveThumbColor: Color(0xF0AEB2B9),
                  ),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

class WifiApItem extends StatelessWidget {
  const WifiApItem({
    super.key,
    required this.isFocused,
    required this.isEnabled,
    required this.wifiProvider,
    required this.apIndex,
  });

  final bool isFocused;
  final bool isEnabled;
  final WifiProvider wifiProvider;
  final int apIndex;

  final double titleFontSize = 15;
  final double subtitleFontSize = 11;
  final double innerPadding = 20;
  final double itemHeight = 65;
  final double iconSize = 25;

  @override
  Widget build(BuildContext context) {
    if (apIndex >= wifiProvider.apList.length) {
      return SizedBox.shrink();
    }

    final ap = wifiProvider.apList[apIndex];
    final isConnected = ap.state == 1;
    final isConnectedToThisAp = wifiProvider.connectedAp?.handle == ap.handle;

    return SizedBox(
      height: itemHeight,
      child: Container(
        decoration: BoxDecoration(
          borderRadius: BorderRadius.circular(10),
          color:
              isFocused
                  ? Theme.of(context).colorScheme.tertiary
                  : Colors.transparent,
        ),
        child: Padding(
          padding: EdgeInsets.symmetric(horizontal: innerPadding),
          child: Row(
            mainAxisAlignment: MainAxisAlignment.start,
            spacing: 15,
            children: [
              Container(
                width: 43,
                height: 43,
                decoration: BoxDecoration(
                  shape: BoxShape.circle,
                  color:
                      isFocused
                          ? Color(0xF04285F4).withAlphaF(0.2)
                          : Color(0xF0263041),
                ),
                child: Icon(
                  _getWifiIcon(ap.rssi),
                  size: iconSize,
                  color: isFocused ? Color(0xF04285F4) : Color(0xF0AEB2B9),
                ),
              ),
              Expanded(
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      ap.essid,
                      style: TextStyle(
                        fontSize: titleFontSize,
                        color:
                            isFocused
                                ? Theme.of(context).colorScheme.onTertiary
                                : Theme.of(context).colorScheme.tertiary,
                      ),
                    ),
                  ],
                ),
              ),
              if (isConnectedToThisAp)
                Theme(
                  data: Theme.of(context).copyWith(useMaterial3: false),
                  child: Switch(
                    value: true,
                    onChanged: null,
                    activeColor: Color(0xF04285F4),
                    inactiveThumbColor: Color(0xF0AEB2B9),
                  ),
                )
              else if (wifiProvider.isConnecting && wifiProvider.connectedAp?.handle == ap.handle)
                SizedBox(
                  width: 20,
                  height: 20,
                  child: CircularProgressIndicator(
                    strokeWidth: 2,
                    color: Color(0xF04285F4),
                  ),
                ),
            ],
          ),
        ),
      ),
    );
  }

  IconData _getWifiIcon(int rssi) {
    if (rssi > -50) {
      return Icons.wifi;
    } else if (rssi > -60) {
      return Icons.network_wifi_3_bar;
    } else if (rssi > -70) {
      return Icons.wifi_2_bar;
    } else if (rssi > -80) {
      return Icons.wifi_1_bar;
    } else {
      return Icons.wifi_off;
    }
  }
}
