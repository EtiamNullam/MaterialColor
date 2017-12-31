using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Injector
{
    public class InstructionInserter
    {
        public InstructionInserter(MethodDefinition targetMethod) : this(targetMethod.Body.GetILProcessor()) { }

        public InstructionInserter(MethodBody targetMethodBody) : this(targetMethodBody.GetILProcessor()) { }

        public InstructionInserter(ILProcessor targetMethodILProcessor)
        {
            _ilProcessor = targetMethodILProcessor;
        }

        private ILProcessor _ilProcessor;

        public void InsertBefore(Instruction targetInstruction, IEnumerable<Instruction> instructionsToInsert)
        {
            foreach (var newInstruction in instructionsToInsert)
            {
                _ilProcessor.InsertBefore(targetInstruction, newInstruction);
            }
        }

        public void InsertBefore(Instruction targetInstruction, Instruction instructionToInsert)
        {
            _ilProcessor.InsertBefore(targetInstruction, instructionToInsert);
        }

        public void InsertAfter(Instruction targetInstruction, IEnumerable<Instruction> instructionsToInsert)
        {
            var reversedInstructions = instructionsToInsert.Reverse();

            foreach (var newInstruction in reversedInstructions)
            {
                _ilProcessor.InsertAfter(targetInstruction, newInstruction);
            }
        }

        public void InsertAfter(int instructionIndex, Instruction instructionToInsert)
        {
            InsertAfter(_ilProcessor.Body.Instructions[instructionIndex], instructionToInsert);
        }

        public void InsertAfter(Instruction targetInstruction, Instruction instructionToInsert)
        {
            _ilProcessor.InsertAfter(targetInstruction, instructionToInsert);
        }
    }
}
