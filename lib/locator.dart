import 'package:get_it/get_it.dart';
import 'package:tizen_fs/models/app_data_model.dart';

final getIt = GetIt.instance;

void setupAppModel() {
  getIt.registerLazySingleton<AppDataModel>(() => AppDataModel());
}
