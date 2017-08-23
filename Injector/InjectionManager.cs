using System;
using Mono.Cecil;
using System.Linq;
using Mono.Cecil.Cil;
using System.Collections.Generic;

namespace MaterialColor.Injector
{
    public class InjectionManager
    {
        public InjectionManager(FileManager fileManager)
        {
            _fileManager = fileManager;
        }

        private FileManager _fileManager;

        public void InjectDefaultAndBackup(bool enableConsole)
        {
            var sourceModule = CecilHelper.GetModule(Paths.DefaultSourceAssemblyPath, Paths.ManagedDirectoryPath);
            var csharpModule = CecilHelper.GetModule(Paths.DefaultAssemblyCSharpPath, Paths.ManagedDirectoryPath);
            var firstPassModule = CecilHelper.GetModule(Paths.DefaultAssemblyFirstPassPath, Paths.ManagedDirectoryPath);

            new Injection(sourceModule, csharpModule, firstPassModule).Inject(enableConsole);

            BackupAndSaveCSharpModule(csharpModule);
            BackupAndSaveFirstPassModule(firstPassModule);
        }

        private void BackupAndSaveCSharpModule(ModuleDefinition module)
        {
            var path = Paths.DefaultAssemblyCSharpPath;
            _fileManager.MakeBackup(path);
            _fileManager.SaveModule(module, path);
        }

        private void BackupAndSaveFirstPassModule(ModuleDefinition module)
        {
            var path = Paths.DefaultAssemblyFirstPassPath;
            _fileManager.MakeBackup(path);
            _fileManager.SaveModule(module, path);
        }
    }
}
