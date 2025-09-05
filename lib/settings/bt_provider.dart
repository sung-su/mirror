
import 'package:flutter/material.dart';

class BluetoothProvider extends ChangeNotifier {
  bool _isInitialized = false;
  bool get isInitialized => _isInitialized;
  void initialize(bool value) {
    if(_isInitialized != value) {
      _isInitialized = value;
      notifyListeners();
    }
  }
}
