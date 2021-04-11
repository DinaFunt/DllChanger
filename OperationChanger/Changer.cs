using System;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace OperationChanger
{
    public static class Changer
    {
        public static void ChangeAddToSubstr(string filename, string target)
        {
            if (filename.Length == 0 || target.Length == 0)
            {
                throw new ArgumentException("Filenames can't be empty");
            }
            var assemblyDefinition = AssemblyDefinition.ReadAssembly(filename);

            var moduleDefinitions = assemblyDefinition.Modules;

            foreach (var moduleDefinition in moduleDefinitions)
            {
                foreach (var typeDefinition in moduleDefinition.Types)
                {
                    foreach (var methodDefinition in typeDefinition.Methods)
                    {

                        var processor = methodDefinition.Body?.GetILProcessor();
                        var instructions = methodDefinition.Body?.Instructions;

                        if (instructions != null)
                        {
                            for (int i = 0; i < instructions.Count; i++)
                            {
                                if (instructions[i].OpCode.Equals(OpCodes.Add))
                                {
                                    instructions[i] = Instruction.Create(OpCodes.Sub);
                                }
                                
                                else if (instructions[i].OpCode.Equals(OpCodes.Call) &&
                                         instructions[i].Operand.ToString().Contains("op_Addition"))
                                {
                                    var ass = typeof(decimal).Assembly;

                                    var asd = AssemblyDefinition.ReadAssembly(ass.Location);

                                    MethodInfo subtractMethod = typeof(decimal).GetMethod("Subtract");
                                    MethodReference subtractReference = assemblyDefinition.MainModule.ImportReference(subtractMethod);
                     
                                    var newInstruction = processor.Create(OpCodes.Call, subtractReference);
                                    newInstruction.Next = instructions[i].Next;
                                    newInstruction.Previous = instructions[i].Previous;
                                    newInstruction.Offset = instructions[i].Offset;

                                    instructions[i] = newInstruction;
                                }
                            }
                        }
                    }
                }
            }

            assemblyDefinition.Write(target);
        }
    }
}