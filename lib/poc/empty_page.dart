import 'package:flutter/material.dart';
import 'package:tizen_fs/models/page_node.dart';
import 'package:tizen_fs/styles/app_style.dart';

class EmptyPage extends StatelessWidget {
  final PageNode node;
  final bool isEnabled;
  const EmptyPage({super.key, required this.node, required this.isEnabled});

  @override
  Widget build(BuildContext context) {
    
    double titleHeight = 100;
    double titleFontSize = 35;

    final textPainter = TextPainter(
      text: TextSpan(
        text: node.title,
        style: TextStyle(fontSize: titleFontSize),
      ),
      textDirection: TextDirection.ltr,
      maxLines: 2, 
    )..layout(maxWidth: 240);

    final neededHeight = textPainter.size.height;

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        SizedBox(
          height: neededHeight + 50 < titleHeight ? titleHeight : titleHeight + neededHeight,
          width: 400,
          child: AnimatedPadding(
            duration: $style.times.med,
            padding: // title up/left padding
              isEnabled
                  ? EdgeInsets.fromLTRB(120, 60, 40, 0)
                  : EdgeInsets.fromLTRB(80, 60, 80, 0),
            child: Text(
              node.title,
              softWrap: true,
              overflow: TextOverflow.visible,
              style: TextStyle(
                fontSize: 35
              )
            ),
          ),
        ),
      ],
    );
  }
}
