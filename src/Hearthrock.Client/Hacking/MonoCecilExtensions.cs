namespace Hearthrock.Client.Hacking
{
    using System;
    using Mono.Cecil;
    using Mono.Cecil.Cil;

    /// <summary>
    /// 
    /// </summary>
    static class MonoCecilExtensions
    {
        public static MethodDefinition GetMethod(this AssemblyDefinition ad, string type, string method)
        {
            // find hook method
            try
            {
                TypeDefinition td = null;
                foreach (TypeDefinition t in ad.MainModule.Types)
                {
                    if (t.Name == type)
                    {
                        td = t;
                        break;
                    }
                }
                if (td == null) return null;
                MethodDefinition md = null;
                foreach (MethodDefinition t in td.Methods)
                {
                    if (t.Name == method)
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

        public static AssemblyDefinition InjectMethod(this AssemblyDefinition ad, MethodDefinition method, MethodDefinition method_tobe_inject)
        {
            try
            {
                ILProcessor ilp = method.Body.GetILProcessor();
                Instruction ins_first = ilp.Body.Instructions[0];
                Instruction ins = ilp.Create(OpCodes.Call, ad.MainModule.Import(method_tobe_inject.Resolve()));
                ilp.InsertBefore(ins_first, ins);
                return ad;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }
    }
}
