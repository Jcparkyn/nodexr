using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace RegexNodes.Shared
{
    public class RegexReplaceHandler
    {
        INodeHandler nodeHandler;
        public RegexReplaceHandler(INodeHandler NodeHandler)
        {
            this.nodeHandler = NodeHandler;
        }

        public string ReplacementRegex { get; set; } = "cow";
        public string SearchText { get; set; } = "The quick brown fox jumped over the lazy dog.";

        public MatchCollection GetAllMatches()
        {
            try
            {
                return Regex.Matches(SearchText, nodeHandler.CachedOutput);
            }
            catch
            {
                return null;
            }
        }

        public string GetReplaceResult()
        {
            //return Regex.Replace(SearchText, nodeHandler.CachedOutput, ReplacementRegex);
            string result = "";
            try
            {
                result = Regex.Replace(SearchText, nodeHandler.CachedOutput, ReplacementRegex);
            }
            catch(Exception ex)
            {
                result = "Error: " + ex.Message;
            }
            return result;
        }
    }
}
