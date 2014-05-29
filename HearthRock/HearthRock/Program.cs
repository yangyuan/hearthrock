using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HearthRock
{
    class Program
    {

        static void Main(string[] args)
        {
            try
            {
                // copy rock file
                string path_hearthstone_managed = "";
                if (File.Exists(@"Hearthstone\Hearthstone_Data\Managed\Assembly-CSharp.dll"))
                {
                    path_hearthstone_managed = @"Hearthstone\Hearthstone_Data\Managed\";
                }
                else if (File.Exists(@"C:\Games\Hearthstone\Hearthstone_Data\Managed\Assembly-CSharp.dll"))
                {
                    path_hearthstone_managed = @"C:\Games\Hearthstone\Hearthstone_Data\Managed\";
                }
                else
                {
                    Console.WriteLine("HearthStone Not Found!");
                    Console.ReadKey(false);
                    return;
                }
                File.Copy("HearthRock.exe", path_hearthstone_managed + "HearthRock.exe", true);
                string path_hearthrock = path_hearthstone_managed + "HearthRock.exe";
                string path_hearthstone = path_hearthstone_managed + "Assembly-CSharp.dll";

                MethodDefinition method_hearthrock = fetch_method(path_hearthrock, "HearthRock", "Hook");
                if (method_hearthrock == null)
                {
                    Console.WriteLine("HearthRock Hook Not Found!");
                    Console.ReadKey(false);
                    return;
                }
                MethodDefinition method_hearthstone = fetch_method(path_hearthstone, "SceneMgr", "Start");
                if (method_hearthstone == null)
                {
                    Console.WriteLine("HearthStone Start Not Found!");
                    Console.ReadKey(false);
                    return;
                }
                AssemblyDefinition assembly_csharp = inject_method(path_hearthstone, method_hearthstone, method_hearthrock);
                
                assembly_csharp.Write(path_hearthstone);

                Console.WriteLine("Patch Success!");
                Console.ReadKey(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: When generating Injection file");
                Console.WriteLine(ex.ToString());
                Console.ReadKey(false);
            }
        }

        static AssemblyDefinition inject_method(string file, MethodDefinition method, MethodDefinition method_tobe_inject)
        {
            try
            {
                AssemblyDefinition ad = AssemblyDefinition.ReadAssembly(file);
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
        static MethodDefinition fetch_method(string file, string type, string method)
        {
            // find hook method
            try
            {
                AssemblyDefinition ad = AssemblyDefinition.ReadAssembly(file);
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
    }
}
