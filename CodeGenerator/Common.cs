using System.Text.RegularExpressions;

namespace CodeGenerator
{
    public static class Common
    {
        public const string GeneratedCodeRoot = "Packages/generatedcode/";

        public static string MakeIdentifier(string str)
        {
            var result = Regex.Replace(str, "[^a-zA-Z0-9]", "");
            if ('0' <= result[0] && result[0] <= '9')
            {
                result = result.Insert(0, "_");
            }

            return result;
        }
    }
}
