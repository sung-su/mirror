import 'package:flutter/material.dart';

class EndWidget extends StatefulWidget {
  const EndWidget({super.key,
  required this.scrollController
  });

  final ScrollController scrollController;

  @override
  State<EndWidget> createState() => _EndWidgetState();
}

class _EndWidgetState extends State<EndWidget> {
  bool _isFocused = false;

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      height: 300,
      child: AnimatedOpacity(
        opacity: _isFocused ? 1.0 : 0.5,
        duration: const Duration(milliseconds: 100),
        child: Container(
          child: Padding(
            padding: const EdgeInsets.fromLTRB(70, 20, 70, 0),
            child: Column(
              mainAxisAlignment: MainAxisAlignment.start,
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Container(
                  height: 0.7,
                  color: Colors.white,
                ),
                SizedBox(
                  height: 20,
                ),
                Row(
                  children: [
                    Text(
                    'Movie and TV recommendations if For you come from your list\n'
                    + 'of streaming services. View and change your services now.',
                    textAlign: TextAlign.left,
                  ),
                  Spacer()
                  ]
                ),
                SizedBox(
                  height: 20,
                ),
                TextButton(
                  onPressed: () {
                    debugPrint("pressed");
                  }, 
                  onFocusChange: (focused){
                    setState(() {
                      _isFocused = focused;
                    });
                  },
                  style: TextButton.styleFrom(
                    padding: const EdgeInsets.symmetric(horizontal: 15, vertical: 10),
                    backgroundColor: _isFocused ? Colors.white : Colors.grey,
                  ),
                  child: Text(
                    'Manage services',
                    style: TextStyle(fontSize: 16),
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}