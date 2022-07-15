using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using UnityEditor;
using UnityEngine;
using CompressionLevel = System.IO.Compression.CompressionLevel;

namespace Editor
{
    public class TestBuild : UnityEditor.Editor
    {
        [MenuItem("Build/zip %k")]
        static void TryZip()
        {
            var executableFile = @"C:\Users\danie\Desktop\git\update\Update\Builds\V1.exe";
            Zip(executableFile, "V1.zip");
        }

        [MenuItem("Build/Build and play V1 %g")]
        public static void BuildV1()
        {
            var version = "V1";
            var executableFile = Build(version);
            Zip(executableFile, version);
            Start(executableFile);
            
        }

        [MenuItem("Build/Build and play V2 %h")]
        public static void BuildV2()
        {
            var executableFile = Build("V2");
            Start(executableFile);
        }

        [MenuItem("Build/Build V1 and V2 %j")]
        public static void BuildBoth()
        {
            var version = "V1";
            var executable = Build(version);
            Zip(executable, version);
        }

        static void Start(string executableFile)
        {
            var proc = new Process();
            proc.StartInfo.FileName = executableFile;
            proc.Start();
        }

        static string Build(string version)
        {
            var buildFolder = Path.Combine("./Builds");
            var executable = Path.Combine(buildFolder, version + ".exe");

            FindScene(version + ".unity");
            PlayerSettings.productName = version;

            if (!Directory.Exists(buildFolder))
                Directory.CreateDirectory(buildFolder);

            BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, executable, BuildTarget.StandaloneWindows,
                BuildOptions.None);
            return executable;
        }

        static void Zip(string executableFile, string version)
        {
            string startPath = Path.GetDirectoryName(executableFile);
            var destination = Path.Combine(new DirectoryInfo(Application.dataPath).Parent!.Parent!.FullName, version);
            
            if (Directory.Exists(destination))
                Directory.Delete(destination);
            
            ZipFile.CreateFromDirectory(startPath, destination, CompressionLevel.Fastest, false);
        }

        static void FindScene(string sceneName)
        {
            var scene = Directory.GetFiles(@"Assets\", sceneName, SearchOption.AllDirectories).First();
            EditorBuildSettingsScene[] toAdd = {new EditorBuildSettingsScene(scene, true)};
            EditorBuildSettings.scenes = toAdd;
        }
    }
}