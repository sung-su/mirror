import 'package:flutter/material.dart';

class BackdropProvider extends ChangeNotifier {
  Widget? _backdrop;
  Widget? get backdrop => _backdrop;

  void updateBackdrop(Widget backdrop) {
    _backdrop = backdrop;
    notifyListeners();
  }
}
