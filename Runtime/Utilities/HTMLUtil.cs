using System.Text;

namespace BehaviorGraph.Runtime.Utilities
{
    public static class TextU
    {
        private const string BOLD = "b`";
        private const string ITALIC = "i`";
        private const string ITALIC_BOLD = "ib`";

        public static string ParseHtmlString(string info)
        {
            var words = info.Split(' ');
            var sb = new StringBuilder();
            foreach (var word in words)
            {
                 if (word.Contains(ITALIC_BOLD))
                    sb.Append($"<i><b>{word.TrimStart(ITALIC_BOLD.ToCharArray())}</b></i>");

                else if(word.Contains(BOLD)) 
                    sb.Append($"<b>{word.TrimStart(ITALIC_BOLD.ToCharArray())}</b>");
                
                else if (word.Contains(ITALIC)) 
                    sb.Append($"<i>{word.TrimStart(ITALIC.ToCharArray())}</i>");
                
             
                else
                    sb.Append(word);

                sb.Append(" ");
            }
            return sb.ToString();
        }
    }
}
