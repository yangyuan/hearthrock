using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Hearthrock
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
                File.Copy("Hearthrock.exe", path_hearthstone_managed + "Hearthrock.exe", true);
                string path_hearthrock = path_hearthstone_managed + "Hearthrock.exe";
                AssemblyDefinition assembly_hearthrock = AssemblyDefinition.ReadAssembly(path_hearthrock);
                string path_hearthstone = path_hearthstone_managed + "Assembly-CSharp.dll";

                MethodDefinition method_hearthrock = fetch_method(assembly_hearthrock, "HearthrockUnity", "Hook");
                if (method_hearthrock == null)
                {
                    Console.WriteLine("Hearthrock Hook Not Found!");
                    Console.ReadKey(false);
                    return;
                }

                AssemblyDefinition ad = AssemblyDefinition.ReadAssembly(path_hearthstone);
                MethodDefinition method_hearthstone = fetch_method(ad, "SceneMgr", "Start");
                // MethodDefinition method_hearthstone = fetch_method(path_hearthstone, "CollectionManager", "Get");
                if (method_hearthstone == null)
                {
                    Console.WriteLine("HearthStone Start Not Found!");
                    Console.ReadKey(false);
                    return;
                }
                AssemblyDefinition assembly_csharp = inject_method(ad, method_hearthstone, method_hearthrock);

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


        static AssemblyDefinition inject_method(AssemblyDefinition ad, MethodDefinition method, MethodDefinition method_tobe_inject)
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

        static MethodDefinition fetch_method(AssemblyDefinition ad, string type, string method)
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
    }
}
