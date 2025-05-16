import 'package:flutter/material.dart';

class StarRating extends StatelessWidget {
  const StarRating({super.key, required this.rating});
  final double rating;

  @override
  Widget build(BuildContext context) {
    return Row(
      spacing: 5,
      children: [
        Icon(Icons.star, color: Colors.yellow[700], size: 15),
        Text('$rating / 5', style: TextStyle(fontSize: 17))
      ],
    );
  }
}