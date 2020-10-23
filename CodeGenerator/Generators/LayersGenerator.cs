using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;

namespace CodeGenerator.Generators
{
    public static class LayersGenerator
    {
        [InitializeOnLoadMethod]
        public static void Starter()
        {
            new Instance().Start();
        }

        private class Instance : BaseGenerator
        {
            protected override string FileName => "Layers";

            protected override string GenerateCode()
            {
                var lines = new List<string> {"public static class Layers", "{"};
                for (var i = 0; i < 32; i++)
                {
                    var layerName = InternalEditorUtility.GetLayerName(i);
                    if (layerName == "")
                    {
                        layerName = "Layer" + i;
                    }

                    lines.Add($"    public const int {Common.MakeIdentifier(layerName)} = {i};");
                }

                lines.Add("}");
                return lines.Aggregate("", (current, line) => current + line + Environment.NewLine);
            }
        }
    }
}
