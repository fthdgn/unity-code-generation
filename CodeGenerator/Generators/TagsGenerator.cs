using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;

namespace CodeGenerator.Generators
{
    public static class TagsGenerator
    {
        [InitializeOnLoadMethod]
        public static void Starter()
        {
            new Instance().Start();
        }

        private class Instance : BaseGenerator
        {
            protected override string FileName => "Tags";

            protected override string GenerateCode()
            {
                var lines = new List<string> {"public static class Tags", "{"};
                lines.AddRange(
                    InternalEditorUtility
                        .tags
                        .Select(tag => $@"   public const string {Common.MakeIdentifier(tag)} = ""{tag}"";"));
                lines.Add("}");
                return lines.Aggregate("", (current, line) => current + line + Environment.NewLine);
            }
        }
    }
}
