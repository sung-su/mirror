import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:tizen_fs/providers/wifi_provider.dart';
import 'package:tizen_fs/styles/app_style.dart';

class WifiPasswordPopup extends StatefulWidget {
  const WifiPasswordPopup({
    super.key,
    required this.ap,
    required this.onConnect,
    required this.onDisconnect,
  });

  final WifiAP ap;
  final Function(String password) onConnect;
  final Function() onDisconnect;

  @override
  State<WifiPasswordPopup> createState() => _WifiPasswordPopupState();
}

class _WifiPasswordPopupState extends State<WifiPasswordPopup> {
  final TextEditingController _passwordController = TextEditingController();
  final FocusNode _passwordFocusNode = FocusNode();
  final FocusNode _connectFocusNode = FocusNode();
  final FocusNode _disconnectFocusNode = FocusNode();
  final FocusNode _cancelFocusNode = FocusNode();
  int _selected = 0;

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      _passwordFocusNode.requestFocus();
    });
  }

  @override
  void dispose() {
    _passwordController.dispose();
    _passwordFocusNode.dispose();
    _connectFocusNode.dispose();
    _disconnectFocusNode.dispose();
    _cancelFocusNode.dispose();
    super.dispose();
  }

  KeyEventResult _onKeyEvent(FocusNode node, KeyEvent event) {
    if (event is KeyDownEvent) {
      if (event.logicalKey == LogicalKeyboardKey.arrowDown) {
        setState(() {
          _selected = (_selected + 1).clamp(0, 3);
          _updateFocus();
        });
        return KeyEventResult.handled;
      } else if (event.logicalKey == LogicalKeyboardKey.arrowUp) {
        setState(() {
          _selected = (_selected - 1).clamp(0, 3);
          _updateFocus();
        });
        return KeyEventResult.handled;
      } else if (event.logicalKey == LogicalKeyboardKey.enter) {
        print("_onKeyEvent enter[${_selected}]");
        if (_selected == 1) {
          _handleConnect();
        } else if (_selected == 2) {
          _handleDisconnect();
        } else if (_selected == 3) {
          Navigator.of(context).pop();
        }
        return KeyEventResult.handled;
      } else if (event.logicalKey == LogicalKeyboardKey.escape) {
        Navigator.of(context).pop();
        return KeyEventResult.handled;
      }
    }
    return KeyEventResult.ignored;
  }

  void _updateFocus() {
    print("_updateFocus[${_selected}]");
    switch (_selected) {
      case 0:
        _passwordFocusNode.requestFocus();
        break;
      case 1:
        _connectFocusNode.requestFocus();
        break;
      case 2:
        _disconnectFocusNode.requestFocus();
        break;
      case 3:
        _cancelFocusNode.requestFocus();
        break;
    }
  }

  void _handleConnect() {
    print("_handleConnect");
    widget.onConnect(_passwordController.text);
    Navigator.of(context).pop();
  }

  void _handleDisconnect() {
    print("_handleDisconnect");
    widget.onDisconnect();
    Navigator.of(context).pop();
  }

  void _select(int index) {
    setState(() {
      _selected = index;
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Focus(
        autofocus: true,
        onKeyEvent: _onKeyEvent,
        child: Container(
          decoration: BoxDecoration(
            gradient: $style.gradients.generateLinearGradient(
              Colors.black,
              Colors.blue.shade900,
            ),
          ),
          child: Row(
            children: [
              Padding(
                padding: const EdgeInsets.fromLTRB(80, 80, 0, 0),
                child: Container(
                  width: 500,
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    spacing: 20,
                    children: [
                      Container(
                        child: Text(
                          'Wi-Fi',
                          style: TextStyle(fontSize: 15, color: Colors.white70),
                        ),
                      ),
                      Container(
                        child: Text(
                          '${widget.ap.essid}',
                          style: TextStyle(fontSize: 30, color: Colors.white),
                        ),
                      ),
                      Container(
                        child: Text(
                          'WPA', //TODO: how to know
                          style: TextStyle(fontSize: 18, color: Colors.white),
                        ),
                      ),
                      Container(
                        child: Text(
                          '${widget.ap.frequency} Hz',
                          style: TextStyle(fontSize: 15, color: Colors.white70),
                        ),
                      ),
                    ],
                  ),
                ),
              ),
              Expanded(
                child: Padding(
                  padding: const EdgeInsets.fromLTRB(0, 120, 100, 0),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.end,
                    spacing: 20,
                    children: [
                      //password, can not re-open ime
                      Container(
                        decoration: BoxDecoration(
                          color: _selected == 0
                              ? Colors.white.withOpacity(0.2)
                              : Colors.white.withOpacity(0.1),
                          borderRadius: BorderRadius.circular(8),
                          border: Border.all(
                            color: _selected == 0
                                ? Color(0xF04285F4)
                                : Colors.transparent,
                            width: 2,
                          ),
                        ),
                        child: TextField(
                          controller: _passwordController,
                          focusNode: _passwordFocusNode,
                          obscureText: true,
                          style: TextStyle(
                            fontSize: 16,
                            color: Colors.white,
                          ),
                          decoration: InputDecoration(
                            border: InputBorder.none,
                            contentPadding: EdgeInsets.symmetric(
                              horizontal: 15,
                              vertical: 12,
                            ),
                            hintText: 'Password',
                            hintStyle: TextStyle(color: Colors.white54),
                          ),
                        ),
                      ),
                      PopupButton(
                        focusNode: _connectFocusNode,
                        text: 'Connect',
                        isSelected: _selected == 1,
                        onPressed: () {
                          _select(1);
                          _handleConnect();
                        },
                      ),
                      PopupButton(
                        focusNode: _disconnectFocusNode,
                        text: 'Disconnect',
                        isSelected: _selected == 2,
                        onPressed: () {
                          _select(2);
                          _handleConnect();
                        },
                      ),
                      PopupButton(
                        focusNode: _cancelFocusNode,
                        text: 'Cancel',
                        isSelected: _selected == 3,
                        onPressed: () {
                          _select(3);
                          Navigator.of(context).pop();
                        },
                      ),
                    ],
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

class PopupButton extends StatelessWidget {
  const PopupButton({
    super.key,
    this.isSelected = false,
    required this.text,
    required this.onPressed,
    this.width = 280,
    this.height = 50,
    this.focusNode,
  });

  final bool isSelected;
  final String text;
  final VoidCallback? onPressed;
  final double width;
  final double height;
  final FocusNode? focusNode;

  @override
  Widget build(BuildContext context) {
    return Focus(
      focusNode: focusNode,
      child: GestureDetector(
        onTap: onPressed,
        child: SizedBox(
          width: width,
          height: height,
          child: AnimatedScale(
            scale: isSelected ? 1.1 : 1,
            duration: const Duration(milliseconds: 80),
            child: Container(
              decoration: BoxDecoration(
                borderRadius: BorderRadius.circular(30),
                color:
                    isSelected
                        ? Theme.of(context).colorScheme.primary
                        : Theme.of(context).colorScheme.surfaceContainerHighest,
              ),
              child: Padding(
                padding: const EdgeInsets.symmetric(horizontal: 30, vertical: 5),
                child: Row(
                  mainAxisAlignment: MainAxisAlignment.start,
                  spacing: 15,
                  children: [
                    Column(
                      mainAxisAlignment: MainAxisAlignment.center,
                      crossAxisAlignment: CrossAxisAlignment.start,
                      spacing: 3,
                      children: [
                        Text(
                          text,
                          style: TextStyle(
                            fontSize: 13,
                            color:
                                isSelected
                                    ? Theme.of(context).colorScheme.onPrimary
                                    : Theme.of(context).colorScheme.primary,
                          ),
                        ),
                      ],
                    ),
                  ],
                ),
              ),
            ),
          ),
        ),
      ),
    );
  }
}
