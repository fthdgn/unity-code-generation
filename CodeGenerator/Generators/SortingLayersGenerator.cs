using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;

namespace CodeGenerator.Generators
{
    public static class SortingLayersGenerator
    {
        [InitializeOnLoadMethod]
        public static void Starter()
        {
            new Instance().Start();
        }

        private class Instance : BaseGenerator
        {
            private string[] SortingLayers
            {
                get
                {
                    var internalEditorUtilityType = typeof(InternalEditorUtility);
                    var sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames",
                        BindingFlags.Static |
                        BindingFlags.NonPublic);
                    if (!(sortingLayersProperty is null))
                    {
                        return (string[]) sortingLayersProperty.GetValue(null, new object[0]);
                    }

                    return new string[] { };
                }
            }

            protected override string FileName => "SortingLayers";

            protected override string GenerateCode()
            {
                var lines = new List<string> {"public static class SortingLayers", "{"};
                lines.AddRange(
                    SortingLayers.Select(
                        layer => $@"    public const string {Common.MakeIdentifier(layer)} = ""{layer}"";"));
                lines.Add("}");
                return lines.Aggregate("", (current, line) => current + line + Environment.NewLine);
            }
        }
    }
}
