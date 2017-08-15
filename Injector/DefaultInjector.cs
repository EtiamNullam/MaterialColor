﻿using System;
using Mono.Cecil;

namespace Injector
{
    public class DefaultInjector
    {
        public DefaultInjector(FileManager fileManager)
        {
            _fileManager = fileManager;
        }

        private FileManager _fileManager;

        public void InjectDefaultAndBackup(bool enableConsole)
        {
            var modifiedModule = InjectDefault();

            if (enableConsole)
            {
                modifiedModule = EnableConsole(modifiedModule);
            }

            var path = DefaultPaths.DefaultTargetAssemblyPath;

            _fileManager.MakeBackup(path);
            _fileManager.SaveModule(modifiedModule, path);
        }

        private ModuleDefinition InjectDefault()
        {
            var sourceModule = CecilHelper.GetModule(DefaultPaths.DefaultSourceAssemblyPath);
            var targetModule = CecilHelper.GetModule(DefaultPaths.DefaultTargetAssemblyPath);

            targetModule = MethodInjectorHelper.InjectAsFirstInstruction(
                sourceModule,
                targetModule,
                "InjectionEntry", "EnterOnce", 
                "Game", "OnPrefabInit");

            // inject before instruction #5
            targetModule = MethodInjectorHelper.InjectBefore(
                sourceModule,
                targetModule,
                "InjectionEntry", "EnterEveryUpdate",
                "Game", "Update",
                5);

            // GetCellColor test - correct
            targetModule = InstructionRemoveHelper.ClearAllButLast(
                targetModule,
                "BlockTileRenderer", "GetCellColor");

            targetModule = MethodInjectorHelper.InjectAsFirstInstruction(
                sourceModule,
                targetModule,
                "InjectionEntry", "EnterCell",
                "BlockTileRenderer", "GetCellColor",
                true, 1);

            targetModule = PublishHelper.MakeFieldPublic(targetModule, "BlockTileRenderer", "selectedCell");
            targetModule = PublishHelper.MakeFieldPublic(targetModule, "BlockTileRenderer", "highlightCell");
            //

            targetModule = PublishHelper.MakeFieldPublic(targetModule, "Ownable", "unownedTint");
            targetModule = PublishHelper.MakeFieldPublic(targetModule, "Ownable", "ownedTint");

            // storagelocker dim on load test - not really needed but should be left
            targetModule = PublishHelper.MakeFieldPublic(targetModule, "StorageLocker", "filterable");
            targetModule = PublishHelper.MakeMethodPublic(targetModule, "StorageLocker", "OnFilterChanged");
            // ownable dim properly test - correct
            targetModule = PublishHelper.MakeMethodPublic(targetModule, "Ownable", "UpdateTint");
            // fridge/rationbox dim properly test- not really needed but should be left
            targetModule = PublishHelper.MakeFieldPublic(targetModule, "Refrigerator", "filterable");
            targetModule = PublishHelper.MakeMethodPublic(targetModule, "Refrigerator", "OnFilterChanged");
            targetModule = PublishHelper.MakeFieldPublic(targetModule, "RationBox", "filterable");
            targetModule = PublishHelper.MakeMethodPublic(targetModule, "RationBox", "OnFilterChanged");
            //

            return targetModule;
        }

        /*
         * is disabled in:
         * FrontEndManager.LateUpdate()
         * Game.Update()
         * GraphicsOptionsScreen.Update()
         * OptionsMenuScreen.Update()
         * ReportErrorDialog.Update()
         */
        private ModuleDefinition EnableConsole(ModuleDefinition module)
        {
            module = InstructionRemoveHelper.RemoveInstructionAt(module, "Game", "Update", 9);
            module = InstructionRemoveHelper.RemoveInstructionAt(module, "Game", "Update", 9);
            module = InstructionRemoveHelper.RemoveInstructionAt(module, "Game", "Update", 9);
            module = InstructionRemoveHelper.RemoveInstructionAt(module, "Game", "Update", 9);

            return module;
        }

        [Obsolete]
        private static void CustomInjection()
        {
            //Console.Write("From module: ");
            //var from = Console.ReadLine();

            //Console.Write("To module: ");
            //var to = Console.ReadLine();

            //MethodInjector injector;

            //try
            //{
            //    injector = new MethodInjector(from, to);
            //}
            //catch (Exception e)
            //{
            //    ShowException(e);
            //    return;
            //}

            //Console.Write("From type name: ");
            //var fromTypeName = Console.ReadLine();

            //Console.Write("From method name: ");
            //var fromMethodName = Console.ReadLine();

            //Console.Write("To type name: ");
            //var toTypeName = Console.ReadLine();

            //Console.Write("To method name: ");
            //var toMethodName = Console.ReadLine();

            //injector.InjectAsFirstInstruction(fromTypeName, fromMethodName, toTypeName, toMethodName);
            ////injector.InjectAsFirstInstructionAtEntryPoint(fromTypeName, fromMethodName);

            //var fileManager = new FileManager();

            //fileManager.MakeBackup(to);

            //try
            //{
            //    fileManager.SaveModule(injector.ToModule, to);
            //}
            //catch (Exception e)
            //{
            //    ShowException(e);
            //}
        }
    }
}
