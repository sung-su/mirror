import 'package:flutter/material.dart';
import 'package:tizen_fs/models/movie.dart';
import 'package:tizen_fs/styles/app_style.dart';

class ImportantInformation extends StatefulWidget {
  final void Function(BuildContext)? onFocused;
  final Movie movie;
  const ImportantInformation({
    super.key,
    this.onFocused,
    required this.movie,
  });

  @override
  State<ImportantInformation> createState() => _ImportantInformationState();
}

class _ImportantInformationState extends State<ImportantInformation> {
  bool _isFocused = false;

  @override
  Widget build(BuildContext context) {
    return Focus(
      child: AnimatedOpacity(
        opacity: _isFocused ? 1.0 : 0.5,
        duration: const Duration(milliseconds: 150),
        child: Padding(
            padding: const EdgeInsets.symmetric(horizontal: 58),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              spacing: 10,
              children: [
                Container(
                  height: 0.7,
                  color: Colors.white.withAlphaF(0.9),
                ),
                SizedBox(
                  height: 15,
                ),
                Text("Important Information", style: TextStyle(fontSize: 20)),
                Row(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                  spacing: 20,
                  children: [
                    Expanded(
                      child: Container(
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          spacing: 10,
                          children: [
                            Text("Quality",
                                style: TextStyle(
                                    fontSize: 15, fontWeight: FontWeight.w500)),
                            Text(widget.movie.quality,
                                maxLines: 7, style: TextStyle(fontSize: 12)),
                            SizedBox(height: 5),
                          ],
                        ),
                      ),
                    ),
                    Expanded(
                      child: Container(
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          spacing: 10,
                          children: [
                            Text("Rating description",
                                style: TextStyle(
                                    fontSize: 15, fontWeight: FontWeight.w500)),
                            Text(widget.movie.certification.toString(),
                                style: TextStyle(fontSize: 12)),
                          ],
                        ),
                      ),
                    ),
                    Expanded(
                      child: Container(
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          spacing: 10,
                          children: [
                            Text("Audio language",
                                style: TextStyle(
                                    fontSize: 15, fontWeight: FontWeight.w500)),
                            Text(widget.movie.audioLanguage,
                                style: TextStyle(fontSize: 12)),
                          ],
                        ),
                      ),
                    ),
                    Expanded(
                      child: Container(
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          spacing: 10,
                          children: [
                            Text("Subtitle",
                                style: TextStyle(
                                    fontSize: 15, fontWeight: FontWeight.w500)),
                            Text(widget.movie.audioLanguage,
                                style: TextStyle(fontSize: 12)),
                          ],
                        ),
                      ),
                    ),
                  ],
                ),
                SizedBox(height: 10)
              ],
            )),
      ),
      onFocusChange: (hasFocus) {
        if (hasFocus) {
          setState(() {
            _isFocused = true;
          });
          widget.onFocused?.call(context);
        }
      },
    );
  }
}
