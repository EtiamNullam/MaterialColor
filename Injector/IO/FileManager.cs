using System.IO;
using Mono.Cecil;

namespace Injector.IO
{
    public class FileManager
    {
        public const string BackupString = ".backup";

        public void MakeBackup(string filePath)
        {
            var backupPath = GetBackupPathForFile(filePath);

            if (BackupForFileExists(filePath))
            {
                File.Delete(backupPath);
            }

            File.Move(filePath, backupPath);
        }

        public bool BackupForFileExists(string filePath)
            => File.Exists(GetBackupPathForFile(filePath));

        private string GetBackupPathForFile(string filePath)
            => filePath + BackupString;

        public bool RestoreBackupForFile(string filePath)
        {
            var backupPath = GetBackupPathForFile(filePath);
            var backupExists = BackupForFileExists(filePath);
            var pathBlocked = File.Exists(filePath);

            if (backupExists)
            {
                if (pathBlocked)
                {
                    File.Delete(filePath);
                }

                File.Move(backupPath, filePath);

                return true;
            }
            else return false;
        }

        public void SaveModule(ModuleDefinition module, string filePath)
        {
            module.Write(filePath);
        }
    }
}
