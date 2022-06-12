namespace Nodexr.Components;

using Microsoft.JSInterop;
using Nodexr.Utils;
using Nodexr.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Components;
using Blazored.Toast.Services;
using Microsoft.FeatureManagement;
using Microsoft.AspNetCore.Components.Web;

public partial class OutputDisplay
{
    [Inject] private INodeHandler NodeHandler { get; set; } = null!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = null!;
    [Inject] private NavigationManager NavManager { get; set; } = null!;
    [Inject] private IToastService ToastService { get; set; } = null!;
    [Inject] private RegexReplaceHandler RegexReplaceHandler { get; set; } = null!;
    [Inject] private IFeatureManager FeatureManager { get; set; } = null!;
    [Inject] private NodeTreeBrowserService BrowserService { get; set; } = null!;

    private bool isEditing;
    private JSModule clipboardModule = null!;
    private Mutation<int, string?> createLinkMutation = null!;

    protected override void OnInitialized()
    {
        NodeHandler.OutputChanged += StateHasChanged;
        clipboardModule = JSRuntime.LoadJSModule("./js/GeneratedJS/ClipboardCopy.js");
        createLinkMutation = new Mutation<int, string?>(
            StateHasChanged,
            _ => BrowserService.PublishAnonymousNodeTree(),
            onError: _ => ToastService.ShowError("Something went wrong while contacting the server, please try again.")
        );
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

    private async Task OnCreateLinkButtonClick(MouseEventArgs eventArgs)
    {
        var useServerLink = eventArgs.ShiftKey && await FeatureManager.IsEnabledAsync("ServerLinks");
        var url = useServerLink
            ? await createLinkMutation.Trigger(0)
            : GetBasicLink();

        if (url is null)
        {
            ToastService.ShowError("", "Something went wrong creating the link, please try again");
        }
        else
        {
            await clipboardModule.InvokeVoidAsync("copyText", url, "");
            ToastService.ShowInfo(url, "Link copied to clipboard");
        }
    }

    private string GetBasicLink()
    {
        var urlParams = new Dictionary<string, string>
        {
            { "parse", NodeHandler.CachedOutput.Expression }
        };
        if (RegexReplaceHandler.SearchText != RegexReplaceHandler.DefaultSearchText)
        {
            urlParams.Add("search", RegexReplaceHandler.SearchText);
        }

        if (RegexReplaceHandler.ReplacementRegex != RegexReplaceHandler.DefaultReplacementRegex)
        {
            urlParams.Add("replace", RegexReplaceHandler.ReplacementRegex);
        }

        return QueryHelpers.AddQueryString(NavManager.BaseUri, urlParams);
    }

    private async Task CopyTextToClipboard()
    {
        string regex = NodeHandler.CachedOutput.Expression;
        await clipboardModule.InvokeVoidAsync("copyText", regex, "");
        ToastService.ShowInfo(regex, "Copied to clipboard");
    }
}
