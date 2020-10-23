using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace CodeGenerator.Generators
{
    public static class AnimatorParameterGenerator
    {
        [InitializeOnLoadMethod]
        public static void Starter()
        {
            new Instance().Start();
        }

        private class Instance : BaseGenerator
        {
            protected override string FileName => "AnimatorControllers";

            protected override string GenerateCode()
            {
                var lines = new List<string> {"using UnityEngine;", Environment.NewLine};
                var controllerCodes = AnimatorControllerDirectoryGeneratedCode("Assets");
                lines.AddRange(controllerCodes);
                return lines.Aggregate("", (current, line) => current + line);
            }

            private static List<string> AnimatorControllerDirectoryGeneratedCode(string path)
            {
                var controllerCodes = new List<string>();

                foreach (var directory in Directory.GetDirectories(path))
                {
                    controllerCodes.AddRange(AnimatorControllerDirectoryGeneratedCode(directory));
                }

                controllerCodes.AddRange(Directory.GetFiles(path).Select(AnimatorControllerFileGeneratedCode));

                return controllerCodes;
            }

            private class AnimatorControllerInfo
            {
                public List<AnimatorControllerParameterInfo> ParameterInfos;

                public class AnimatorControllerParameterInfo
                {
                    public string Name;
                    public AnimatorControllerParameterType Type;
                }
            }

            [CanBeNull]
            private static string AnimatorControllerFileGeneratedCode(string path)
            {
                if (!path.EndsWith(".controller")) return null;
                var file = new StreamReader(path);
                string fileLine;

                AnimatorControllerInfo info = null;

                while ((fileLine = file.ReadLine()) != null)
                {
                    if (fileLine == "AnimatorController:")
                    {
                        info = new AnimatorControllerInfo();
                    }
                    else if (fileLine == "  m_AnimatorParameters:" && info != null)
                    {
                        info.ParameterInfos = new List<AnimatorControllerInfo.AnimatorControllerParameterInfo>();
                    }
                    else if (fileLine.StartsWith("  - m_Name: ") && info?.ParameterInfos != null)
                    {
                        info.ParameterInfos.Add(new AnimatorControllerInfo.AnimatorControllerParameterInfo());
                        info.ParameterInfos.Last().Name = fileLine.Replace("  - m_Name: ", "");
                    }
                    else if (fileLine.StartsWith("    m_Type: ") && info?.ParameterInfos != null)
                    {
                        info.ParameterInfos.Last().Type =
                            (AnimatorControllerParameterType) int.Parse(fileLine.Replace("    m_Type: ", ""));
                    }
                    else if (fileLine.StartsWith("---") && info != null)
                    {
                        break;
                    }
                }

                file.Close();

                if (info == null) return null;

                var namespaceParts = new List<string> {"AnimatorControllers"};
                namespaceParts.AddRange(from s in Path.GetDirectoryName(path)?.Split(Path.DirectorySeparatorChar)
                                        select Common.MakeIdentifier(s));
                var namespaceFromPath = string.Join(".", namespaceParts);
                var className = Common.MakeIdentifier(Path.GetFileName(path).Replace(".controller", ""));
                var lines = new List<string>
                {
                    "",
                    $"namespace {namespaceFromPath}",
                    "{",
                    $"    public class {className}",
                    "    {",
                    "        public readonly Animator Animator;",
                    "",
                    $"        public {className}(Animator animator)",
                    "        {",
                    "            Animator = animator;",
                    "        }",
                };
                if (info.ParameterInfos != null)
                {
                    foreach (var parameter in info.ParameterInfos)
                    {
                        var parameterValidIdentifier = Common.MakeIdentifier(parameter.Name);

                        lines.Add("");
                        lines.Add(
                            $"        private static readonly int {parameterValidIdentifier} = Animator.StringToHash(\"{parameter.Name}\");");
                        lines.Add("");
                        switch (parameter.Type)
                        {
                            case AnimatorControllerParameterType.Bool:
                                lines.Add($"        public void Set{parameterValidIdentifier}(bool value)");
                                lines.Add("        {");
                                lines.Add($"            Animator.SetBool({parameterValidIdentifier}, value);");
                                lines.Add("        }");
                                break;
                            case AnimatorControllerParameterType.Float:
                                lines.Add($"        public void Set{parameterValidIdentifier}(float value)");
                                lines.Add("        {");
                                lines.Add($"            Animator.SetFloat({parameterValidIdentifier}, value);");
                                lines.Add("        }");
                                break;
                            case AnimatorControllerParameterType.Int:
                                lines.Add($"        public void Set{parameterValidIdentifier}(int value)");
                                lines.Add("        {");
                                lines.Add($"            Animator.SetInteger({parameterValidIdentifier}, value);");
                                lines.Add("        }");
                                break;
                            case AnimatorControllerParameterType.Trigger:
                                lines.Add($"        public void Set{parameterValidIdentifier}()");
                                lines.Add("        {");
                                lines.Add($"            Animator.SetTrigger({parameterValidIdentifier});");
                                lines.Add("        }");
                                lines.Add("");
                                lines.Add($"        public void Reset{parameterValidIdentifier}()");
                                lines.Add("        {");
                                lines.Add($"        Animator.ResetTrigger({parameterValidIdentifier});");
                                lines.Add("        }");
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }

                lines.Add("    }");
                lines.Add("}");

                return lines.Aggregate("", (current, line) => current + line + Environment.NewLine);
            }
        }
    }
}
