import 'package:flutter/material.dart';
import 'package:tizen_fs/styles/app_style.dart';

class NotificationsPanel extends StatefulWidget {
  const NotificationsPanel({super.key});

  @override
  State<NotificationsPanel> createState() => _NotificationsPanelState();
}

class _NotificationsPanelState extends State<NotificationsPanel> {
  @override
  Widget build(BuildContext context) {
    return Material(
      type: MaterialType.transparency,
      child: Stack(
        children: [
          Positioned(
            top: 30,
            left: 630,
            child: SizedBox(
              width: 300,
              height: 450,
              child: Container(
                decoration: BoxDecoration(
                  borderRadius: BorderRadius.circular(16),
                  color: Theme.of(context).colorScheme.onPrimary,
                ),
                child : Padding(
                  padding: const EdgeInsets.symmetric(horizontal: 35, vertical: 40),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    mainAxisAlignment: MainAxisAlignment.start,
                    children: [
                      Text("There are no notifications")
                    ],
                  ),
                )
              ),
            )
          )       
        ],
      ),
    );
  }
}