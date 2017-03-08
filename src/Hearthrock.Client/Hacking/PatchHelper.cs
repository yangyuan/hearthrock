// <copyright file="PatchHelper.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Client.Hacking
{
    using System;
    using System.IO;
    using Mono.Cecil;
    using System.Threading.Tasks;
    using Contracts;
    using Newtonsoft.Json;

    /// <summary>
    /// The class to inject or remove Hearthrock
    /// </summary>
    public class PatchHelper
    {
        /// <summary>
        /// The name of Hearthrock main dll, aka Hearthrock.dll.
        /// </summary>
        const string HearthrockAssemblyName = @"Hearthrock.dll";
        /// <summary>
        /// The name of Hearthrock configuration file, aka hearthrock.json.
        /// </summary>
        const string HearthrockConfigurationName = @"hearthrock.json";

        /// <summary>
        /// The name of Hearthstone main dll, aka Assembly-CSharp.dll.
        /// </summary>
        const string HearthstoneAssemblyName = @"Assembly-CSharp.dll";

        /// <summary>
        /// The name of Hearthstone main dll backup.
        /// </summary>
        const string HearthstoneBackupAssemblyName = @"Assembly-CSharp-original.dll";

        /// <summary>
        /// Relative path to Hearthstone assembly directory, aka the directory contains Assembly-CSharp.dll.
        /// </summary>
        const string RelativePathToHearthstoneAssemblyDirectory = @"Hearthstone\Hearthstone_Data\Managed";

        public async Task InjectAsync(string root)
        {
            await Task.Run(() => Inject(root));
        }

        public void Inject(string root)
        {
            SetupHearthrock(root);

            var hearthstoneAssemblyPath = Path.Combine(root, RelativePathToHearthstoneAssemblyDirectory, HearthstoneAssemblyName);
            var hearthrockAssemblyPath = Path.Combine(root, RelativePathToHearthstoneAssemblyDirectory, HearthrockAssemblyName);
            var resolver = new DefaultAssemblyResolver();
            resolver.AddSearchDirectory(Path.Combine(root, RelativePathToHearthstoneAssemblyDirectory));

            var hearthrockAssembly = AssemblyDefinition.ReadAssembly(hearthrockAssemblyPath, new ReaderParameters { AssemblyResolver = resolver });
            var hearthstoneAssembly = AssemblyDefinition.ReadAssembly(hearthstoneAssemblyPath, new ReaderParameters { AssemblyResolver = resolver });

            MethodDefinition method_hearthrock = hearthrockAssembly.GetMethod("RockUnity", "Hook");
            if (method_hearthrock == null)
            {
                throw new Exception("Hearthrock Hook Not Found!");
            }

            MethodDefinition method_hearthstone = hearthstoneAssembly.GetMethod("SceneMgr", "Start");
            if (method_hearthstone == null)
            {
                throw new Exception("Hearthstone Start Not Found!");
            }
            AssemblyDefinition assembly_csharp = hearthstoneAssembly.InjectMethod(method_hearthstone, method_hearthrock);

            assembly_csharp.Write(hearthstoneAssemblyPath);
        }

        /// <summary>
        /// Is the path a Hearthstone directory.
        /// </summary>
        /// <param name="path">The path as string.</param>
        /// <returns>True iff the path is a Hearthstone directory.</returns>
        private static bool IsHearthstoneDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                return false;
            }

            if (File.Exists(Path.Combine(path, RelativePathToHearthstoneAssemblyDirectory, HearthstoneAssemblyName)))
            {
                return true;
            }

            return false;
        }


        public async Task<string> SearchHearthstoneDirectoryAsync()
        {
            return await Task.Run(() => SearchHearthstoneDirectory());
        }

        /// <summary>
        /// Search Hearthstone directory (from a built-in lists)
        /// </summary>
        /// <returns>The hearthstone directory</returns>
        private string SearchHearthstoneDirectory()
        {
            var candicates = new string[] { "", @"C:\Program Files\", @"C:\Program Files (x86)\", @"C:\Games\" };

            foreach (var candicate in candicates)
            {
                if (IsHearthstoneDirectory(candicate))
                {
                    return candicate;
                }
            }

            throw new Exception();
        }

        /// <summary>
        /// Setup Hearthrock environments
        /// </summary>
        /// <param name="root">The Hearthstone root path.</param>
        /// <returns>True iff Hearthrock environments setup successful.</returns>
        private static bool SetupHearthrock(string root)
        {
            var hearthstoneAssemblyPath = Path.Combine(root, RelativePathToHearthstoneAssemblyDirectory, HearthstoneAssemblyName);
            var hearthstoneBackupAssemblyPath = Path.Combine(root, RelativePathToHearthstoneAssemblyDirectory, HearthstoneBackupAssemblyName);
            var hearthrockAssemblyPath = Path.Combine(root, RelativePathToHearthstoneAssemblyDirectory, HearthrockAssemblyName);

            File.Copy(hearthstoneAssemblyPath, hearthstoneBackupAssemblyPath, true);
            File.Copy(HearthrockAssemblyName, hearthrockAssemblyPath, true);

            return true;
        }


        public async Task RecoverHearthstoneAsync(string root, string clientRoot)
        {
            await Task.Run(() => RecoverHearthstone(root, clientRoot));
        }

        private void RecoverHearthstone(string root, string clientRoot)
        {
            var sampleHearthstoneAssembly = Path.Combine(clientRoot, HearthstoneAssemblyName);
            var hearthstoneAssemblyPath = Path.Combine(root, RelativePathToHearthstoneAssemblyDirectory, HearthstoneAssemblyName);
            var hearthstoneBackupAssemblyPath = Path.Combine(root, RelativePathToHearthstoneAssemblyDirectory, HearthstoneBackupAssemblyName);

            if (!File.Exists(sampleHearthstoneAssembly))
            {
                throw new Exception();
            }

            if (File.Exists(hearthstoneAssemblyPath))
            {
                if (CompareFiles(hearthstoneAssemblyPath, sampleHearthstoneAssembly))
                {
                    return;
                }
            }


            if (File.Exists(hearthstoneBackupAssemblyPath))
            {
                if (CompareFiles(hearthstoneBackupAssemblyPath, sampleHearthstoneAssembly))
                {
                    File.Copy(hearthstoneBackupAssemblyPath, hearthstoneAssemblyPath, true);
                    return;
                }
            }

            throw new Exception();
        }

        public async Task<RockConfiguration> ReadConfigurationAsync(string root)
        {
            return await Task.Run(() => ReadConfiguration(root));
        }

        private RockConfiguration ReadConfiguration(string root)
        {
            var hearthstoneConfigurationPath = Path.Combine(root, RelativePathToHearthstoneAssemblyDirectory, HearthrockConfigurationName);
            var configurationJson = File.ReadAllText(hearthstoneConfigurationPath);
            var configuration = JsonConvert.DeserializeObject<RockConfiguration>(configurationJson);

            return configuration;
        }

        public async Task WriteConfigurationAsync(string root, RockConfiguration rockConfiguration)
        {
            await Task.Run(() => WriteConfiguration(root, rockConfiguration));
        }

        private void WriteConfiguration(string root, RockConfiguration rockConfiguration)
        {
            var hearthstoneConfigurationPath = Path.Combine(root, RelativePathToHearthstoneAssemblyDirectory, HearthrockConfigurationName);
            var configurationJson = JsonConvert.SerializeObject(rockConfiguration, Formatting.Indented);
            File.WriteAllText(hearthstoneConfigurationPath, configurationJson);
        }

        /// <summary>
        /// Compare two files.
        /// </summary>
        /// <param name="fileName1">The path of the first file.</param>
        /// <param name="fileName2">The path of the second file.</param>
        /// <returns>True iff the files are equal.</returns>
        private bool CompareFiles(string fileName1, string fileName2)
        {
            FileInfo fi1 = new FileInfo(fileName1);
            FileInfo fi2 = new FileInfo(fileName2);

            if (fi1.Length != fi2.Length)
            {
                return false;
            }

            var length = fi1.Length;

            using (FileStream fs1 = fi1.OpenRead())
            using (FileStream fs2 = fi2.OpenRead())
            using (BufferedStream bs1 = new BufferedStream(fs1))
            using (BufferedStream bs2 = new BufferedStream(fs2))
            {
                for (long i = 0; i < length; i++)
                {
                    if (bs1.ReadByte() != bs2.ReadByte())
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
