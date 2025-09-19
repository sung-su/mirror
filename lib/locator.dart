import 'package:get_it/get_it.dart';
import 'package:tizen_fs/models/app_data_model.dart';
// import 'package:tizen_fs/models/bt_model.dart';

final getIt = GetIt.instance;

void setupAppModel() {
  // if (!getIt.isRegistered<BtModel>()) {
    getIt.registerLazySingleton<AppDataModel>(() => AppDataModel());
  // }
}

// void setupBtModel() {
//   if (!getIt.isRegistered<BtModel>()) {
//     getIt.registerLazySingleton<BtModel>(() => BtModel.init());
//   }
// }