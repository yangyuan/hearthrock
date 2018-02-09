// <copyright file="Patcher.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

using System.Linq;

namespace Hearthrock.Client.Hacking
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using Hearthrock.Bot.Exceptions;
    using Hearthrock.Contracts;
    using Mono.Cecil;
    using Newtonsoft.Json;

    /// <summary>
    /// The class to inject or remove Hearthrock
    /// </summary>
    public class Patcher
    {
        /// <summary>
        /// The name of Hearthrock main dll, aka Hearthrock.dll.
        /// </summary>
        private const string HearthrockAssemblyName = @"Hearthrock.dll";

        /// <summary>
        /// The name of Hearthrock configuration file, aka hearthrock.json.
        /// </summary>
        private const string HearthrockConfigurationFileName = @"hearthrock.json";

        /// <summary>
        /// The name of Hearthstone main dll, aka Assembly-CSharp.dll.
        /// </summary>
        private const string HearthstoneAssemblyName = @"Assembly-CSharp.dll";

        /// <summary>
        /// The name of Hearthstone main dll backup.
        /// </summary>
        private const string HearthstoneBackupAssemblyName = @"Assembly-CSharp-original.dll";

        /// <summary>
        /// Relative path to Hearthstone assembly directory, aka the directory contains Assembly-CSharp.dll.
        /// </summary>
        private const string RelativePathToHearthstoneAssemblyDirectory = @"Hearthstone_Data\Managed";

        /// <summary>
        /// The name of Hearthstone product file, aka .product.db.
        /// </summary>
        private const string HearthstoneProductFileName = @".product.db";

        /// <summary>
        /// The name of Pegasus information file, aka pegasus.json.
        /// </summary>
        private const string PegasusInformationFileName = @"pegasus.json";

        /// <summary>
        /// The root of Hearthstone.
        /// </summary>
        private string pegasusRoot;

        /// <summary>
        /// The root of Hearthrock.
        /// </summary>
        private string clientRoot;

        /// <summary>
        /// Initializes a new instance of the <see cref="Patcher" /> class.
        /// </summary>
        /// <param name="clientRoot">The client root.</param>
        public Patcher(string clientRoot)
        {
            this.pegasusRoot = string.Empty;
            this.clientRoot = clientRoot;
        }

        /// <summary>
        /// Gets or sets the root of Hearthstone.
        /// </summary>
        public string RootPath
        {
            get
            {
                return this.pegasusRoot;
            }

            set
            {
                if (!IsHearthstoneDirectory(value))
                {
                    throw new PegasusException("The directory does not conatins hearthstone");
                }

                this.pegasusRoot = value;
            }
        }

        /// <summary>
        /// Inject Hearthrock into The Hearthstone
        /// </summary>
        /// <returns>The async task.</returns>
        public async Task InjectAsync()
        {
            await Task.Run(() => this.Inject());
        }

        /// <summary>
        /// Inject Hearthrock into The Hearthstone
        /// </summary>
        public void Inject()
        {
            this.AssertRootPath();
            this.SetupHearthrock();

            var hearthstoneAssemblyPath = Path.Combine(this.pegasusRoot, RelativePathToHearthstoneAssemblyDirectory, HearthstoneAssemblyName);
            var hearthrockAssemblyPath = Path.Combine(this.pegasusRoot, RelativePathToHearthstoneAssemblyDirectory, HearthrockAssemblyName);
            var resolver = new DefaultAssemblyResolver();
            resolver.AddSearchDirectory(Path.Combine(this.pegasusRoot, RelativePathToHearthstoneAssemblyDirectory));

            var hearthrockAssembly = AssemblyDefinition.ReadAssembly(hearthrockAssemblyPath, new ReaderParameters { AssemblyResolver = resolver });
            var hearthstoneAssembly = AssemblyDefinition.ReadAssembly(hearthstoneAssemblyPath, new ReaderParameters { AssemblyResolver = resolver });

            MethodDefinition methodRockUnityHook = hearthrockAssembly.GetMethod("RockUnity", "Hook");
            MethodDefinition methodSceneMgrStart = hearthstoneAssembly.GetMethod("SceneMgr", "Start");
            AssemblyDefinition tmpAssembly = hearthstoneAssembly.InjectMethod(methodSceneMgrStart, methodRockUnityHook);

            MethodDefinition methodRockPlayZoneSlotMousedOver = hearthrockAssembly.GetMethod("RockGameHooks", "PlayZoneSlotMousedOver");
            MethodDefinition methodStonePlayZoneSlotMousedOver = hearthstoneAssembly.GetMethod("InputManager", "PlayZoneSlotMousedOver");
            tmpAssembly = tmpAssembly.HijackMethod(methodStonePlayZoneSlotMousedOver, methodRockPlayZoneSlotMousedOver);

            MethodDefinition methodRockGetMousePosition = hearthrockAssembly.GetMethod("RockGameHooks", "GetMousePosition");
            MethodDefinition methodStoneGetMousePosition = hearthstoneAssembly.GetMethod("UniversalInputManager", "GetMousePosition");

            tmpAssembly = tmpAssembly.HijackMethod(methodStoneGetMousePosition, methodRockGetMousePosition);

            tmpAssembly.Write(hearthstoneAssemblyPath);
        }

        /// <summary>
        /// Search Hearthstone Directory
        /// </summary>
        /// <returns>The Hearthstone Directory or null.</returns>
        public async Task<string> SearchHearthstoneDirectoryAsync()
        {
            return await Task.Run(() => SearchHearthstoneDirectory());
        }

        /// <summary>
        /// Recover Hearthstone client
        /// </summary>
        /// <returns>The async task.</returns>
        public async Task RecoverHearthstoneAsync()
        {
            await Task.Run(() => this.RecoverHearthstone());
        }

        /// <summary>
        /// Read Current Configuration
        /// </summary>
        /// <returns>The RockConfiguration.</returns>
        public async Task<RockConfiguration> ReadRockConfigurationAsync()
        {
            return await Task.Run(() => this.ReadRockConfiguration());
        }

        /// <summary>
        /// Write Configuration
        /// </summary>
        /// <param name="rockConfiguration">The RockConfiguration.</param>
        /// <returns>The async task.</returns>
        public async Task WriteRockConfigurationAsync(RockConfiguration rockConfiguration)
        {
            await Task.Run(() => this.WriteRockConfiguration(rockConfiguration));
        }

        /// <summary>
        /// Is the path a Hearthstone directory.
        /// </summary>
        /// <param name="path">The path as string.</param>
        /// <returns>True if the path is a Hearthstone directory.</returns>
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

        /// <summary>
        /// Get the SHA1 checksum of a file.
        /// </summary>
        /// <param name="path">The file path</param>
        /// <returns>The checksum in upper case.</returns>
        private static string CheckSum(string path)
        {
            SHA1 sha = new SHA1CryptoServiceProvider();
            var result = sha.ComputeHash(File.ReadAllBytes(path));

            string hex = BitConverter.ToString(result);
            return hex.Replace("-", string.Empty);
        }

        /// <summary>
        /// Search Hearthstone directory (from a built-in lists)
        /// </summary>
        /// <returns>The hearthstone directory</returns>
        private static string SearchHearthstoneDirectory()
        {
            List<string> candicates = new List<string>
            {
                string.Empty,
                @"C:\Program Files\Hearthstone\",
                @"C:\Program Files (x86)\Hearthstone\",
                @"E:\Program Files (x86)\HS\Hearthstone\"
            };

            for (int i = 2; i < 26; i++)
            {
                char drive = (char)(Convert.ToUInt16('A') + i);
                candicates.Add($@"{drive}:\Game\Hearthstone\");
                candicates.Add($@"{drive}:\Games\Hearthstone\");
            }

            foreach (var candicate in candicates)
            {
                if (IsHearthstoneDirectory(candicate))
                {
                    return candicate;
                }
            }

            throw new PegasusException("Cannot find Hearthstone, please set the path manually.");
        }

        /// <summary>
        /// Setup Hearthrock environments
        /// </summary>
        /// <returns>True if Hearthrock environments setup successful.</returns>
        private bool SetupHearthrock()
        {
            this.AssertRootPath();

            var pegasusInformation = this.ReadPegasusInformation();
            var pegasusVersion = this.ReadPegasusVersion();

            if (string.IsNullOrEmpty(pegasusVersion))
            {
                throw new PegasusException($"Cannot detect Hearthstone client version.");
            }

            if (!pegasusVersion.Contains(pegasusInformation.Version))
            {
                throw new PegasusException($"Hearthrock is out of data: supported version {pegasusInformation.Version}, current version {pegasusVersion}");
            }

            var rockHearthstoneAssembly = Path.Combine(this.clientRoot, HearthstoneAssemblyName);
            var rockAssemblyPath = Path.Combine(this.clientRoot, HearthrockAssemblyName);
            var hearthstoneAssemblyPath = Path.Combine(this.pegasusRoot, RelativePathToHearthstoneAssemblyDirectory, HearthstoneAssemblyName);
            var hearthstoneBackupAssemblyPath = Path.Combine(this.pegasusRoot, RelativePathToHearthstoneAssemblyDirectory, HearthstoneBackupAssemblyName);
            var hearthrockAssemblyPath = Path.Combine(this.pegasusRoot, RelativePathToHearthstoneAssemblyDirectory, HearthrockAssemblyName);

            File.Copy(hearthstoneAssemblyPath, hearthstoneBackupAssemblyPath, true);
            File.Copy(rockAssemblyPath, hearthrockAssemblyPath, true);
            File.Copy(rockHearthstoneAssembly, hearthstoneAssemblyPath, true);

            return true;
        }

        /// <summary>
        /// Recover Hearthstone client
        /// </summary>
        private void RecoverHearthstone()
        {
            this.AssertRootPath();

            var pegasusInformation = this.ReadPegasusInformation();
            var pegasusVersion = this.ReadPegasusVersion();

            var rockHearthstoneAssembly = Path.Combine(this.clientRoot, HearthstoneAssemblyName);
            var hearthstoneAssemblyPath = Path.Combine(this.pegasusRoot, RelativePathToHearthstoneAssemblyDirectory, HearthstoneAssemblyName);
            var hearthstoneBackupAssemblyPath = Path.Combine(this.pegasusRoot, RelativePathToHearthstoneAssemblyDirectory, HearthstoneBackupAssemblyName);

            if (!File.Exists(rockHearthstoneAssembly))
            {
                throw new PegasusException($"Cannot find {hearthstoneAssemblyPath} in Hearthrock client.");
            }

            if (string.IsNullOrEmpty(pegasusVersion))
            {
                throw new PegasusException($"Cannot detect Hearthstone client version.");
            }

            if (!pegasusVersion.Contains(pegasusInformation.Version))
            {
                if (File.Exists(hearthstoneBackupAssemblyPath))
                {
                    File.Delete(hearthstoneBackupAssemblyPath);
                }

                throw new PegasusException($"Hearthrock is out of data: supported version {pegasusInformation.Version}, current version {pegasusVersion}");
            }

            if (File.Exists(hearthstoneAssemblyPath))
            {
                if (CheckSum(hearthstoneAssemblyPath).Equals(pegasusInformation.Checksum))
                {
                    if (File.Exists(hearthstoneBackupAssemblyPath))
                    {
                        File.Delete(hearthstoneBackupAssemblyPath);
                    }

                    return;
                }
                else
                {
                    File.Delete(hearthstoneAssemblyPath);
                }
            }

            if (File.Exists(hearthstoneBackupAssemblyPath))
            {
                if (CheckSum(hearthstoneBackupAssemblyPath).Equals(pegasusInformation.Checksum))
                {
                    File.Copy(hearthstoneBackupAssemblyPath, hearthstoneAssemblyPath);
                    File.Delete(hearthstoneBackupAssemblyPath);

                    return;
                }
            }

            throw new PegasusException("Hearthstone files corrupted, please repair the client before continue.");
        }

        /// <summary>
        /// Read Current Pegasus Information
        /// </summary>
        /// <returns>The PegasusInformation.</returns>
        private PegasusInformation ReadPegasusInformation()
        {
            var pegasusInformationPath = Path.Combine(this.clientRoot, PegasusInformationFileName);
            if (File.Exists(pegasusInformationPath))
            {
                var informationJson = File.ReadAllText(pegasusInformationPath);
                var information = JsonConvert.DeserializeObject<PegasusInformation>(informationJson);

                return information;
            }

            throw new PegasusException("Pegasus information file not found or corrupted.");
        }

        /// <summary>
        /// Read Current Pegasus Information
        /// </summary>
        /// <returns>The PegasusInformation.</returns>
        private string ReadPegasusVersion()
        {
            this.AssertRootPath();
            var hearthstoneProductPath = Path.Combine(this.pegasusRoot, HearthstoneProductFileName);
            if (!File.Exists(hearthstoneProductPath))
            {
                throw new PegasusException("Pegasus product file not found or corrupted.");
            }

            string productInfo = File.ReadAllText(hearthstoneProductPath, Encoding.ASCII);
            string version = string.Empty;

            Regex regex = new Regex("([0-9]+\\.[0-9]+\\.[0-0]+\\.[0-9]+)");
            var versions = regex.Matches(productInfo);
            if (versions.Count == 1)
            {
                version = versions[0].Value;
            }

            return version;
        }

        /// <summary>
        /// Read Current Configuration
        /// </summary>
        /// <returns>The RockConfiguration.</returns>
        private RockConfiguration ReadRockConfiguration()
        {
            this.AssertRootPath();
            var hearthstoneConfigurationPath = Path.Combine(this.pegasusRoot, RelativePathToHearthstoneAssemblyDirectory, HearthrockConfigurationFileName);
            if (File.Exists(hearthstoneConfigurationPath))
            {
                var configurationJson = File.ReadAllText(hearthstoneConfigurationPath);
                var configuration = JsonConvert.DeserializeObject<RockConfiguration>(configurationJson);

                return configuration;
            }
            else
            {
                return new RockConfiguration();
            }
        }

        /// <summary>
        /// Write Configuration
        /// </summary>
        /// <param name="rockConfiguration">The RockConfiguration</param>
        private void WriteRockConfiguration(RockConfiguration rockConfiguration)
        {
            var hearthstoneConfigurationPath = Path.Combine(this.pegasusRoot, RelativePathToHearthstoneAssemblyDirectory, HearthrockConfigurationFileName);
            var configurationJson = JsonConvert.SerializeObject(rockConfiguration, Formatting.Indented);
            File.WriteAllText(hearthstoneConfigurationPath, configurationJson);
        }

        /// <summary>
        /// Assert the root path has been set.
        /// </summary>
        private void AssertRootPath()
        {
            if (IsHearthstoneDirectory(this.pegasusRoot))
            {
                return;
            }

            throw new PegasusException("Hearthstone client path not set.");
        }
    }
}
