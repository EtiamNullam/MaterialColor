using Mono.Cecil;
using System.Linq;
using Injector.IO;
using Common.Data;

namespace Injector
{
    public class InjectionManager
    {
        public InjectionManager(FileManager fileManager)
        {
            _fileManager = fileManager;
        }

        private readonly FileManager _fileManager;

        public Common.IO.Logger Logger { get; set; }

        public bool InjectDefaultAndBackup(InjectorState injectorState)
        {
            var coreModule = CecilHelper.GetModule(Paths.DefaultCoreAssemblyPath, Paths.ManagedDirectoryPath);
            var materialModule = CecilHelper.GetModule(Paths.DefaultMaterialAssemblyPath, Paths.ManagedDirectoryPath);
            var onionModule = CecilHelper.GetModule(Paths.DefaultOnionAssemblyPath, Paths.ManagedDirectoryPath);

            var csharpModule = CecilHelper.GetModule(Paths.DefaultAssemblyCSharpPath, Paths.ManagedDirectoryPath);
            var firstPassModule = CecilHelper.GetModule(Paths.DefaultAssemblyFirstPassPath, Paths.ManagedDirectoryPath);

            var injection = new Injection(coreModule, materialModule, onionModule, csharpModule, firstPassModule)
            {
                Logger = Logger
            };

            injection.Inject(injectorState);

            BackupAndSaveCSharpModule(csharpModule);
            BackupAndSaveFirstPassModule(firstPassModule);

            return injection.Failed;
        }

        public bool IsCurrentAssemblyCSharpPatched()
            => CecilHelper.GetModule(Paths.DefaultAssemblyCSharpPath, Paths.ManagedDirectoryPath).Types.Any(TypePatched);

        public bool IsCurrentAssemblyFirstpassPatched()
            => CecilHelper.GetModule(Paths.DefaultAssemblyFirstPassPath, Paths.ManagedDirectoryPath).Types.Any(TypePatched);

        private static bool TypePatched(TypeDefinition type)
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
