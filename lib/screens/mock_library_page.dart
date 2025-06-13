import 'package:flutter/material.dart';
import 'package:tizen_fs/styles/app_style.dart';

class MockLibraryPage extends StatelessWidget {
  const MockLibraryPage({super.key});

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      height: MediaQuery.of(context).size.height,
      child: Center(
        child: Column(
        children: [
          Padding(
            padding: const EdgeInsets.only(top: 80, bottom: 20),
            child: SizedBox(
              width: 450,
              height: 120,
              child: ShaderMask(
                shaderCallback: (bounds) {
                  return LinearGradient(
                        begin: Alignment.centerLeft,
                        end:Alignment.centerRight,
                        stops: [0.15, 0.25, 0.5, 0.75, 0.85],
                        colors: [
                          Colors.white,
                          Colors.transparent,
                          Colors.transparent,
                          Colors.transparent,
                          Colors.white,
                        ]).createShader(bounds);
                },
                blendMode: BlendMode.dstOut,
                child:
                  Row(
                    mainAxisAlignment: MainAxisAlignment.center,
                    spacing: 10,
                    children: [
                      Container(
                        decoration: BoxDecoration(
                          borderRadius: BorderRadius.circular(10),
                          color: $style.colors.surfaceVariant.withAlphaF(0.4),
                        ),
                        height: 100,
                        width: 100,
                      ),
                      Container(
                        decoration: BoxDecoration(
                          borderRadius: BorderRadius.circular(10),
                          color: $style.colors.surfaceVariant.withAlphaF(0.4),
                        ),
                        height: 120,
                        width: 200,
                        child: Center(
                          child: Container(
                              padding: EdgeInsets.all(10),
                              decoration: BoxDecoration(
                                borderRadius: BorderRadius.circular(30),
                                color: Colors.black.withAlphaF(0.8),
                              ),
                              child: Icon(
                                Icons.movie_outlined,
                                size: 30,
                                color: Colors.white.withAlphaF(0.8),
                              )),
                        ),
                      ),
                      Container(
                        decoration: BoxDecoration(
                          borderRadius: BorderRadius.circular(10),
                          color: $style.colors.surfaceVariant.withAlphaF(0.4),
                        ),
                        height: 100,
                        width: 100,
                      ),
                    ],
                  ),
              ),
            ),
          ),
          Text("Your Library is empty", style:TextStyle(fontSize: 20)),
          SizedBox(height: 10),
          Text("Find your purchases, rentals, and watchlisted\ncontent across TV, mobile, and web here", style:TextStyle(fontSize:16, color: $style.colors.onSurface.withAlphaF(0.6)), textAlign: TextAlign.center,),
        ],
      )),
    );
  }
}
