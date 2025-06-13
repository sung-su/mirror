import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:tizen_fs/screens/setting_panel.dart';
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
