// <copyright file="MonoCecilExtensions.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Client.Hacking
{
    using System;

    using Mono.Cecil;
    using Mono.Cecil.Cil;

    /// <summary>
    /// Extensions for MonoCecil
    /// </summary>
    public static class MonoCecilExtensions
    {
        /// <summary>
        /// Get a MethodDefinition with type name and method name.
        /// </summary>
        /// <param name="assemblyDefinition">The AssemblyDefinition</param>
        /// <param name="typeName">The type name.</param>
        /// <param name="methodName">The method name.</param>
        /// <returns>The MethodDefinition.</returns>
        public static MethodDefinition GetMethod(this AssemblyDefinition assemblyDefinition, string typeName, string methodName)
        {
            // find hook method
            try
            {
                TypeDefinition td = null;
                foreach (TypeDefinition t in assemblyDefinition.MainModule.Types)
                {
                    if (t.Name == typeName)
                    {
                        td = t;
                        break;
                    }
                }

                if (td == null)
                {
                    return null;
                }

                MethodDefinition md = null;
                foreach (MethodDefinition t in td.Methods)
                {
                    if (t.Name == methodName)
                    {
                        md = t;
                        break;
                    }
                }

                return md;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }

        /// <summary>
        /// Inject a method to an existing method.
        /// </summary>
        /// <param name="assemblyDefinition">The AssemblyDefinition</param>
        /// <param name="newMethodDefinition">The MethodDefinition.</param>
        /// <param name="baseMethodDefinition">The MethodDefinition To Inject</param>
        /// <returns>The updated AssemblyDefinition.</returns>
        public static AssemblyDefinition InjectMethod(this AssemblyDefinition assemblyDefinition, MethodDefinition newMethodDefinition, MethodDefinition baseMethodDefinition)
        {
            try
            {
                ILProcessor ilp = newMethodDefinition.Body.GetILProcessor();
                Instruction ins_first = ilp.Body.Instructions[0];
                Instruction ins = ilp.Create(OpCodes.Call, assemblyDefinition.MainModule.Import(baseMethodDefinition.Resolve()));
                ilp.InsertBefore(ins_first, ins);
                return assemblyDefinition;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }
    }
}
