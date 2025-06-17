import 'package:flutter/material.dart';
import 'package:tizen_fs/styles/app_style.dart';

class AgeRating extends StatelessWidget {
  final String rating;
  const AgeRating({super.key, required this.rating});

  @override
  Widget build(BuildContext context) {
    // draw solid rounded border and place a rating on center
    return Container(
        padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 2),
        decoration: BoxDecoration(
            borderRadius: BorderRadius.circular(6),
            border: Border.all(color: Colors.white.withAlphaF(0.6), width: 1)),
            child: Center(child: Text(rating, style: TextStyle(fontSize: 11),)));
  }
}