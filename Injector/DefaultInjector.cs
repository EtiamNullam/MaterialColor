using System;
using Mono.Cecil;
using System.Linq;
using Mono.Cecil.Cil;
using System.Collections.Generic;

namespace MaterialColor.Injector
{
    //TODO: refactor
    public class DefaultInjector
    {
        public DefaultInjector(FileManager fileManager)
        {
            _fileManager = fileManager;
        }

        private FileManager _fileManager;

        // TODO: refactor
        public void InjectDefaultAndBackup(bool enableConsole)
        {
            //var modifiedCSharpModule = InjectDefault();

            InjectDefault(out var modifiedCSharpModule, out var modifiedFirstPassModule);

            if (enableConsole)
            {
                EnableConsole(modifiedCSharpModule);
            }

            var assemblyCSharpPath = Paths.DefaultAssemblyCSharpPath;
            var assemblyFirstPassPath = Paths.DefaultAssemblyFirstPassPath;

            _fileManager.MakeBackup(assemblyCSharpPath);
            _fileManager.SaveModule(modifiedCSharpModule, assemblyCSharpPath);

            _fileManager.MakeBackup(assemblyFirstPassPath);
            _fileManager.SaveModule(modifiedFirstPassModule, assemblyFirstPassPath);
        }

        // TODO: refactor
        private void InjectDefault(out ModuleDefinition csharpModule, out ModuleDefinition firstPassModule)
        {
            var sourceModule = CecilHelper.GetModule(Paths.DefaultSourceAssemblyPath, Paths.ManagedDirectoryPath);
            csharpModule = CecilHelper.GetModule(Paths.DefaultAssemblyCSharpPath, Paths.ManagedDirectoryPath);
            firstPassModule = CecilHelper.GetModule(Paths.DefaultAssemblyFirstPassPath, Paths.ManagedDirectoryPath);

            MethodInjectorHelper.InjectAsFirstInstruction(
                sourceModule,
                csharpModule,
                "InjectionEntry", "EnterOnce",
                "Game", "OnPrefabInit");

            // inject before instruction #5
             MethodInjectorHelper.InjectBefore(
                sourceModule,
                csharpModule,
                "InjectionEntry", "EnterEveryUpdate",
                "Game", "Update",
                5);

            // GetCellColor test - correct
             InstructionRemoveHelper.ClearAllButLast(
                csharpModule,
                "BlockTileRenderer", "GetCellColor");

             MethodInjectorHelper.InjectAsFirstInstruction(
                sourceModule,
                csharpModule,
                "InjectionEntry", "EnterCell",
                "BlockTileRenderer", "GetCellColor",
                true, 1);

            PublishHelper.MakeFieldPublic(csharpModule, "BlockTileRenderer", "selectedCell");
            PublishHelper.MakeFieldPublic(csharpModule, "BlockTileRenderer", "highlightCell");
            //

            PublishHelper.MakeFieldPublic(csharpModule, "Ownable", "unownedTint");
            PublishHelper.MakeFieldPublic(csharpModule, "Ownable", "ownedTint");

            // storagelocker dim on load test - not really needed but should be left
            PublishHelper.MakeFieldPublic(csharpModule, "StorageLocker", "filterable");
            PublishHelper.MakeMethodPublic(csharpModule, "StorageLocker", "OnFilterChanged");
            // ownable dim properly test - correct
            PublishHelper.MakeMethodPublic(csharpModule, "Ownable", "UpdateTint");
            // fridge/rationbox dim properly test - probably correct
            PublishHelper.MakeFieldPublic(csharpModule, "Refrigerator", "filterable");
            PublishHelper.MakeMethodPublic(csharpModule, "Refrigerator", "OnFilterChanged");
            PublishHelper.MakeFieldPublic(csharpModule, "RationBox", "filterable");
            PublishHelper.MakeMethodPublic(csharpModule, "RationBox", "OnFilterChanged");
            //
            InjectKeybindings(csharpModule, firstPassModule);
            //
            AddLocalizationString(sourceModule, csharpModule);
            //
            AddOverlayButton(csharpModule);
            //
            AttachCustomActionToToggle(sourceModule, csharpModule);
            //
        }

        private void InjectKeybindings(ModuleDefinition csharpModule, ModuleDefinition firstPassModule)
        {
            AddDefaultKeybinding(csharpModule, firstPassModule, KKeyCode.F6, Modifier.Alt, Action.Overlay12);
        }

        // TODO: refactor, test
        private void AddOverlayButton(ModuleDefinition csharpModule)
        {
            var overlayMenu = csharpModule.Types.First(type => type.Name == "OverlayMenu");

            var initializeTogglesMethod = overlayMenu.Methods.First(method => method.Name == "InitializeToggles");

            var lastAddInstruction = initializeTogglesMethod.Body.Instructions.Last(instruction => instruction.OpCode == OpCodes.Callvirt);
            var newToggleInfoInstruction = initializeTogglesMethod.Body.Instructions.Last(instruction => instruction.OpCode == OpCodes.Newobj);

            // not used
            var locStringToStringInstruction = initializeTogglesMethod.Body.Instructions.Last(instruction => instruction.OpCode == OpCodes.Call);

            // test
            var toggleInfoConstructor = csharpModule.Types.First(type => type.Name == "KIconToggleMenu")
                .NestedTypes.First(type => type.Name == "ToggleInfo")
                .Methods.First(method => method.Name == ".ctor");
            //

            // is it neccesary?
            var boxToSimViewMode = initializeTogglesMethod.Body.Instructions.Last(instruction => instruction.OpCode == OpCodes.Box);

            var buttonInstructions = new List<Instruction>();

            // TODO: remove "overlay" from below as its not overlay technically its whole mod toggle
            buttonInstructions.Add(Instruction.Create(OpCodes.Ldloc_0));
            buttonInstructions.Add(Instruction.Create(OpCodes.Ldstr, "MaterialColor Overlay"));
            //buttonInstructions.Add(locStringToStringInstruction); above is not a LocString
            buttonInstructions.Add(Instruction.Create(OpCodes.Ldstr, "overlay_materialcolor")); // probably wrong, reuse other sprite
            //buttonInstructions.Add(Instruction.Create(OpCodes.Ldc_I4, (int) SimViewMode.TileType)); already used SimViewMode
            //buttonInstructions.Add(Instruction.Create(OpCodes.Ldc_I4, (int)SimViewMode.Reserved)); better way, see below
            //buttonInstructions.Add(Instruction.Create(OpCodes.Ldstr, "ToggleMaterialColorOverlayMessage")); //doesnt work, see below
            buttonInstructions.Add(Instruction.Create(OpCodes.Ldc_I4, Common.IDs.ToggleMaterialColorOverlayID));
            buttonInstructions.Add(boxToSimViewMode);
            buttonInstructions.Add(Instruction.Create(OpCodes.Ldc_I4, (int)Action.Overlay12));
            buttonInstructions.Add(Instruction.Create(OpCodes.Ldstr, "Toggles MaterialColor overlay"));
            buttonInstructions.Add(Instruction.Create(OpCodes.Newobj, toggleInfoConstructor));
            buttonInstructions.Add(lastAddInstruction);

            buttonInstructions.Reverse();

            var ILProcessor = initializeTogglesMethod.Body.GetILProcessor();

            foreach (var instruction in buttonInstructions)
            {
                ILProcessor.InsertAfter(lastAddInstruction, instruction);
            }
        }

        // TODO: refactor
        private void AddDefaultKeybinding(ModuleDefinition CSharpModule, ModuleDefinition FirstPassModule, KKeyCode keyCode, Modifier keyModifier, Action action, string screen = "Root")
        {
            var beforeFieldInit = CecilHelper.GetMethodDefinition(FirstPassModule, CecilHelper.GetTypeDefinition(FirstPassModule, "GameInputMapping"), ".cctor");

            var lastKeybindingDeclarationEnd = beforeFieldInit.Body.Instructions.Last(instruction => instruction.OpCode == OpCodes.Stobj);
            var stoBindingEntryInstruction = beforeFieldInit.Body.Instructions.First(instruction => instruction.OpCode == OpCodes.Stobj);
            var newBindingEntryInstruction = beforeFieldInit.Body.Instructions.First(instruction => instruction.OpCode == OpCodes.Newobj);

            var keybindingInstructions = new List<Instruction>();

            keybindingInstructions.Add(Instruction.Create(OpCodes.Dup));
            keybindingInstructions.Add(Instruction.Create(OpCodes.Ldc_I4_S, (sbyte) 0x6C)); // index 
            keybindingInstructions.Add(Instruction.Create(OpCodes.Ldelema, (TypeReference)stoBindingEntryInstruction.Operand));
            keybindingInstructions.Add(Instruction.Create(OpCodes.Ldstr, screen));
            keybindingInstructions.Add(Instruction.Create(OpCodes.Ldc_I4_S, (sbyte)16)); // gamepad button
            keybindingInstructions.Add(Instruction.Create(OpCodes.Ldc_I4, (int)keyCode));
            keybindingInstructions.Add(Instruction.Create(OpCodes.Ldc_I4, (int)keyModifier));
            keybindingInstructions.Add(Instruction.Create(OpCodes.Ldc_I4, (int)action));
            keybindingInstructions.Add(Instruction.Create(OpCodes.Ldc_I4_1)); // rebindable = true
            keybindingInstructions.Add(newBindingEntryInstruction); // create new object
            keybindingInstructions.Add(stoBindingEntryInstruction); // store in array?

            keybindingInstructions.Reverse();

            var ILProcessor = beforeFieldInit.Body.GetILProcessor();

            // increase array size by one
            var firstInstruction = beforeFieldInit.Body.Instructions.First();

            ILProcessor.Replace(firstInstruction, Instruction.Create(OpCodes.Ldc_I4_S, (sbyte)0x6D));
            //

            foreach (var instruction in keybindingInstructions)
            {
                ILProcessor.InsertAfter(lastKeybindingDeclarationEnd, instruction);
            }
        }

        // use only one entry point for initialization?
        private void AddLocalizationString(ModuleDefinition sourceModule, ModuleDefinition csharpModule)
        {
            var inputBindings = csharpModule
                .Types
                .First(type => type.Name == "INPUT_BINDINGS")
                    .NestedTypes
                    .First(type => type.Name == "ROOT");

            var fieldDefinition = new FieldDefinition(
                    "OVERLAY12",
                    FieldAttributes.Static | FieldAttributes.Public,
                    CecilHelper.GetTypeDefinition(csharpModule, "LocString")
                );

            inputBindings.Fields.Add(fieldDefinition);

            MethodInjectorHelper.InjectAsFirstInstruction(
                sourceModule,
                csharpModule,
                "InjectionEntry",
                "SetLocalizationString",
                "GlobalAssets",
                "Awake");
        }

        /*
         * is disabled in:
         * FrontEndManager.LateUpdate()
         * Game.Update()
         * GraphicsOptionsScreen.Update()
         * OptionsMenuScreen.Update()
         * ReportErrorDialog.Update()
         */
        private void EnableConsole(ModuleDefinition module)
        {
            InstructionRemoveHelper.RemoveInstructionAt(module, "FrontEndManager", "LateUpdate", 0);
            InstructionRemoveHelper.RemoveInstructionAt(module, "FrontEndManager", "LateUpdate", 0);
            InstructionRemoveHelper.RemoveInstructionAt(module, "FrontEndManager", "LateUpdate", 0);
            InstructionRemoveHelper.RemoveInstructionAt(module, "FrontEndManager", "LateUpdate", 0);

            InstructionRemoveHelper.RemoveInstructionAt(module, "Game", "Update", 9);
            InstructionRemoveHelper.RemoveInstructionAt(module, "Game", "Update", 9);
            InstructionRemoveHelper.RemoveInstructionAt(module, "Game", "Update", 9);
            InstructionRemoveHelper.RemoveInstructionAt(module, "Game", "Update", 9);
        }

        // TODO: refactor
        private void AttachCustomActionToToggle(ModuleDefinition sourceModule, ModuleDefinition csharpModule)
        {
            var OnToggleSelectMethod = csharpModule.Types.First(type => type.Name == "OverlayMenu").Methods.First(method => method.Name == "OnToggleSelect");

            var firstInstruction = OnToggleSelectMethod.Body.Instructions.First();

            var instructionsToAdd = new List<Instruction>();

            instructionsToAdd.Add(Instruction.Create(OpCodes.Brfalse, firstInstruction));
            instructionsToAdd.Add(Instruction.Create(OpCodes.Ret));

            var ILProcessor = OnToggleSelectMethod.Body.GetILProcessor();

            foreach (var instruction in instructionsToAdd)
            {
                ILProcessor.InsertBefore(firstInstruction, instruction);
            }

            MethodInjectorHelper.InjectAsFirstInstruction(
                sourceModule,
                csharpModule,
                "InjectionEntry",
                "EnterToggle",
                "OverlayMenu",
                "OnToggleSelect",
                true, 1);
        }
    }
}
