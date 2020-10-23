using System.IO;
using UnityEditor;

namespace CodeGenerator
{
    public static class Editor
    {
        [MenuItem("Code Generation/Refresh")]
        public static void Refresh()
        {
            foreach (var file in Directory.GetFiles(Common.GeneratedCodeRoot))
            {
                var fileName = Path.GetFileName(file);
                if (fileName != "GeneratedCode.asmdef" &&
                    fileName != "GeneratedCode.asmdef.meta" &&
                    fileName != ".gitignore")
                {
                    File.Delete(file);
                }
            }
        }
    }
}
