using System;
using Mono.Cecil;
using System.Linq;
using Mono.Cecil.Cil;
using System.Collections.Generic;
using MaterialColor.Injector.IO;

namespace MaterialColor.Injector
{
    public class InjectionManager
    {
        public InjectionManager(FileManager fileManager)
        {
            _fileManager = fileManager;
        }

        private FileManager _fileManager;

        public void InjectDefaultAndBackup(bool injectMaterial, bool enableConsole, bool injectOnion)
        {
            var materialModule = CecilHelper.GetModule(Paths.DefaultMaterialAssemblyPath, Paths.ManagedDirectoryPath);
            var onionModule = CecilHelper.GetModule(Paths.DefaultOnionAssemblyPath, Paths.ManagedDirectoryPath);
            var csharpModule = CecilHelper.GetModule(Paths.DefaultAssemblyCSharpPath, Paths.ManagedDirectoryPath);
            var firstPassModule = CecilHelper.GetModule(Paths.DefaultAssemblyFirstPassPath, Paths.ManagedDirectoryPath);

            new Injection(materialModule, onionModule, csharpModule, firstPassModule).Inject(injectMaterial, enableConsole, injectOnion);

            BackupAndSaveCSharpModule(csharpModule);
            BackupAndSaveFirstPassModule(firstPassModule);
        }

        public bool IsCurrentAssemblyCSharpPatched()
            => CecilHelper.GetModule(Paths.DefaultAssemblyCSharpPath, Paths.ManagedDirectoryPath).Types.Any(TypePatched);

        public bool IsCurrentAssemblyFirstpassPatched()
            => CecilHelper.GetModule(Paths.DefaultAssemblyFirstPassPath, Paths.ManagedDirectoryPath).Types.Any(TypePatched);

        private bool TypePatched(TypeDefinition type)
        {
            return type.Namespace == "Mods" && type.Name == "Patched";
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
