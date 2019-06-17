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

        public string ReplacementRegex { get; set; } = "";
        public string SearchText { get; set; } = "Lorem Ipsum";

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
