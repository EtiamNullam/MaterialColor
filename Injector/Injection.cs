using Common.Data;
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

        public Common.IO.Logger Logger { get; set; }

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

        public bool Failed { get; set; } = false;

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

        // TODO: refactor
        public void Inject(InjectorState injectorState)
        {
            InjectCore();

            if (injectorState.FixLogicBridges)
            {
                MakeLogicBridgesPlaceableOnLogicGates();
            }

            if (injectorState.EnableImprovedOxygenOverlay)
            {
                try
                {
                    _csharpInstructionRemover.ClearAllButLast("SimDebugView", "GetOxygenMapColour");
                    _materialToCSharpInjector.InjectAsFirst("InjectionEntry", "EnterGasOverlay", "SimDebugView", "GetOxygenMapColour", includeArgumentCount: 1);
                }
                catch (Exception e)
                {
                    Logger.Log("Improved gas overlay injection failed");
                    Logger.Log(e);

                    Failed = true;
                }
            }

            if (injectorState.EnableDraggableGUI)
            {
                try
                {
                    var coreToFirstpass = new MethodInjector(_coreModule, _firstPassModule);
                    coreToFirstpass.InjectAsFirst("DraggablePanel", "Attach", "KScreen", "OnPrefabInit", includeCallingObject: true);
                }
                catch (Exception e)
                {
                    Logger.Log("Draggable GUI injection failed");
                    Logger.Log(e);

                    Failed = true;
                }
            }

            if (injectorState.InjectMaterialColor)
            {
                InjectMain();
                InjectCellColorHandling();
                InjectBuildingsSpecialCasesHandling();

                try
                {
                    _materialToCSharpInjector.InjectAsFirst("InjectionEntry", "OverlayChangedEntry", "OverlayMenu", "OnOverlayChanged");
                }
                catch (Exception e)
                {
                    Logger.Log("OverlayChangedEntry injection failed");
                    Logger.Log(e);

                    Failed = true;
                }

                if (injectorState.InjectMaterialColorOverlayButton)
                {
                    try
                    {
                        InjectToggleButton();
                    }
                    catch (Exception e)
                    {
                        if (Logger != null)
                        {
                            Logger.Log("Overlay menu button injection failed");
                            Logger.Log(e);

                            Failed = true;
                        }
                    }
                }
            }

            if (injectorState.InjectOnion)
            {
                InjectOnionPatcher();
            }

            if (injectorState.CustomSensorRanges)
            {
                ExpandTemperatureSensorRange(injectorState.MaxSensorTemperature);
                ExpandGasSensorPressureRange(injectorState.MaxGasSensorPressure);
                ExpandLiquidSensorPressureRange(injectorState.MaxLiquidSensorPressure);
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
            _csharpInstructionRemover.ClearAllButLast("BlockTileRenderer", "GetCellColour");

            _materialToCSharpInjector.InjectAsFirst(
                "InjectionEntry", "EnterCell",
                "BlockTileRenderer", "GetCellColour",
                true, 1);

            _csharpPublisher.MakeFieldPublic("BlockTileRenderer", "selectedCell");
            _csharpPublisher.MakeFieldPublic("BlockTileRenderer", "highlightCell");
            _csharpPublisher.MakeFieldPublic("BlockTileRenderer", "invalidPlaceCell");

            var deconstructable = _csharpModule.Types.FirstOrDefault(t => t.Name == "Deconstructable");

            if (deconstructable != null)
            {
                var onCompleteWorkBody = deconstructable.Methods.FirstOrDefault(m => m.Name == "OnCompleteWork").Body;

                if (onCompleteWorkBody != null)
                {
                    var lastInstruction = onCompleteWorkBody.Instructions.LastOrDefault();

                    if (lastInstruction != null)
                    {
                        var inserter = new InstructionInserter(onCompleteWorkBody);

                        inserter.InsertBefore(lastInstruction, Instruction.Create(OpCodes.Ldloc_3));
                        _materialToCSharpInjector.InjectBefore("InjectionEntry", "ResetCell", onCompleteWorkBody, lastInstruction);
                    }
                    else
                    {
                        Logger.Log("Couldn't find last instruction at Deconstructable.OnCompleteWork method");

                        Failed = true;
                    }
                }
                else
                {
                    Logger.Log("Couldn't find method at Deconstructable.OnCompleteWork");

                    Failed = true;
                }
            }
            else
            {
                Logger.Log("Couldn't find type: Deconstructable");

                Failed = true;
            }
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
            ExtendMaxActionCount();
            InjectKeybindings();

            AddOverlayButton();
            AttachCustomActionToToggle();

            AddOverlayButtonSprite();
        }

        // TODO: notify user when injection fails
        private void ExtendMaxActionCount()
        {
            const int MaxActionCount = 1000;

            var kInputController = _firstPassModule.Types.FirstOrDefault(type => type.Name == "KInputController");

            if (kInputController != null)
            {
                var keyDef = kInputController.NestedTypes.FirstOrDefault(nestedType => nestedType.Name == "KeyDef");

                if (keyDef != null)
                {
                    var keyDefConstructorBody = keyDef.Methods.First(method => method.Name == ".ctor").Body;

                    keyDefConstructorBody.Instructions.Last(instruction => instruction.OpCode == OpCodes.Ldc_I4)
                        .Operand = MaxActionCount;

                    var kInputControllerConstructorBody = CecilHelper.GetMethodDefinition(_firstPassModule, kInputController, ".ctor").Body;

                    kInputControllerConstructorBody.Instructions.First(instruction => instruction.OpCode == OpCodes.Ldc_I4)
                        .Operand = MaxActionCount;
                }
                else
                {
                    Logger.Log("Can't find type KInputController.KeyDef");

                    Failed = true;
                }
            }
            else
            {
                Logger.Log("Can't find type KInputController");

                Failed = true;
            }
        }

        private void InjectKeybindings()
        {
            AddDefaultKeybinding(_csharpModule, _firstPassModule, KKeyCode.F6, Modifier.Alt, (Action)Common.IDs.ToggleMaterialColorOverlayAction);
        }

        private void AddDefaultKeybinding(ModuleDefinition CSharpModule, ModuleDefinition FirstPassModule, KKeyCode keyCode, Modifier keyModifier, Action action, string screen = "Root")
        {
            var beforeFieldInit = CecilHelper.GetMethodDefinition(FirstPassModule, CecilHelper.GetTypeDefinition(FirstPassModule, "GameInputMapping"), ".cctor");

            var lastKeybindingDeclarationEnd = beforeFieldInit.Body.Instructions.Last(instruction => instruction.OpCode == OpCodes.Stobj);
            var stoBindingEntryInstruction = beforeFieldInit.Body.Instructions.First(instruction => instruction.OpCode == OpCodes.Stobj);
            var newBindingEntryInstruction = beforeFieldInit.Body.Instructions.First(instruction => instruction.OpCode == OpCodes.Newobj);

            var lastDupInstruction = beforeFieldInit.Body.Instructions.LastOrDefault(instr => instr.OpCode == OpCodes.Dup);

            if (lastDupInstruction != null)
            {
                var lastEntryIndex = Convert.ToInt32(lastDupInstruction.Next.Operand);

                var instructionsToAdd = new List<Instruction>
                {
                    Instruction.Create(OpCodes.Dup),
                    Instruction.Create(OpCodes.Ldc_I4, lastEntryIndex + 1), // index 
                    Instruction.Create(OpCodes.Ldelema, (TypeReference)stoBindingEntryInstruction.Operand),
                    Instruction.Create(OpCodes.Ldstr, screen),
                    Instruction.Create(OpCodes.Ldc_I4_S, (sbyte)16), // gamepad button
                    Instruction.Create(OpCodes.Ldc_I4, (int)keyCode),
                    Instruction.Create(OpCodes.Ldc_I4, (int)keyModifier),
                    Instruction.Create(OpCodes.Ldc_I4, (int)action),
                    Instruction.Create(OpCodes.Ldc_I4_1), // rebindable = true
                    Instruction.Create(OpCodes.Ldc_I4_1), // ignore root conflicts = true
                    newBindingEntryInstruction, // create new object
                    stoBindingEntryInstruction // store in array
                };

                var ILProcessor = beforeFieldInit.Body.GetILProcessor();

                // increase array size by one
                var arraySizeSetInstruction = beforeFieldInit.Body.Instructions.First();
                ILProcessor.Replace(arraySizeSetInstruction, Instruction.Create(OpCodes.Ldc_I4, (int)arraySizeSetInstruction.Operand + 1));
                //

                new InstructionInserter(ILProcessor).InsertAfter(lastKeybindingDeclarationEnd, instructionsToAdd);
            }
            else
            {
                Logger.Log("Can't find last duplication instruction at GameInputMapping.cctor");

                Failed = true;
            }

        }

        // TODO: use other sprite, refactor
        private void AddOverlayButton()
        {
            _csharpPublisher.MakeFieldPublic("OverlayMenu", "overlay_toggle_infos");

            var overlayMenu = _csharpModule.Types.First(type => type.Name == "OverlayMenu");

            overlayMenu.NestedTypes.First(nestedType => nestedType.Name == "OverlayToggleInfo").IsPublic = true;

            var onPrefabInitBody = CecilHelper.GetMethodDefinition(_csharpModule, overlayMenu, "OnPrefabInit").Body;

            var loadOverlayToggleInfosInstuction = onPrefabInitBody.Instructions.First(instruction => instruction.OpCode == OpCodes.Ldfld);

            _coreToCSharpInjector.InjectBefore(
                "OverlayMenuManager", "OnOverlayMenuPrefabInit",
                onPrefabInitBody, loadOverlayToggleInfosInstuction.Next, true);
        }

        private void AddOverlayButtonSprite()
        {
            try
            {
                var body = _csharpModule.Types.FirstOrDefault(t => t.Name == "KIconToggleMenu").Methods.FirstOrDefault(m => m.Name == "RefreshButtons").Body;
                var targetInstruction = body.Instructions.Reverse().Skip(5).FirstOrDefault(i => i.OpCode == OpCodes.Callvirt);

                var inserter = new InstructionInserter(body);

                inserter.InsertBefore(targetInstruction, Instruction.Create(OpCodes.Ldloc, body.Variables[5]));

                _coreToCSharpInjector.InjectBefore("OverlayMenuManager", "EnterToggleSprite", body, targetInstruction, includeCallingObject: true);

            }
            catch (Exception e)
            {
                Logger.Log("Toggle change sprite failed");
                Logger.Log(e);

                Failed = true;
            }
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
        // Doesn't work on Tubular Upgrade
        private void EnableConsole()
        {
            try
            {
                _csharpModule
                    .Types.FirstOrDefault(t => t.Name == "Game")
                        .Methods.FirstOrDefault(m => m.Name == "Update")
                            .Body.Instructions.FirstOrDefault(i => i.OpCode == OpCodes.Ldc_I4_0)
                               .OpCode = OpCodes.Ldc_I4_1;

                _csharpModule
                    .Types.FirstOrDefault(t => t.Name == "FrontEndManager")
                        .Methods.FirstOrDefault(m => m.Name == "LateUpdate")
                            .Body.Instructions.FirstOrDefault(i => i.OpCode == OpCodes.Ldc_I4_0)
                               .OpCode = OpCodes.Ldc_I4_1;

                _csharpModule
                    .Types.FirstOrDefault(t => t.Name == "GraphicsOptionsScreen")
                        .Methods.FirstOrDefault(m => m.Name == "Update")
                            .Body.Instructions.FirstOrDefault(i => i.OpCode == OpCodes.Ldc_I4_0)
                               .OpCode = OpCodes.Ldc_I4_1;

                _csharpModule
                    .Types.FirstOrDefault(t => t.Name == "OptionsMenuScreen")
                        .Methods.FirstOrDefault(m => m.Name == "Update")
                            .Body.Instructions.FirstOrDefault(i => i.OpCode == OpCodes.Ldc_I4_0)
                               .OpCode = OpCodes.Ldc_I4_1;

                _csharpModule
                    .Types.FirstOrDefault(t => t.Name == "ReportErrorDialog")
                        .Methods.FirstOrDefault(m => m.Name == "Update")
                            .Body.Instructions.FirstOrDefault(i => i.OpCode == OpCodes.Ldc_I4_0)
                               .OpCode = OpCodes.Ldc_I4_1;
            }
            catch (Exception e)
            {
                Logger.Log("Enable console failed");
                Logger.Log(e);

                Failed = true;
            }
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
            var doWorldGenInitialiseBody = CecilHelper.GetMethodDefinition(_csharpModule, "OfflineWorldGen", "DoWordGenInitialise").Body;

            var callResetInstruction = doWorldGenInitialiseBody
                .Instructions
                .Where(instruction => instruction.OpCode == OpCodes.Call)
                .Reverse()
                .Skip(3)
                .First();

            var instructionInserter = new InstructionInserter(doWorldGenInitialiseBody);

            instructionInserter.InsertBefore(callResetInstruction, Instruction.Create(OpCodes.Pop));
            instructionInserter.InsertBefore(callResetInstruction, Instruction.Create(OpCodes.Pop));

            _onionToCSharpInjector.InjectBefore(
                "Hooks", "OnDoOfflineWorldGen",
                doWorldGenInitialiseBody,
                callResetInstruction);

            _csharpInstructionRemover.ReplaceByNop(doWorldGenInitialiseBody, callResetInstruction);
        }

        private void InjectOnionDebugHandler()
        {
            var debugHandler = _csharpModule
                .Types
                .FirstOrDefault(type => type.Name == "DebugHandler");

            if (debugHandler != null)
            {
                var debugHandlerEnabledProperty = debugHandler
                    .Properties
                    .FirstOrDefault(property => property.Name == "enabled");

                if (debugHandlerEnabledProperty != null)
                {
                    debugHandlerEnabledProperty
                        .SetMethod
                        .IsPublic = true;
                }
            }
            else
            {
                Logger.Log("Can't find type DebugHandler");

                Failed = true;
            }

            var debugHandlerConstructorBody = CecilHelper.GetMethodDefinition(_csharpModule, debugHandler, ".ctor").Body;

            var lastInstruction = debugHandlerConstructorBody.Instructions.Last();

            _onionToCSharpInjector.InjectBefore("Hooks", "OnDebugHandlerCtor", debugHandlerConstructorBody, lastInstruction);
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

        private void ExpandTemperatureSensorRange(float newMax)
        {
            try
            {
                _csharpModule.Types.FirstOrDefault(t => t.Name == "LogicTemperatureSensorConfig")
                    .Methods.FirstOrDefault(m => m.Name == "DoPostConfigureComplete").Body
                    .Instructions.LastOrDefault(i => i.OpCode == OpCodes.Ldc_R4).Operand = newMax;
            }
            catch (Exception e)
            {
                Logger.Log("Expand temperature sensor range failed");
                Logger.Log(e);

                Failed = true;
            }
        }

        private void MakeLogicBridgesPlaceableOnLogicGates()
        {
            try
            {
                var body = _csharpModule.Types.FirstOrDefault(t => t.Name == "LogicWireBridgeConfig").Methods.FirstOrDefault(m => m.Name == "CreateBuildingDef").Body;
                var lastInstruction = body.Instructions.LastOrDefault();

                _coreToCSharpInjector.InjectBefore("InjectionEntry", "LogicWireBridgeEnter", body, lastInstruction);

                new InstructionInserter(body).InsertBefore(lastInstruction, Instruction.Create(OpCodes.Ldloc_0));
            }
            catch (Exception e)
            {
                Logger.Log("Logic bridges placable everywhere injection failed");
                Logger.Log(e);
            }
        }

        private void ExpandGasSensorPressureRange(float newMax)
        {
            try
            {
                _csharpModule.Types.FirstOrDefault(t => t.Name == "LogicPressureSensorGasConfig")
                    .Methods.FirstOrDefault(m => m.Name == "DoPostConfigureComplete").Body
                    .Instructions.LastOrDefault(i => i.OpCode == OpCodes.Ldc_R4 && (float)i.Operand == 2).Operand = newMax;
            }
            catch (Exception e)
            {
                Logger.Log("Expand gas sensor range failed");
                Logger.Log(e);

                Failed = true;
            }
        }

        private void ExpandLiquidSensorPressureRange(float newMax)
        {
            try
            {
                _csharpModule.Types.FirstOrDefault(t => t.Name == "LogicPressureSensorLiquidConfig")
                    .Methods.FirstOrDefault(m => m.Name == "DoPostConfigureComplete").Body
                    .Instructions.LastOrDefault(i => i.OpCode == OpCodes.Ldc_R4 && (float)i.Operand == 2000).Operand = newMax;
            }
            catch (Exception e)
            {
                Logger.Log("Expand liquid sensor range failed");
                Logger.Log(e);

                Failed = true;
            }
        }

        private void InjectPatchedSign()
        {
            _csharpModule.Types.Add(new TypeDefinition("Mods", "Patched", TypeAttributes.Class, _csharpModule.TypeSystem.Object));
            _firstPassModule.Types.Add(new TypeDefinition("Mods", "Patched", TypeAttributes.Class, _firstPassModule.TypeSystem.Object));
        }
    }
}
