import 'package:flutter/material.dart';
import 'package:shimmer/shimmer.dart';

class ShimmerLoadingScreen extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: FutureBuilder(
        future: fetchData(),
        builder: (context, snapshot) {
          if (snapshot.connectionState == ConnectionState.waiting) {
            return ShimmerScreen(); // Shimmer effect while loading
          }
          else if (snapshot.hasError) {
            return Center(
              child: Text('Error: ${snapshot.error}'),
            );
          } else {
            return Screen(data: snapshot.data!);
          }
        },
      ),
    );
  }

  Future<List<String>> fetchData() async {
    // Simulate data fetching delay
    await Future.delayed(Duration(seconds: 5));
    return ['Item 1', 'Item 2', 'Item 3'];
  }
}

class ShimmerScreen extends StatelessWidget {

  @override
  Widget build(BuildContext context) {
    return Shimmer.fromColors(
      baseColor: Theme.of(context).colorScheme.onPrimary,
      highlightColor: Theme.of(context).colorScheme.primaryContainer,
      child: DummyScreen()
    );
  }
}

class Screen extends StatelessWidget {
  final List<String> data;

  Screen({required this.data});

  @override
  Widget build(BuildContext context) {
    return DummyScreen();
  }
}

class DummyScreen extends StatelessWidget {

  @override
  Widget build(BuildContext context) {
    final Color baseColor = Theme.of(context).colorScheme.onPrimary;

    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 50, vertical: 20),
      child: Column(
        spacing: 20,
        children: [
          Row(
            spacing: 10,
            children: [
              Container (
                width: 40,
                height: 40,
                decoration: BoxDecoration(
                  shape: BoxShape.circle,
                  color: baseColor
                )
              ),
              Container (
                width: 100,
                height: 40,
                decoration: BoxDecoration(
                  borderRadius: BorderRadius.circular(24),
                  color: baseColor
                )
              ),
              Container (
                width: 100,
                height: 40,
                decoration: BoxDecoration(
                  borderRadius: BorderRadius.circular(24),
                  color: baseColor
                )
              ),
              Spacer(),
              Container (
                width: 40,
                height: 40,
                decoration: BoxDecoration(
                  shape: BoxShape.circle,
                  color: baseColor
                )
              ),
              Container (
                width: 40,
                height: 40,
                decoration: BoxDecoration(
                  shape: BoxShape.circle,
                  color: baseColor
                )
              ),
              Container (
                width: 80,
                height: 30,
                decoration: BoxDecoration(
                  borderRadius: BorderRadius.circular(10),
                  color: baseColor
                )
              ),
            ],
          ),
          Row(
            children: [
              Column(
                spacing: 10,
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  SizedBox(height: 150),
                  Container(
                    width: 250,
                    height: 30,
                    decoration: BoxDecoration(
                      borderRadius: BorderRadius.circular(10),
                      color: baseColor
                    )
                  ),
                  Container(
                    width: 400,
                    height: 15,
                    decoration: BoxDecoration(
                      borderRadius: BorderRadius.circular(10),
                      color: baseColor
                    )
                  ),
                  SizedBox(height: 10),
                  Container(
                    width: 100,
                    height: 40,
                    decoration: BoxDecoration(
                      borderRadius: BorderRadius.circular(24),
                      color: baseColor
                    )
                  )
                ],
              )
            ],
          ),
          SizedBox(height: 10),
          Row(
            spacing: 20,
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Container(
                width: 168,
                height: 78,
                decoration: BoxDecoration(
                  borderRadius: BorderRadius.circular(16),
                  color: baseColor
                )
              ),
              Container(
                width: 168,
                height: 78,
                decoration: BoxDecoration(
                  borderRadius: BorderRadius.circular(16),
                  color: baseColor
                )
              ),
              Container(
                width: 168,
                height: 78,
                decoration: BoxDecoration(
                  borderRadius: BorderRadius.circular(16),
                  color: baseColor
                )
              ),
            ],
          ),
        ],
      ),
    );
  }
}