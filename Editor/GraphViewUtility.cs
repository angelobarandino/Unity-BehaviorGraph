using System;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;

namespace BehaviourGraph.Editor
{
    public static class GraphViewUtility
    {
        public static string ToFriendlyName(this string text)
        {
            var invalidCharsRgx = new Regex("[^_a-zA-Z0-9]");
            var whiteSpace = new Regex(@"(?<=\s)");
            var startsWithLowerCaseChar = new Regex("^[a-z]");
            var firstCharFollowedByUpperCasesOnly = new Regex("(?<=[A-Z])[A-Z0-9]+$");
            var lowerCaseNextToNumber = new Regex("(?<=[0-9])[a-z]");
            var upperCaseInside = new Regex("(?<=[A-Z])[A-Z]+?((?=[A-Z][a-z])|(?=[0-9]))");

            // replace white spaces with undescore, then replace all invalid chars with empty string
            var pascalCase = invalidCharsRgx.Replace(whiteSpace.Replace(text, "_"), string.Empty)
                // split by underscores
                .Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries)
                // set first letter to uppercase
                .Select(w => startsWithLowerCaseChar.Replace(w, m => m.Value.ToUpper()))
                // replace second and all following upper case letters to lower if there is no next lower (ABC -> Abc)
                .Select(w => firstCharFollowedByUpperCasesOnly.Replace(w, m => m.Value.ToLower()))
                // set upper case the first lower case following a number (Ab9cd -> Ab9Cd)
                .Select(w => lowerCaseNextToNumber.Replace(w, m => m.Value.ToUpper()))
                // lower second and next upper case letters except the last if it follows by any lower (ABcDEf -> AbcDef)
                .Select(w => upperCaseInside.Replace(w, m => m.Value.ToLower()));

            return new string(string.Concat(pascalCase).SelectMany((c, i) => i > 0 && char.IsUpper(c) ? new[] { ' ', c } : new[] { c }).ToArray());
        }

        public static string ToCamelCase(this string str)
        {
            var words = str.Split(new[] { "_", " " }, StringSplitOptions.RemoveEmptyEntries);

            var leadWord = words[0].ToLower();

            var tailWords = words.Skip(1).Select(word => char.ToUpper(word[0]) + word.Substring(1)).ToArray();

            return $"{leadWord}{string.Join(string.Empty, tailWords)}";
        }
    }
}
