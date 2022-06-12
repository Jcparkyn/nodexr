namespace Nodexr.Utils;
using BlazorNodes.Core;
using Microsoft.JSInterop;

public static class ExtensionMethods
{
    public static Vector2 GetClientPos(this Microsoft.AspNetCore.Components.Web.MouseEventArgs e)
    {
        return new Vector2(e.ClientX, e.ClientY);
    }

    public static string InNonCapturingGroup(this string input) => $"(?:{input})";

    public static bool IsSingleRegexChar(this string input)
    {
        return input.Length <= 1 || (input.Length == 2 && input.StartsWith("\\", StringComparison.InvariantCulture));
    }

    public static string EscapeCharacters(this string input, IEnumerable<char> chars)
    {
        string result = "";
        for (int i = 0; i < input.Length; i++)
        {
            if (chars.Contains(input[i]))
            {
                //Escape the current character
                result += @"\";
            }
            result += input[i];
        }
        return result;
    }

    public static string EscapeSpecialCharacters(this string input, bool escapeBackslashes = true)
    {
        const string specialCharacters = "()[]{}$^?.+*|";

        string charsToEscape = escapeBackslashes ?
            specialCharacters + "\\" :
            specialCharacters;

        return EscapeCharacters(input, charsToEscape);
    }

    public static IEnumerable<(int index, T item)> Enumerate<T>(this IEnumerable<T> self) =>
        self?.Select((item, index) => (index, item)) ?? Enumerable.Empty<(int, T)>();

    /// <summary>
    /// Begins asynchronously loading a JS module from the given path.
    /// </summary>
    /// <remarks>
    /// The module can be used immediately, and calls will automatically
    /// begin executing once the module is loaded.
    /// </remarks>
    public static JSModule LoadJSModule(this IJSRuntime jsRuntime, string path)
    {
        return new JSModule(jsRuntime.InvokeAsync<IJSObjectReference>("import", path).AsTask());
    }
}
