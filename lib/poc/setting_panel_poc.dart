import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:tizen_fs/widgets/backdrop_scaffold.dart';

class SettingPanelPocPage extends StatelessWidget {
  const SettingPanelPocPage({super.key});

  @override
  Widget build(BuildContext context) {
    return BackdropScaffold(
      child: SettingPanelPoc(),
    );
  }
}

class SettingPanel extends StatefulWidget {
  const SettingPanel({super.key});

  @override
  State<SettingPanel> createState() => _SettingPanelState();
}

class _SettingPanelState extends State<SettingPanel> {
  final FocusNode _settingFocusNode = FocusNode();
  final List<FocusNode> _focusNodes = List.generate(4, (index) => FocusNode());
  final List<IconData> _icons = [
    Icons.home_outlined,
    Icons.wifi_outlined,
    Icons.accessibility_outlined,
    Icons.bluetooth_outlined
  ];
  final List<String> _texts = [
    "Screensaver",
    "Wi-Fi",
    "Accessibility",
    "Bluetooth"
  ];

  @override
  void initState() {
    super.initState();
    _settingFocusNode.requestFocus();
  }

  @override
  void dispose() {
    _settingFocusNode.dispose();
    for (var focusNode in _focusNodes) {
      focusNode.dispose();
    }
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Material(
      color: Colors.transparent,
      child: Align(
        alignment: Alignment.centerRight,
        child: Container(
          margin: const EdgeInsets.fromLTRB(15, 15, 25, 15),
          padding: const EdgeInsets.fromLTRB(10, 20, 10, 20),
          width: 300,
          height: MediaQuery.of(context).size.height,
          decoration: BoxDecoration(
            color: const Color.fromARGB(255, 28, 32, 42),
            borderRadius: BorderRadius.circular(20),
          ),
          child: FocusScope(
            onKeyEvent: (node, event) {
              if (_settingFocusNode.hasFocus) {
                if (event is KeyDownEvent) {
                  if (event.logicalKey == LogicalKeyboardKey.arrowLeft) {
                    Navigator.of(context).pop();
                    return KeyEventResult.handled;
                  }
                }
              }
              for (int i = 0; i < _focusNodes.length; i+=2) {
                if (_focusNodes[i].hasFocus) {
                   if (event is KeyDownEvent && event.logicalKey == LogicalKeyboardKey.arrowLeft) {
                    Navigator.of(context).pop();
                    return KeyEventResult.handled;
                  }
                }
              }
              return KeyEventResult.ignored;
            },
            child: Column(
              children: [
                Row(
                  children: [
                    Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        const Text(
                          "Thu, Apr 24",
                          style: TextStyle(fontSize: 13, color: Colors.grey),
                        ),
                        const SizedBox(height: 5),
                        const Text(
                          "12:00 PM",
                          style: TextStyle(
                              fontSize: 20,
                              color: Color.fromARGB(255, 167, 167, 167)),
                        ),
                      ],
                    ),
                    const Spacer(),
                    IconButton(
                      focusNode: _settingFocusNode,
                      icon: const Icon(Icons.settings_outlined, size: 18),
                      color: Colors.grey,
                      onPressed: () {
                        Navigator.of(context).pop();
                      },
                    ),
                    IconButton(
                      icon: const Icon(Icons.sentiment_satisfied_outlined,
                          size: 18),
                      color: Colors.grey,
                      onPressed: () {
                        Navigator.of(context).pop();
                      },
                    ),
                  ],
                ),
                const SizedBox(height: 20),
                GridView.builder(
                  shrinkWrap: true,
                  physics: const NeverScrollableScrollPhysics(),
                  gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
                    crossAxisCount: 2,
                    childAspectRatio: 2.5,
                    crossAxisSpacing: 5,
                    mainAxisSpacing: 5,
                  ),
                  itemCount: 4,
                  itemBuilder: (context, index) {
                    return Focus(
                      focusNode: _focusNodes[index],
                      child: Builder(
                        builder: (context) {
                          return Container(
                            padding: const EdgeInsets.only(left:15, right: 15),
                            decoration: BoxDecoration(
                              color: Focus.of(context).hasFocus ? const Color.fromARGB(255, 100, 100, 100) : const Color.fromARGB(255, 18, 18, 18),
                              borderRadius: BorderRadius.circular(10),
                            ),
                            child: Align(
                              alignment: Alignment.centerLeft,
                              child: Row(
                                spacing: 10,
                                children: [
                                  Icon(
                                    _icons[index],
                                    size: 16,
                                    color: Colors.grey,
                                  ),
                                  Text(
                                    _texts[index],
                                    style: const TextStyle(color: Colors.white, fontSize: 13),
                                    textAlign: TextAlign.left,
                                  ),
                                ],
                              ),
                            ),
                          );
                        }
                      ),
                    );
                  },
                ),
                const SizedBox(height: 20),
                Center(
                    child: Padding(
                  padding: const EdgeInsets.all(8.0),
                  child: const Text('Tip of the day',
                      style: TextStyle(fontSize: 13, color: Colors.grey)),
                )),
                Container(
                    padding: const EdgeInsets.all(10),
                    margin: EdgeInsets.zero,
                    // border decoration
                    decoration: BoxDecoration(
                      border: Border.all(color: Colors.grey.withAlpha(50)),
                      borderRadius: BorderRadius.circular(10),
                    ),
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.center,
                      mainAxisAlignment: MainAxisAlignment.center,
                      children: [
                        Icon(Icons.bookmark_outline,
                            size: 16, color: Colors.grey),
                        const SizedBox(height: 5),
                        const Text(
                          "One whatchlist across streaming services",
                          style: TextStyle(
                            fontSize: 13,
                            color: Color.fromARGB(255, 167, 167, 167),
                          ),
                          textAlign: TextAlign.center,
                        ),
                        const SizedBox(height: 5),
                        Padding(
                          padding: const EdgeInsets.all(8.0),
                          child: const Text(
                            "Find all the movies and shows you've added to your watchlist in a single place under the library tab",
                            style: TextStyle(
                              fontSize: 10,
                              color: Color.fromARGB(255, 125, 125, 125),
                            ),
                            textAlign: TextAlign.center,
                          ),
                        ),
                      ],
                    ))
              ],
            ),
          ),
        ),
      ),
    );
  }
}

class SettingPanelPoc extends StatefulWidget {
  const SettingPanelPoc({super.key});

  @override
  State<SettingPanelPoc> createState() => _SettingPanelPocState();
}

class _SettingPanelPocState extends State<SettingPanelPoc> {
  void showSettingPanel(BuildContext context) {
    showGeneralDialog(
      context: context,
      barrierDismissible: true,
      barrierLabel: "Close",
      barrierColor: Colors.transparent,
      transitionDuration: const Duration(milliseconds: 200),
      pageBuilder: (BuildContext buildContext, Animation animation,
          Animation secondaryAnimation) {
        return SettingPanel();
      },
      transitionBuilder: (context, animation, secondaryAnimation, child) {
        return SlideTransition(
          position: Tween<Offset>(
            begin: const Offset(1, 0),
            end: Offset.zero,
          ).animate(animation),
          child: FadeTransition(
            opacity: animation,
            child: child,
          ),
        );
      },
    );
  }

  int selected = 0;

  @override
  Widget build(BuildContext context) {
    return Column(
      mainAxisAlignment: MainAxisAlignment.center,
      children: <Widget>[
        const Text('Setting panel poc',
            style: TextStyle(fontSize: 24, color: Colors.white)),

        Focus(
          autofocus: true,
          onFocusChange: (hasFocus) {
            if (hasFocus && selected == 3) {
              setState(() {
                selected = 2;
              });
            }
          },
          onKeyEvent: (_, onKeyEvent) {
            if (onKeyEvent is KeyDownEvent) {
              if (onKeyEvent.logicalKey == LogicalKeyboardKey.arrowRight) {
                if (selected == 2) {
                  showSettingPanel(context);
                }
                setState(() {
                  selected = (selected + 1) % 4;
                });
                return KeyEventResult.handled;
              } else if (onKeyEvent.logicalKey == LogicalKeyboardKey.arrowLeft) {
                setState(() {
                  selected = (selected - 1) % 3;
                });
                return KeyEventResult.handled;
              }
            }
            return KeyEventResult.ignored;
          },
          child: Builder(
            builder: (context) {
              return Row(
                spacing: 20,
                children: [
                SelectableText("Home", selected == 0, Focus.of(context).hasFocus),
                SelectableText("Apps", selected == 1, Focus.of(context).hasFocus),
                SelectableText("Library", selected == 2, Focus.of(context).hasFocus),
              ],);
            }
          ),
        ),
      ],
    );
  }
}

class SelectableText extends StatelessWidget {
  const SelectableText(this.text, this.isSelected, this.isFocused, {super.key});
  final bool isSelected;
  final bool isFocused;
  final String text;

  @override
  Widget build(BuildContext context) {
    return Card(
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(30),
      ),
      color: isSelected ? (isFocused ? const Color.fromARGB(255, 220, 220, 220) : const Color.fromARGB(255, 109, 109, 109) ) : Colors.transparent,
      child: Padding(
        padding: const EdgeInsets.all(10),
        child: Text(
          text,
          style: TextStyle(color: isSelected ? Colors.black : Colors.white, fontSize: 16),
        ),
      ),
    );
  }
}
