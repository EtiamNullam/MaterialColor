using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaterialColor.Injector
{
    public class Injection
    {
        public Injection(ModuleDefinition sourceModule, ModuleDefinition csharpModule, ModuleDefinition firstPassModule)
        {
            Initialize(sourceModule, csharpModule, firstPassModule);
        }

        private ModuleDefinition _sourceModule;
        private ModuleDefinition _csharpModule;
        private ModuleDefinition _firstPassModule;

        private MethodInjector _sourceToCSharpInjector;
        private InstructionRemover _csharpInstructionRemover;
        private Publisher _csharpPublisher;

        private void Initialize(ModuleDefinition sourceModule, ModuleDefinition csharpModule, ModuleDefinition firstPassModule)
        {
            _sourceModule = sourceModule;
            _csharpModule = csharpModule;
            _firstPassModule = firstPassModule;

            _csharpInstructionRemover = new InstructionRemover(csharpModule);
            _csharpPublisher = new Publisher(csharpModule);
            _csharpInstructionRemover = new InstructionRemover(csharpModule);
            _sourceToCSharpInjector = new MethodInjector(sourceModule, csharpModule);
        }

        public void Inject(bool enableConsole)
        {
            if (enableConsole)
            {
                EnableConsole();
            }

            // sprite test
            //_csharpPublisher.MakeFieldPublic("KIconToggleMenu", "icons");
            //_csharpPublisher.MakeMethodPublic("KIconToggleMenu", "RefreshButtons");
            //

            InjectMain();
            InjectCellColorHandling();
            InjectBuildingsSpecialCasesHandling();
            InjectToggleButton();
        }

        private void InjectMain()
        {
            _sourceToCSharpInjector.InjectAsFirstInstruction(
                 "InjectionEntry", "EnterOnce",
                "Game", "OnPrefabInit");

            _sourceToCSharpInjector.InjectBefore(
               "InjectionEntry", "EnterEveryUpdate",
               "Game", "Update",
               5);
        }

        private void InjectCellColorHandling()
        {
            _csharpInstructionRemover.ClearAllButLast("BlockTileRenderer", "GetCellColor");

            _sourceToCSharpInjector.InjectAsFirstInstruction(
                "InjectionEntry", "EnterCell",
                "BlockTileRenderer", "GetCellColor",
                true, 1);

            _csharpPublisher.MakeFieldPublic("BlockTileRenderer", "selectedCell");
            _csharpPublisher.MakeFieldPublic("BlockTileRenderer", "highlightCell");
        }

        private void InjectBuildingsSpecialCasesHandling()
        {
            _csharpPublisher.MakeFieldPublic("Ownable", "unownedTint");
            _csharpPublisher.MakeFieldPublic("Ownable", "ownedTint");

            _csharpPublisher.MakeMethodPublic("Ownable", "UpdateTint");

            _csharpPublisher.MakeFieldPublic("FilteredStorage", "filterTint");
            _csharpPublisher.MakeFieldPublic("FilteredStorage", "noFilterTint");

            _csharpPublisher.MakeFieldPublic("FilteredStorage", "filterable");

            _csharpPublisher.MakeFieldPublic("StorageLocker", "filteredStorage");
            _csharpPublisher.MakeFieldPublic("Refrigerator", "filteredStorage");
            _csharpPublisher.MakeFieldPublic("RationBox", "filteredStorage");
        }

        private void InjectToggleButton()
        {
            InjectKeybindings();
            AddLocalizationString();
            AddOverlayButton();
            AttachCustomActionToToggle();
        }

        private void InjectKeybindings()
        {
            AddDefaultKeybinding(_csharpModule, _firstPassModule, KKeyCode.F6, Modifier.Alt, Action.Overlay12);
        }

        // make it read and increase index, instead of hard values
        private void AddDefaultKeybinding(ModuleDefinition CSharpModule, ModuleDefinition FirstPassModule, KKeyCode keyCode, Modifier keyModifier, Action action, string screen = "Root")
        {
            var beforeFieldInit = CecilHelper.GetMethodDefinition(FirstPassModule, CecilHelper.GetTypeDefinition(FirstPassModule, "GameInputMapping"), ".cctor");

            var lastKeybindingDeclarationEnd = beforeFieldInit.Body.Instructions.Last(instruction => instruction.OpCode == OpCodes.Stobj);
            var stoBindingEntryInstruction = beforeFieldInit.Body.Instructions.First(instruction => instruction.OpCode == OpCodes.Stobj);
            var newBindingEntryInstruction = beforeFieldInit.Body.Instructions.First(instruction => instruction.OpCode == OpCodes.Newobj);

            var instructionsToAdd = new List<Instruction>
            {
                Instruction.Create(OpCodes.Dup),
                Instruction.Create(OpCodes.Ldc_I4_S, (sbyte)0x6E), // index 
                Instruction.Create(OpCodes.Ldelema, (TypeReference)stoBindingEntryInstruction.Operand),
                Instruction.Create(OpCodes.Ldstr, screen),
                Instruction.Create(OpCodes.Ldc_I4_S, (sbyte)16), // gamepad button
                Instruction.Create(OpCodes.Ldc_I4, (int)keyCode),
                Instruction.Create(OpCodes.Ldc_I4, (int)keyModifier),
                Instruction.Create(OpCodes.Ldc_I4, (int)action),
                Instruction.Create(OpCodes.Ldc_I4_1), // rebindable = true
                newBindingEntryInstruction, // create new object
                stoBindingEntryInstruction // store in array
            };

            var ILProcessor = beforeFieldInit.Body.GetILProcessor();

            // increase array size by one
            var firstInstruction = beforeFieldInit.Body.Instructions.First();

            //ILProcessor.Replace(firstInstruction, Instruction.Create(OpCodes.Ldc_I4_S, (sbyte)0x6D)); // old version
            ILProcessor.Replace(firstInstruction, Instruction.Create(OpCodes.Ldc_I4_S, (sbyte)0x6F));
            //
            new InstructionInserter(ILProcessor).InsertAfter(lastKeybindingDeclarationEnd, instructionsToAdd);
        }

        // TODO: use other sprite, refactor
        private void AddOverlayButton()
        {
            var overlayMenu = _csharpModule.Types.First(type => type.Name == "OverlayMenu");
            var initializeTogglesMethod = overlayMenu.Methods.First(method => method.Name == "InitializeToggles");
            var lastAddInstruction = initializeTogglesMethod.Body.Instructions.Last(instruction => instruction.OpCode == OpCodes.Callvirt);

            var toggleInfoConstructor = _csharpModule.Types.First(type => type.Name == "KIconToggleMenu")
                .NestedTypes.First(type => type.Name == "ToggleInfo")
                .Methods.First(method => method.Name == ".ctor");

            var boxToSimViewMode = initializeTogglesMethod.Body.Instructions.Last(instruction => instruction.OpCode == OpCodes.Box);

            var instructionsToAdd = new List<Instruction>
            {
                Instruction.Create(OpCodes.Ldloc_0),
                Instruction.Create(OpCodes.Ldstr, "Toggle MaterialColor"),
                Instruction.Create(OpCodes.Ldstr, "overlay_materialcolor"), // probably wrong, reuse other sprite
                Instruction.Create(OpCodes.Ldc_I4, Common.IDs.ToggleMaterialColorOverlayID),
                boxToSimViewMode,
                Instruction.Create(OpCodes.Ldc_I4, (int)Action.Overlay12),
                Instruction.Create(OpCodes.Ldstr, "Toggles MaterialColor overlay"),
                Instruction.Create(OpCodes.Ldstr, "MaterialColor"), // new version only
                Instruction.Create(OpCodes.Newobj, toggleInfoConstructor),
                lastAddInstruction
            };

            var inserter = new InstructionInserter(initializeTogglesMethod);

            inserter.InsertAfter(lastAddInstruction, instructionsToAdd);
        }

        // use only one entry point for initialization?
        // TODO: refactor
        private void AddLocalizationString()
        {
            var inputBindings = _csharpModule
                .Types
                .First(type => type.Name == "INPUT_BINDINGS")
                    .NestedTypes
                    .First(type => type.Name == "ROOT");

            var fieldDefinition = new FieldDefinition(
                    "OVERLAY12",
                    FieldAttributes.Static | FieldAttributes.Public,
                    CecilHelper.GetTypeDefinition(_csharpModule, "LocString")
                );

            inputBindings.Fields.Add(fieldDefinition);

            _sourceToCSharpInjector.InjectAsFirstInstruction(
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
         //TODO: make it more flexible for future versions
        private void EnableConsole()
        {
            // error needs to be around here
            _csharpInstructionRemover.RemoveInstructionAt("FrontEndManager", "LateUpdate", 0);
            _csharpInstructionRemover.RemoveInstructionAt("FrontEndManager", "LateUpdate", 0);
            _csharpInstructionRemover.RemoveInstructionAt("FrontEndManager", "LateUpdate", 0);
            _csharpInstructionRemover.RemoveInstructionAt("FrontEndManager", "LateUpdate", 0);

            // see eventsource guid for supporting many versions

            _csharpInstructionRemover.RemoveInstructionAt("Game", "Update", 8);
            _csharpInstructionRemover.RemoveInstructionAt("Game", "Update", 8);
            _csharpInstructionRemover.RemoveInstructionAt("Game", "Update", 8);
            _csharpInstructionRemover.RemoveInstructionAt("Game", "Update", 8);
        }

        private void AttachCustomActionToToggle()
        {
            var OnToggleSelectMethod = CecilHelper.GetMethodDefinition(_csharpModule, "OverlayMenu", "OnToggleSelect");

            var firstInstruction = OnToggleSelectMethod.Body.Instructions.First();

            var instructionsToAdd = new List<Instruction>
            {
                Instruction.Create(OpCodes.Brfalse, firstInstruction),
                Instruction.Create(OpCodes.Ret)
            };

            new InstructionInserter(OnToggleSelectMethod).InsertBefore(firstInstruction, instructionsToAdd);

            _sourceToCSharpInjector.InjectAsFirstInstruction(
                "InjectionEntry",
                "EnterToggle",
                "OverlayMenu",
                "OnToggleSelect",
                true, 1);
        }
    }
}
