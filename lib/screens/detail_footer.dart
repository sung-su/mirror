import 'package:flutter/material.dart';
import 'package:tizen_fs/styles/app_style.dart';

class ImportantInformation extends StatefulWidget {
  final void Function(BuildContext)? onFocused;
  const ImportantInformation({
    super.key,
    this.onFocused
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
                  spacing: 20,
                  children: [
                    SizedBox(
                      width: 200,
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        spacing: 10,
                        children: [
                          Text("Quality", style: TextStyle(fontSize: 15)),
                          Text(
                              "Automatically plays in the highjest\nquality available for your purchase",
                              style: TextStyle(fontSize: 12)),
                          Text("Purchase details", style: TextStyle(fontSize: 15)),
                          Text(
                              "Automatically plays in the highjest\nquality available for your purchase",
                              style: TextStyle(fontSize: 12)),
                        ],
                      ),
                    ),
                    SizedBox(
                      width: 200,
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        spacing: 10,
                        children: [
                          Text("Rating description",
                              style: TextStyle(fontSize: 15)),
                          Text("15", style: TextStyle(fontSize: 12)),
                          Text("Data sharing", style: TextStyle(fontSize: 15)),
                          Text(
                              "Information about movie and show transactions may be shared amongst YouTube, Google TV, Google Play Movies & TV, and other Google Services to support your access to content and those services",
                              style: TextStyle(fontSize: 12)),
                        ],
                      ),
                    )
                  ],
                ),
                Container(
                  color: Colors.blue,
                  child: SizedBox(
                    height: 100,
                  ),
                )
              ],
            )),
      ),
      onFocusChange: (hasFocus) {
        setState(() {
          _isFocused = hasFocus;
        });
        if(hasFocus)
          widget.onFocused?.call(context);
      },
    );
  }
}
