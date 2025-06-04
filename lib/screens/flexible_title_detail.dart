import 'dart:math';

import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:tizen_fs/models/movie.dart';
import 'package:tizen_fs/widgets/age_rating.dart';
import 'package:tizen_fs/screens/rotten_rating.dart';
import 'package:tizen_fs/widgets/star_rating.dart';

class FlexibleTitleForDetail extends StatelessWidget {
  final void Function(BuildContext)? onFocused;
  final GlobalKey<RottenRatingState> _focusedKey = GlobalKey<RottenRatingState>();
  final double expandedHeight;
  final double collapsedHeight;
  final double minFontSize;
  final double maxFontSize;
  final Movie movie;

  FlexibleTitleForDetail({
    super.key,
    required this.expandedHeight,
    required this.collapsedHeight,
    required this.movie,
    this.minFontSize = 35,
    this.maxFontSize = 45,
    this.onFocused    
  });

  KeyEventResult _onKeyEvent(FocusNode focusNode, KeyEvent event) {
    if (event is KeyDownEvent || event is KeyRepeatEvent) {
      if (event.logicalKey == LogicalKeyboardKey.arrowUp){
        return KeyEventResult.handled;
      }
    }
    return KeyEventResult.ignored;
  }

  @override
  Widget build(BuildContext context) {
    return LayoutBuilder(builder: (context, constraints) {
      final ratio = max(constraints.maxHeight - collapsedHeight, 0) /
          max(expandedHeight - collapsedHeight, 0);
      return Focus(
        onKeyEvent: _onKeyEvent,
        child: Column(
          mainAxisAlignment: MainAxisAlignment.end,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            DefaultTextStyle(
                style: TextStyle(
                    fontSize: maxFontSize - ratio * (maxFontSize - minFontSize)),
                child: Text(movie.title)),
            Padding(
              padding: const EdgeInsets.only(top: 16),
              child: Row(
                spacing: 20,
                children: [
                  RottenRating(
                    key: _focusedKey,
                    rating: (movie.voteAverage * 10).floor()),
                  StarRating(rating: (movie.voteAverage / 2).toStringAsFixed(2)),
                  AgeRating(rating: movie.certification),
                  Text(movie.genres.isNotEmpty ? movie.genres[0].name : ''),
                  Text(movie.releaseYear),
                  Text(movie.runtime > 0
                      ? '${(movie.runtime / 60).floor()}h ${movie.runtime % 60}m'
                      : ''),
                ],
              ),
            ),
          ],
        ),
        onFocusChange: (hasFocus) {
          if(hasFocus) {
            _focusedKey.currentState?.requestFocus();
            onFocused?.call(context);
          }
        }
      );
    });
  }
} 