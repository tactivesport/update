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

        [MenuItem("Build/Build and zip V1 %g")]
        public static void BuildV1()
        {
            var version = "V1";
            var executableFile = Build(version);
            Zip(executableFile, version);
        }

        [MenuItem("Build/Build and zip V2 %h")]
        public static void BuildV2()
        {
            var version = "V2";
            var executableFile = Build(version);
            Zip(executableFile, version);
        }

        [MenuItem("Build/Build V1 and V2 and zip to project folder %j")]
        public static void BuildBoth()
        {
            var version = "V1";
            var executable = Build(version);
            Zip(executable, version);
            
            version = "V2";
            executable = Build(version);
            Zip(executable, version);
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
            var startPath = Path.GetDirectoryName(executableFile);
            var destination = Path.Combine(new DirectoryInfo(Application.dataPath).Parent!.Parent!.FullName, $"{version}.zip");
            
            if (File.Exists(destination))
                File.Delete(destination);
            
            ZipFile.CreateFromDirectory(startPath, destination, CompressionLevel.Fastest, false);
            
            Directory.Delete(startPath!, true);
            Directory.CreateDirectory(startPath);
            

        }

        static void FindScene(string sceneName)
        {
            var scene = Directory.GetFiles(@"Assets\", sceneName, SearchOption.AllDirectories).First();
            EditorBuildSettingsScene[] toAdd = {new EditorBuildSettingsScene(scene, true)};
            EditorBuildSettings.scenes = toAdd;
        }
    }
}