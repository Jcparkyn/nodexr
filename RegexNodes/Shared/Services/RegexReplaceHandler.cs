using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace RegexNodes.Shared.Services
{
    public class RegexReplaceHandler
    {
        readonly INodeHandler nodeHandler;
        public RegexReplaceHandler(INodeHandler NodeHandler)
        {
            this.nodeHandler = NodeHandler;
        }

        public string ReplacementRegex { get; set; } = "cow";
        public string SearchText { get; set; } =
            "The quick brown fox jumped over the lazy dog." +
            "\nDrag nodes in from the left to start building an expression. " +
            "Check the results of search and replace as you work, using the bottom panels. " +
            "You can also start from an existing Regex using the 'edit' button at the top left.";

        public MatchCollection GetAllMatches()
        {
            //return Regex.Matches("" + SearchText, nodeHandler.CachedOutput, RegexOptions.None, TimeSpan.FromSeconds(0.5));
            try
            {
                return Regex.Matches("" + SearchText, nodeHandler.CachedOutput.Expression, RegexOptions.None, TimeSpan.FromSeconds(0.5));
            }
            catch (RegexMatchTimeoutException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public string GetReplaceResult()
        {
            //return Regex.Replace(SearchText, nodeHandler.CachedOutput, ReplacementRegex);
            string result;
            try
            {
                result = Regex.Replace(SearchText, nodeHandler.CachedOutput.Expression, ReplacementRegex, RegexOptions.None, TimeSpan.FromSeconds(0.5));
            }
            catch (RegexMatchTimeoutException ex)
            {
                result = "Regex replace timed out: " + ex.Message;
            }
            catch (Exception ex)
            {
                result = "Error: " + ex.Message;
            }
            return result;
        }
    }
}
