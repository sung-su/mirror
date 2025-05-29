import 'dart:math';

import 'package:flutter/material.dart';
import 'package:tizen_fs/models/movie.dart';
import 'package:tizen_fs/widgets/age_rating.dart';
import 'package:tizen_fs/widgets/rotten_rating.dart';
import 'package:tizen_fs/widgets/star_rating.dart';

class FlexibleTitleForDetail extends StatelessWidget {
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
    this.minFontSize = 40,
    this.maxFontSize = 70,
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
          Padding(
            padding: const EdgeInsets.only(top: 16),
            child: Row(
              spacing: 20,
              children: [
                RottenRating(rating: 93),
                StarRating(rating: 3.8),
                AgeRating(rating: "12"),
                Text(movie.genres.isNotEmpty ? movie.genres[0].name : ''),
                Text(movie.releaseYear),
                Text(movie.runtime > 0
                    ? '${(movie.runtime / 60).floor()}h ${movie.runtime % 60}m'
                    : ''),
              ],
            ),
          ),
        ],
      );
    });
  }
}
