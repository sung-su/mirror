import 'package:flutter/material.dart';

class MenuItem {
  final String title;
  final String? subtitle;
  final IconData? icon;
  final bool? Radio;
  final bool? Toggle;
  MenuItem({
    this.title = "",
    this.subtitle,
    this.icon,
    this.Radio,
    this.Toggle,
  });
}

class Menu {
  final String title;
  final List<MenuItem> items;
  Menu({this.title = "", required this.items});
}
