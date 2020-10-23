using System.IO;
using UnityEditor;

namespace CodeGenerator.Generators
{
    public static class EssentialFileGenerator
    {
        [InitializeOnLoadMethod]
        public static void Generate()
        {
            Directory.CreateDirectory(Common.GeneratedCodeRoot);
            GenerateFile($"{Common.GeneratedCodeRoot}GeneratedCode.asmdef", @"
{
    ""name"": ""GeneratedCode"",
    ""references"": [],
    ""includePlatforms"": [],
    ""excludePlatforms"": [],
    ""allowUnsafeCode"": false,
    ""overrideReferences"": false,
    ""precompiledReferences"": [],
    ""autoReferenced"": true,
    ""defineConstraints"": [],
    ""versionDefines"": [],
    ""noEngineReferences"": false
}");

            GenerateFile($"{Common.GeneratedCodeRoot}.gitignore", @"/**");


            GenerateFile($"{Common.GeneratedCodeRoot}package.json", @"
{
  ""name"": ""generatedcode"",
  ""version"": ""1.0.0"",
  ""displayName"": ""Generated Code"",
  ""description"": ""Generated Code""
}");
        }

        private static void GenerateFile(string path, string content)
        {
            var file = new FileInfo(path);
            if (file.Exists) return;
            var streamWriter = file.AppendText();
            streamWriter.Write(content);
            streamWriter.Close();
        }
    }
}
