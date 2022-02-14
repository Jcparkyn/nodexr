namespace Nodexr.Components;

using Microsoft.JSInterop;
using Nodexr.Utils;
using Nodexr.Services;
using Microsoft.AspNetCore.WebUtilities;

public partial class OutputDisplay
{
    private bool isEditing = false;
    private JSModule clipboardModule = null!;

    protected override void OnInitialized()
    {
        NodeHandler.OutputChanged += (sender, e) => StateHasChanged();
        clipboardModule = JSRuntime.LoadJSModule("./js/GeneratedJS/ClipboardCopy.js");
    }

    private void OnEditButtonClick()
    {
        isEditing = !isEditing;
    }

    private void OnEditSubmitted(string newExpression)
    {
        isEditing = false;
        NodeHandler.TryCreateTreeFromRegex(newExpression.Trim('\n', '\r'));
    }

    private void OnEditCancelled()
    {
        isEditing = false;
        StateHasChanged();
    }

    private async Task OnCreateLinkButtonClick()
    {
        var urlParams = new Dictionary<string, string>
        {
            {"parse", NodeHandler.CachedOutput.Expression}
        };
        if (RegexReplaceHandler.SearchText != RegexReplaceHandler.DefaultSearchText)
        {
            urlParams.Add("search", RegexReplaceHandler.SearchText);
        }

        if (RegexReplaceHandler.ReplacementRegex != RegexReplaceHandler.DefaultReplacementRegex)
        {
            urlParams.Add("replace", RegexReplaceHandler.ReplacementRegex);
        }

        string url = QueryHelpers.AddQueryString(NavManager.BaseUri, urlParams);
        await clipboardModule.InvokeVoidAsync("copyText", url, "");
        ToastService.ShowInfo("", "Link copied to clipboard");
    }

    private async Task CopyTextToClipboard()
    {
        string regex = NodeHandler.CachedOutput.Expression;
        await clipboardModule.InvokeVoidAsync("copyText", regex, "");
        ToastService.ShowInfo(regex, "Copied to clipboard");
    }
}
