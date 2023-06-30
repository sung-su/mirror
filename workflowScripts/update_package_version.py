import argparse
import re
import os
import shutil
from git import Repo

VERSION_IN_SPEC_REGEX = r'[\n^](Version:\s*([0-9]+).([0-9]+).([0-9]+))'
VERSION_IN_MANIFEST_REGEX = r'\" version=\s*\"[0-9]+.[0-9]+.[0-9]+\s*\"'
SPEC_FILEPATH = 'packaging/org.tizen.cssettings.spec'
GADGET_MANIFEST_PATH = 'SettingMainGadget/SettingMainGadget/tizen-manifest.xml'
APP_MANIFEST_PATH = 'SettingView/tizen-manifest.xml'


class VersionUpdater():
    """
    Class for updating project version.
    """

    def __init__(self) -> None:
        self.update_type = None
        self.new_ver = None
        self.repo = None
        current_version = self.__get_current_version()
        if current_version is not None:
            self.major, self.minor, self.patch = current_version
            self.ver = self.__get_current_version_string()

    def print_version(self):
        """Prints the current project version.
        """
        print(f"Current project version: {self.ver}")

    def update_version(self, update_type):
        """Updates the version of the project.

        Args:
            update_type (str): Update type.
        """
        self.update_type = update_type
        self.new_ver = self.__get_new_version_string()
        print(f"Change project version from {self.ver} to: {self.new_ver}")
        self.__update_version(SPEC_FILEPATH, VERSION_IN_SPEC_REGEX)
        self.__update_version(GADGET_MANIFEST_PATH, VERSION_IN_MANIFEST_REGEX)
        self.__update_version(APP_MANIFEST_PATH, VERSION_IN_MANIFEST_REGEX)

    def commit_changes(self):
        """Commit project changes.
        """
        files_to_copy = self.__get_files_to_copy()
        if files_to_copy is None:
            print('Please build tpk and rpk files for new version.')
            return
        self.repo = Repo(os.getcwd())
        self.__remove_files()
        self.__copy_files(files_to_copy)
        if not self.__commit_files():
            print('Can\'t find files to commit')
            return
        self.repo.git.commit('-m', f'Version {self.__get_current_version_string()}')

    def __find_file_fullpath(self, filename_to_find):
        scanned = os.walk('.')
        for dir_path, _, filenames in scanned:
            for filename in filenames:
                if filename == filename_to_find:
                    return os.path.join(dir_path, filename)
        return None

    def __get_files_to_copy(self):
        filenames_to_copy = [
            f'org.tizen.cssettings-{self.ver}.tpk',
            f'org.tizen.settings.main-{self.ver}.rpk'
        ]
        files_to_copy = [self.__find_file_fullpath(filename) for filename in filenames_to_copy]

        if self.__check_if_files_exist(files_to_copy):
            return files_to_copy
        return None

    def __copy_files(self, files_to_copy):
        for file in files_to_copy:
            shutil.copy(file, 'packaging')

    def __remove_files(self):
        files_to_remove = []
        for dir_path, _, filename in os.walk('packaging'):
            files_to_remove.extend(os.path.join(dir_path, f)
                                   for f in filename if os.path.splitext(f)[1] in ['.tpk', '.rpk'])
        if len(files_to_remove) > 0:
            for file in files_to_remove:
                os.remove(file)
            self.repo.git.rm(files_to_remove)

    def __commit_files(self):
        files_to_commit = [
            APP_MANIFEST_PATH,
            GADGET_MANIFEST_PATH,
            SPEC_FILEPATH,
            os.path.join('packaging',
                         f'org.tizen.cssettings-{self.__get_current_version_string()}.tpk'),
            os.path.join('packaging',
                         f'org.tizen.settings.main-{self.__get_current_version_string()}.rpk'),
        ]
        if self.__check_if_files_exist(files_to_commit):
            self.repo.git.add(files_to_commit)
            return True
        return False

    def __check_if_files_exist(self, files):
        for file in files:
            if not os.path.exists(file) or not os.path.isfile(file):
                print(f'File: {file} doesn\'t exist.')
                return False
        return True

    def __update_version(self, file_path, regex):
        file_content = self.__get_file_content(file_path)
        result = re.search(regex, file_content)
        if result is not None:
            self.__replace(file_path, result.group(0),
                           result.group(0).replace(self.ver, self.new_ver))

    def __get_current_version_string(self):
        return '.'.join([self.major, self.minor, self.patch])

    def __get_new_version_string(self):
        if self.update_type == 'patch':
            return '.'.join([self.major, self.minor, str(int(self.patch) + 1)])
        if self.update_type == 'minor':
            return '.'.join([self.major, str(int(self.minor) + 1), "0"])
        return '.'.join([str(int(self.major) + 1), "0", "0"])

    def __replace(self, file, search, replace):
        with open(file, 'r', newline='', encoding='utf8') as filestream:
            data = filestream.read()
            data = data.replace(search, replace)
        with open(file, 'w', newline='', encoding='utf8') as filestream:
            filestream.write(data)

    def __get_current_version(self):
        spec_content = self.__get_file_content(SPEC_FILEPATH)
        result = re.search(VERSION_IN_SPEC_REGEX, spec_content)
        if result is not None:
            return result.group(2), result.group(3), result.group(4)
        return None

    def __get_file_content(self, file):
        with open(file, 'r', encoding='utf8') as filestream:
            file_content = filestream.read()
        return file_content


def main():
    """The main function.
    """
    parser = argparse.ArgumentParser(
        prog="update_package_version",
        description="Update the project version and commit changes.",
    )
    parser.add_argument('-v', '--version', action="store_true", help='get current project version')
    subparsers = parser.add_subparsers(title='subcommands', dest='command')
    update_parser = subparsers.add_parser('update', help='update the project version')
    subparsers.add_parser('commit', help='git commit changes')
    update_parser.add_argument('-t', '--type',
                               default='patch',
                               choices=['major', 'minor', 'patch'],
                               help='update type (default: %(default)s)')
    args = parser.parse_args()

    version_updater = VersionUpdater()
    if args.version:
        version_updater.print_version()
    elif args.command is None:
        parser.print_help()
    else:
        if args.command == 'update':
            version_updater.update_version(args.type)
        elif args.command == 'commit':
            version_updater.commit_changes()


if __name__ == "__main__":
    main()
