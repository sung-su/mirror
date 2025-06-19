import 'dart:math';

import 'package:flutter/material.dart';
import 'package:tizen_fs/models/movie.dart';

class FlexibleTitleForDetail extends StatelessWidget {
  final void Function(BuildContext)? onFocused;
  final double expandedHeight;
  final double collapsedHeight;
  final double minFontSize;
  final double maxFontSize;
  final Movie movie;

  const FlexibleTitleForDetail({
    super.key,
    required this.expandedHeight,
    required this.collapsedHeight,
    required this.movie,
    this.minFontSize = 35,
    this.maxFontSize = 60,
    this.onFocused    
  });

  @override
  Widget build(BuildContext context) {
    return LayoutBuilder(builder: (context, constraints) {
      final ratio = max(constraints.maxHeight - collapsedHeight, 0) /
          max(expandedHeight - collapsedHeight, 0);
      return Column(
        mainAxisAlignment: MainAxisAlignment.end,
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          DefaultTextStyle(
              style: TextStyle(
                  fontSize: maxFontSize - ratio * (maxFontSize - minFontSize)),
              child: Text(movie.title)),
        ],
      );
    });
  }
} 