import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:tizen_fs/settings/master_page.dart';

class MasterDetailPage extends StatefulWidget {
  final String title;
  final Widget? masterPage;
  final Widget? detailPage;
  final Function(int)? onFocusedItemChanged;
  final Function(int)? onPageChanged;

  MasterDetailPage({
    super.key,
    this.title = "",
    this.masterPage,
    this.detailPage,
    this.onFocusedItemChanged,
    this.onPageChanged,
  });

  @override
  State<MasterDetailPage> createState() => MasterDetailPageState();
}

class MasterDetailPageState extends State<MasterDetailPage> {
  PageController pageController =
      PageController(initialPage: 0, viewportFraction: 0.585);

  static const double padding = 90;

  late PageView pageView;
  void moveNext() {
    print("@mdpage moveNext");
    pageController.animateToPage(current,
        duration: Duration(milliseconds: 300), curve: Curves.easeInOut);
  }

  void movePrev() {
    print("@mdpage movePrev");
    pageController.animateToPage(current,
        duration: Duration(milliseconds: 300), curve: Curves.linear);
  }

  int focusedItemIndex = 0;
  int current = 0;
  int pageIndex = 0;

  KeyEventResult _onKeyEvent(FocusNode node, KeyEvent event) {
    if (event is KeyDownEvent) {
      if (event.logicalKey == LogicalKeyboardKey.arrowRight) {
        print("@main _onKeyEvent.arrowRight pageIndex=[$pageIndex]");
        setState(() {
          current < 3 ? ++current : current;
        });
        moveNext();
        return KeyEventResult.handled;
      } else if (event.logicalKey == LogicalKeyboardKey.arrowLeft) {
        print("@main _onKeyEvent.arrowLeft pageIndex=[$pageIndex]");
        setState(() {
          current > 0 ? --current : current;
        });
        movePrev();
        return KeyEventResult.handled;
      } else if (event.logicalKey == LogicalKeyboardKey.enter ||
          event.logicalKey == LogicalKeyboardKey.select) {
        return KeyEventResult.handled;
      }
      return KeyEventResult.ignored;
    }
    return KeyEventResult.ignored;
  }

  @override
  void initState() {
    super.initState();
    focusedItemIndex = 0;
    current = 0;
    pageIndex = 0;
  }

  @override
  void didUpdateWidget(covariant MasterDetailPage oldWidget) {
    super.didUpdateWidget(oldWidget);
  }

  @override
  void dispose() {
    pageController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Focus(
      focusNode: FocusNode(),
      onKeyEvent: _onKeyEvent,
      child: PageView(
          controller: pageController,
          scrollDirection: Axis.horizontal,
          onPageChanged: (index) {
            print("@ pageview onpagechanged index=$index, current=$current");
            // setState(() {
            //   current = index;
            // });
          },
          // physics: PageScrollPhysics(),
          physics: NeverScrollableScrollPhysics(),
          padEnds: false,
          children: [
            MasterPage(
              title: "0",
              isEnabled: current >= 0,
            ),
            MasterPage(
              title: "1",
              isEnabled: current >= 1,
            ),
            MasterPage(
              title: "2",
              isEnabled: current >= 2,
            ),
            MasterPage(
              title: "3",
              isEnabled: current >= 3,
            ),
          ]),
    );
  }
}
