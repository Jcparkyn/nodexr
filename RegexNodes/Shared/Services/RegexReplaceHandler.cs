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
        private RegexOptions options = RegexOptions.None;

        public RegexReplaceHandler(INodeHandler NodeHandler)
        {
            this.nodeHandler = NodeHandler;
        }

        public string ReplacementRegex { get; set; } = "cow";
        public string SearchText { get; set; } =
            "The quick brown fox jumped over the lazy dog.";
        public RegexOptions Options
        {
            get => options;
            set
            {
                options = value;
                RegexOptionsChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public event EventHandler RegexOptionsChanged;

        public MatchCollection GetAllMatches()
        {
            //return Regex.Matches("" + SearchText, nodeHandler.CachedOutput, RegexOptions.None, TimeSpan.FromSeconds(0.5));
            try
            {
                return Regex.Matches("" + SearchText, nodeHandler.CachedOutput.Expression, Options, TimeSpan.FromSeconds(0.5));
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
                result = Regex.Replace(SearchText, nodeHandler.CachedOutput.Expression, ReplacementRegex, Options, TimeSpan.FromSeconds(0.5));
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
