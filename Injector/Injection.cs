using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Injector
{
    public class Injection
    {
        public Injection(ModuleDefinition coreModule, ModuleDefinition materialModule, ModuleDefinition onionModule, ModuleDefinition csharpModule, ModuleDefinition firstPassModule)
        {
            Initialize(coreModule, materialModule, onionModule, csharpModule, firstPassModule);
        }

        private ModuleDefinition _coreModule;
        private ModuleDefinition _materialModule;
        private ModuleDefinition _onionModule;
        private ModuleDefinition _csharpModule;
        private ModuleDefinition _firstPassModule;

        private MethodInjector _coreToCSharpInjector;
        private MethodInjector _materialToCSharpInjector;
        private MethodInjector _onionToCSharpInjector;

        private InstructionRemover _csharpInstructionRemover;
        private Publisher _csharpPublisher;


        private void Initialize(ModuleDefinition coreModule, ModuleDefinition materialModule, ModuleDefinition onionModule, ModuleDefinition csharpModule, ModuleDefinition firstPassModule)
        {
            _coreModule = coreModule;
            _materialModule = materialModule;
            _onionModule = onionModule;
            _csharpModule = csharpModule;
            _firstPassModule = firstPassModule;

            InitializeHelpers();
        }

        private void InitializeHelpers()
        {
            _csharpInstructionRemover = new InstructionRemover(_csharpModule);
            _csharpPublisher = new Publisher(_csharpModule);
            _csharpInstructionRemover = new InstructionRemover(_csharpModule);

            InitializeMethodInjectors();
        }

        private void InitializeMethodInjectors()
        {
            _coreToCSharpInjector = new MethodInjector(_coreModule, _csharpModule);
            _materialToCSharpInjector = new MethodInjector(_materialModule, _csharpModule);
            _onionToCSharpInjector = new MethodInjector(_onionModule, _csharpModule);
        }

        public void Inject(bool injectMaterial, bool enableConsole, bool injectOnion)
        {
            if (enableConsole)
            {
                EnableConsole();
            }

            InjectCore();

            if (injectMaterial)
            {
                InjectMain();
                InjectCellColorHandling();
                InjectBuildingsSpecialCasesHandling();
                InjectToggleButton();
            }

            if (injectOnion)
            {
                InjectOnionPatcher();
            }

            InjectPatchedSign();
            FixGameUpdateExceptionHandling();
        }

        private void InjectCore()
        {
            _coreToCSharpInjector.InjectBefore(
               "UpdateQueueManager", "OnGameUpdate",
               "Game", "Update",
               5);
        }

        private void InjectMain()
        {
            _materialToCSharpInjector.InjectAsFirst(
                 "InjectionEntry", "EnterOnce",
                "Game", "OnPrefabInit");

            _materialToCSharpInjector.InjectBefore(
               "InjectionEntry", "EnterEveryUpdate",
               "Game", "Update",
               5);
        }

        private void InjectCellColorHandling()
        {
            _csharpInstructionRemover.ClearAllButLast("BlockTileRenderer", "GetCellColor");

            _materialToCSharpInjector.InjectAsFirst(
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

            _materialToCSharpInjector.InjectAsFirst(
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
            _csharpInstructionRemover.ReplaceByNopAt("FrontEndManager", "LateUpdate", 0);
            _csharpInstructionRemover.ReplaceByNopAt("FrontEndManager", "LateUpdate", 1);
            _csharpInstructionRemover.ReplaceByNopAt("FrontEndManager", "LateUpdate", 2);
            _csharpInstructionRemover.ReplaceByNopAt("FrontEndManager", "LateUpdate", 3);

            _csharpInstructionRemover.ReplaceByNopAt("Game", "Update", 8);
            _csharpInstructionRemover.ReplaceByNopAt("Game", "Update", 9);
            _csharpInstructionRemover.ReplaceByNopAt("Game", "Update", 10);
            _csharpInstructionRemover.ReplaceByNopAt("Game", "Update", 11);
        }

        private void FixGameUpdateExceptionHandling()
        {
            var handler = new ExceptionHandler(ExceptionHandlerType.Finally);
            var methodBody = CecilHelper.GetMethodDefinition(_csharpModule, "Game", "Update").Body;
            var methodInstructions = methodBody.Instructions;

            handler.TryStart = methodInstructions.First(instruction => instruction.OpCode == OpCodes.Ldsfld);

            var tryEndInstruction = methodInstructions.Last(instruction => instruction.OpCode == OpCodes.Ldloca_S);

            handler.TryEnd = tryEndInstruction;
            handler.HandlerStart = tryEndInstruction;
            handler.HandlerEnd = methodInstructions.Last();
            handler.CatchType = _csharpModule.Import(typeof(Exception));

            methodBody.ExceptionHandlers.Clear();
            methodBody.ExceptionHandlers.Add(handler);
        }

        private void AttachCustomActionToToggle()
        {
            var onToggleSelectMethod = CecilHelper.GetMethodDefinition(_csharpModule, "OverlayMenu", "OnToggleSelect");
            var onToggleSelectMethodBody = onToggleSelectMethod.Body;

            var firstInstruction = onToggleSelectMethodBody.Instructions.First();

            var instructionsToAdd = new List<Instruction>
            {
                Instruction.Create(OpCodes.Brfalse, firstInstruction),
                Instruction.Create(OpCodes.Ret)
            };

            new InstructionInserter(onToggleSelectMethod).InsertBefore(firstInstruction, instructionsToAdd);

            _materialToCSharpInjector.InjectAsFirst(
                "InjectionEntry",
                "EnterToggle",
                onToggleSelectMethodBody,
                true, 1);
        }

        private void InjectOnionPatcher()
        {
            InjectOnionDoWorldGen();
            InjectOnionCameraController();
            InjectOnionDebugHandler();
            InjectOnionInitRandom();
        }

        private void InjectOnionInitRandom()
        {
            _onionToCSharpInjector.InjectAsFirst(
                "Hooks", "OnInitRandom",
                "WorldGen", "InitRandom",
                false, 4, true
                );
        }

        private void InjectOnionDoWorldGen()
        {
            var doWorldGenBody = CecilHelper.GetMethodDefinition(_csharpModule, "OfflineWorldGen", "DoWorldGen").Body;

            // TODO: create variable add helper
            doWorldGenBody.Variables.Add(new VariableDefinition("w", _csharpModule.TypeSystem.Int32));
            doWorldGenBody.Variables.Add(new VariableDefinition("h", _csharpModule.TypeSystem.Int32));

            // is it needed?
            //DoWorldGen.NoOptimization = true;

            var callResetInstruction = doWorldGenBody
                .Instructions
                .Where(instruction => instruction.OpCode == OpCodes.Call)
                .Reverse()
                .Skip(1)
                .First();

            var instructionInserter = new InstructionInserter(doWorldGenBody);

            instructionInserter.InsertBefore(callResetInstruction, Instruction.Create(OpCodes.Pop));
            instructionInserter.InsertBefore(callResetInstruction, Instruction.Create(OpCodes.Pop));

            _onionToCSharpInjector.InjectBefore(
                "Hooks", "OnDoOfflineWorldGen",
                doWorldGenBody,
                callResetInstruction);

            _csharpInstructionRemover.ReplaceByNop(doWorldGenBody, callResetInstruction);
        }

        private void InjectOnionDebugHandler()
        {
            _csharpPublisher.MakeFieldPublic("DebugHandler", "enabled");

            var debugHandlerConstructorBody = CecilHelper.GetMethodDefinition(_csharpModule, "DebugHandler", ".ctor").Body;

            var lastInstruction = debugHandlerConstructorBody.Instructions.Last();

            _onionToCSharpInjector.InjectBefore("Hooks", "OnDebugHandlerCtor", debugHandlerConstructorBody, lastInstruction, true);

        }

        private void InjectOnionCameraController()
        {
            var typeName = "CameraController";

            _csharpPublisher.MakeFieldPublic(typeName, "maxOrthographicSize");
            _csharpPublisher.MakeFieldPublic(typeName, "maxOrthographicSizeDebug");

            var cameraControllerOnSpawnBody = CecilHelper.GetMethodDefinition(_csharpModule, typeName, "OnSpawn").Body;
            var restoreCall = cameraControllerOnSpawnBody.Instructions.Last(instruction => instruction.OpCode == OpCodes.Call);

            _onionToCSharpInjector.InjectBefore(
                "Hooks",
                "OnCameraControllerCtor",
                cameraControllerOnSpawnBody,
                restoreCall,
                true);
        }

        private void InjectPatchedSign()
        {
            _csharpModule.Types.Add(new TypeDefinition("Mods", "Patched", TypeAttributes.Class, _csharpModule.TypeSystem.Object));
            _firstPassModule.Types.Add(new TypeDefinition("Mods", "Patched", TypeAttributes.Class, _firstPassModule.TypeSystem.Object));
        }
    }
}
