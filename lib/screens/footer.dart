import 'package:flutter/material.dart';
import 'package:tizen_fs/styles/app_style.dart';

class Footer extends StatefulWidget {
  const Footer({
    super.key,
    required this.scrollController
  });

  final ScrollController scrollController;

  @override
  State<Footer> createState() => _FooterState();
}

class _FooterState extends State<Footer> {
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
                  color: Colors.white.withAlphaF(0.9),
                ),
                SizedBox(
                  height: 30,
                ),
                Row(
                  children: [
                    Column(
                      mainAxisAlignment: MainAxisAlignment.start,
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(
                          'Movie and TV recommendations if For you come from your list\n'
                          + 'of streaming services. View and change your services now.',
                          textAlign: TextAlign.left,
                          style: TextStyle(
                            fontSize: 16,
                            color: Colors.white.withAlphaF(0.7),
                          ),
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
                            padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 18),
                            backgroundColor: _isFocused ? Colors.white.withAlphaF(0.95): Colors.grey,
                          ),
                          child: Text(
                            'Manage services',
                            style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
                          ),
                        ),
                      ],
                    ),
                    Spacer(),
                    SizedBox(
                      width: 300,
                      child: Image.asset(
                        'assets/mock/images/bottom_widget.png',
                        fit: BoxFit.fill,
                      ),
                    ),
                  ]
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}