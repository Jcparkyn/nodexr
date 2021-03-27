using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace Nodexr.Shared.Services
{
    public class RegexReplaceHandler
    {
        private readonly INodeHandler nodeHandler;
        private RegexOptions options = RegexOptions.None;

        public const string DefaultReplacementRegex = "animal";

        public string ReplacementRegex { get; set; } = DefaultReplacementRegex;

        public const string DefaultSearchText = "The quick brown fox jumped over the lazy dog.";

        public string SearchText { get; set; } = DefaultSearchText;

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

        public RegexReplaceHandler(INodeHandler NodeHandler, NavigationManager navManager)
        {
            nodeHandler = NodeHandler;

            var uriParams = QueryHelpers.ParseQuery(navManager.ToAbsoluteUri(navManager.Uri).Query);
            if (uriParams.TryGetValue("search", out var searchString))
            {
                SearchText = searchString[0];
            }
            if (uriParams.TryGetValue("replace", out var replaceString))
            {
                ReplacementRegex = replaceString[0];
            }
        }

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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public string GetReplaceResult()
        {
            if (!IsRegexOptionsValid(Options))
            {
                return "ECMAScript mode must only be used with Multiline and Ignore Case flags";
            }

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

        private static bool IsRegexOptionsValid(RegexOptions options)
        {
            //Options can only be invalid in ECMAScript mode
            if (!options.HasFlag(RegexOptions.ECMAScript)) return true;

            const RegexOptions disallowedFlags = ~(
                RegexOptions.Multiline |
                RegexOptions.IgnoreCase);

            //Regex is only allowed to have multiline or ignoreCase flags when in ECMAScript mode
            return (options & disallowedFlags & ~RegexOptions.ECMAScript) == 0;
        }
    }
}
