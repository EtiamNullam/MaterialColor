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
        public Injection(ModuleDefinition materialModule, ModuleDefinition onionModule, ModuleDefinition csharpModule, ModuleDefinition firstPassModule)
        {
            Initialize(materialModule, onionModule, csharpModule, firstPassModule);
        }

        private ModuleDefinition _materialModule;
        private ModuleDefinition _onionModule;
        private ModuleDefinition _csharpModule;
        private ModuleDefinition _firstPassModule;

        private MethodInjector _materialToCSharpInjector;
        private InstructionRemover _csharpInstructionRemover;
        private Publisher _csharpPublisher;

        private MethodInjector _onionToCSharpInjector;

        private void Initialize(ModuleDefinition materialModule, ModuleDefinition onionModule, ModuleDefinition csharpModule, ModuleDefinition firstPassModule)
        {
            _materialModule = materialModule;
            _onionModule = onionModule;
            _csharpModule = csharpModule;
            _firstPassModule = firstPassModule;

            _csharpInstructionRemover = new InstructionRemover(csharpModule);
            _csharpPublisher = new Publisher(csharpModule);
            _csharpInstructionRemover = new InstructionRemover(csharpModule);
            _materialToCSharpInjector = new MethodInjector(materialModule, csharpModule);

            _onionToCSharpInjector = new MethodInjector(_onionModule, _csharpModule);
        }

        public void Inject(bool enableConsole)
        {
            if (enableConsole)
            {
                EnableConsole();
            }

            InjectMain();
            InjectCellColorHandling();
            InjectBuildingsSpecialCasesHandling();
            InjectToggleButton();

            InjectRawOnionPatcher();
        }

        private void InjectMain()
        {
            _materialToCSharpInjector.InjectAsFirstInstruction(
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

            _materialToCSharpInjector.InjectAsFirstInstruction(
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

            _materialToCSharpInjector.InjectAsFirstInstruction(
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
            _csharpInstructionRemover.RemoveInstructionAt("FrontEndManager", "LateUpdate", 0);
            _csharpInstructionRemover.RemoveInstructionAt("FrontEndManager", "LateUpdate", 0);
            _csharpInstructionRemover.RemoveInstructionAt("FrontEndManager", "LateUpdate", 0);
            _csharpInstructionRemover.RemoveInstructionAt("FrontEndManager", "LateUpdate", 0);

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

            _materialToCSharpInjector.InjectAsFirstInstruction(
                "InjectionEntry",
                "EnterToggle",
                "OverlayMenu",
                "OnToggleSelect",
                true, 1);
        }

        /// <summary>
        /// WIP, not used yet 
        /// </summary>
        private void InjectOnionPatcher()
        {
            _onionToCSharpInjector.InjectAsFirstInstruction(
                "Hooks",
                "OnInitRandom",
                "WorldGen",
                "InitRandom",
                false,
                4,
                true
                );
        }

        /// <summary> Seems its working
        /// <para>TODO: major refactor needed </para>
        /// </summary>
        private void InjectRawOnionPatcher()
        {
            /* Find all injection-points for Assembly-CSharp.dll */
            MethodDefinition InitRandom = _csharpModule
                .Types.First(T => T.Name == "WorldGen")
                .Methods.First(F => F.Name == "InitRandom");


            MethodDefinition OnInitRandom = _onionModule
                .Types.First(T => T.Name == "Hooks")
                .Methods.First(M => M.Name == "OnInitRandom");

            MethodDefinition DoWorldGen = _csharpModule
                .Types.First(T => T.Name == "OfflineWorldGen")
                .Methods.First(F => F.Name == "DoWorldGen");

            MethodDefinition DebugHandlerCTOR = _csharpModule
                .Types.First(T => T.Name == "DebugHandler")
                .Methods.First(F => F.Name == ".ctor");

            Console.WriteLine(DebugHandlerCTOR.Name);


            if (InitRandom == null && OnInitRandom == null)
            {
                Console.WriteLine("Missing MethodDefinitions");
                return;
            }

            DoWorldGen.Body.Variables.Add(new VariableDefinition("w", _csharpModule.TypeSystem.Int32));
            DoWorldGen.Body.Variables.Add(new VariableDefinition("h", _csharpModule.TypeSystem.Int32));

            /* Write IL */
            ILProcessor proc = InitRandom.Body.GetILProcessor();
            Instruction IP = InitRandom.Body.Instructions[0];
            try
            {

                byte slot_0 = 0;
                byte slot_1 = 1;
                byte slot_2 = 2;
                byte slot_3 = 3;
                byte slot_4 = 4;

                // Add hook to Assembly-CSharp.Klei.WorldGen.InitRandom
                proc.InsertBefore(IP, IP = proc.Create(OpCodes.Ldarga_S, slot_1)); // Load the address, since we are passing by ref.
                // Update the compatability
                proc.InsertAfter(IP, IP = proc.Create(OpCodes.Ldarga_S, slot_2));
                proc.InsertAfter(IP, IP = proc.Create(OpCodes.Ldarga_S, slot_3));
                proc.InsertAfter(IP, IP = proc.Create(OpCodes.Ldarga_S, slot_4));
                proc.InsertAfter // Add call to our dll
                (
                    IP,
                    IP = proc.Create(OpCodes.Call, InitRandom.Module.Import(
                        CecilHelper.GetMethodReference(_onionModule, CecilHelper.GetMethodDefinition(_onionModule, "Hooks", "OnInitRandom"))
                    ))
                );

                // Add hook to <hook>
                DoWorldGen.NoOptimization = true;
                proc = DoWorldGen.Body.GetILProcessor();
                IP = DoWorldGen.Body.Instructions[26];

                proc.InsertAfter(proc.Body.Instructions[23], proc.Create(OpCodes.Stloc_2));
                Instruction I = proc.Body.Instructions[26];
                proc.InsertAfter(I, I = proc.Create(OpCodes.Stloc_3));
                proc.InsertAfter(I, I = proc.Create(OpCodes.Ldloca_S, slot_2));
                proc.InsertAfter(I, I = proc.Create(OpCodes.Ldloca_S, slot_3));

                proc.InsertBefore(IP, IP = proc.Create(OpCodes.Call, DoWorldGen.Module.Import(
                    CecilHelper.GetMethodReference(_onionModule,
                        CecilHelper.GetMethodDefinition(_onionModule, "Hooks", "OnDoOfflineWorldGen")
                    )
                    ))
                );


                proc.InsertAfter(IP, IP = proc.Create(OpCodes.Ldloc_2));
                proc.InsertAfter(IP, IP = proc.Create(OpCodes.Ldloc_3));

                FieldReference enabled = _csharpModule
                .Types.First(T => T.Name == "DebugHandler").Fields.First(F => F.Name == "enabled");

                FieldReference camera = _csharpModule
                .Types.First(T => T.Name == "DebugHandler").Fields.First(F => F.Name == "FreeCameraMode");

                Console.WriteLine(camera.Name);
                // Add hook to <hook>

                proc = DebugHandlerCTOR.Body.GetILProcessor();
                IP = DebugHandlerCTOR.Body.Instructions.Last();


                proc.InsertBefore(IP, IP = proc.Create(OpCodes.Ldarg_0));
                proc.InsertAfter(IP, IP = proc.Create(OpCodes.Call, DebugHandlerCTOR.Module.Import(
                   CecilHelper.GetMethodDefinition(_onionModule, "Hooks", "GetDebugEnabled")
                )));


                //proc.InsertBefore(IP, IP = proc.Create(OpCodes.Ldc_I4_1));
                proc.InsertAfter(IP, IP = proc.Create(OpCodes.Stfld, enabled));



                proc.InsertAfter(IP, IP = proc.Create(OpCodes.Call, DebugHandlerCTOR.Module.Import(
                   CecilHelper.GetMethodDefinition(_onionModule, "Hooks", "GetDebugEnabled")
                )));


                //proc.InsertAfter(IP, IP = proc.Create(OpCodes.Ldc_I4_1));
                proc.InsertAfter(IP, IP = proc.Create(OpCodes.Stsfld, camera));

                // Hook

                MethodDefinition CameraController = _csharpModule
                       .Types.First(T => T.Name == "CameraController")
                       .Methods.First(F => F.Name == ".ctor");

                FieldReference MaxCameraDistance = _csharpModule
                .Types.First(T => T.Name == "CameraController").Fields.First(F => F.Name == "maxOrthographicSize");

                FieldReference MaxCameraDistance2 = _csharpModule
                        .Types.First(T => T.Name == "CameraController").Fields.First(F => F.Name == "maxOrthographicSizeDebug");

                proc = CameraController.Body.GetILProcessor();
                IP = CameraController.Body.Instructions.Last();

                proc.InsertBefore(IP, IP = proc.Create(OpCodes.Ldarg_0));
                proc.InsertAfter(IP, IP = proc.Create(OpCodes.Call, CameraController.Module.Import(
                   CecilHelper.GetMethodDefinition(_onionModule, "Hooks", "GetMaxCameraShow")
                )));
                proc.InsertAfter(IP, IP = proc.Create(OpCodes.Stfld, MaxCameraDistance));

                proc.InsertAfter(IP, IP = proc.Create(OpCodes.Ldarg_0));
                proc.InsertAfter(IP, IP = proc.Create(OpCodes.Call, CameraController.Module.Import(
                   CecilHelper.GetMethodDefinition(_onionModule, "Hooks", "GetMaxCameraShow")
                )));
                proc.InsertAfter(IP, IP = proc.Create(OpCodes.Stfld, MaxCameraDistance));


                // Add OnionID which is a class that identifies if the Assembly-CSharp.dll has been patched.
                var OnionPatched = new TypeDefinition("", "OnionPatched", TypeAttributes.Class, _csharpModule.TypeSystem.Object);
                _csharpModule.Types.Add(OnionPatched);

                //CSharp.Write(GetAssemblyDir() + Path.DirectorySeparatorChar + "Assembly-CSharp.dll");
                //MessageBox.Show("Patch Complete!");

                // TODO: use
                //ValidateAssembly(GetAssemblyDir());
            }
            // temporal solution
            catch (Exception ex)
            {
                throw ex;
                //MessageBox.Show(ex.ToString());
            }
        }
    }
}
