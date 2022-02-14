namespace Nodexr.Services;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Nodexr.Api.Contracts.NodeTrees;
using Nodexr.Api.Contracts.Pagination;
using Nodexr.Serialization;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Web;

public class NodeTreeBrowserService
{
    private readonly HttpClient httpClient;
    private readonly INodeHandler nodeHandler;
    private readonly NavigationManager navManager;
    private readonly string apiAddress;
    private NodeTreePreviewDto? selectedNodeTree;

    public event EventHandler? SelectedNodeTreeChanged;
    public NodeTreePreviewDto? SelectedNodeTree
    {
        get => selectedNodeTree;
        set
        {
            selectedNodeTree = value;
            SelectedNodeTreeChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public NodeTreeBrowserService(
        HttpClient httpClient,
        NavigationManager navManager,
        INodeHandler nodeHandler,
        IConfiguration config)
    {
        this.httpClient = httpClient;
        this.nodeHandler = nodeHandler;
        this.navManager = navManager;
        apiAddress = config["apiAddress"];
    }

    public void LoadSelectedNodeTree()
    {
        if (SelectedNodeTree?.Expression is null)
            return;
        nodeHandler.TryCreateTreeFromRegex(SelectedNodeTree.Expression);
        //TODO: Load search/replace strings
    }

    public async Task PublishNodeTree(CreateNodeTreeCommand model)
    {
        await httpClient.PostAsJsonAsync(
            $"{apiAddress}/nodetree",
            model
            ).ConfigureAwait(false);
    }

    public async Task<string?> PublishAnonymousNodeTree()
    {
        var jsonOptions = new JsonSerializerOptions()
        {
            ReferenceHandler = new CachePreservingReferenceHandler(),
            Converters = { new RegexNodeJsonConverter() },
        };

        var nodes = JsonObject.Create(
            JsonSerializer.SerializeToElement(nodeHandler.Tree.Nodes, jsonOptions)
        )!;
        var command = new CreateAnonymousNodeTreeCommand(nodes);

        var response = await httpClient.PostAsJsonAsync($"{apiAddress}/nodetree/anon", command);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }
        else
        {
            var nodeTreeId = await response.Content.ReadAsStringAsync();
            return navManager.ToAbsoluteUri($"shared/{HttpUtility.UrlEncode(nodeTreeId)}").AbsoluteUri;
        }
    }

    public async Task<Paged<NodeTreePreviewDto>> GetAllNodeTrees(CancellationToken cancellationToken, string? search = null, int start = 0, int limit = 50)
    {
        var queryParams = new Dictionary<string, string>
            {
                { "start", start.ToString() },
                { "limit", limit.ToString() },
            };

        if (!string.IsNullOrEmpty(search))
            queryParams.Add("search", search);

        string uri = QueryHelpers.AddQueryString($"{apiAddress}/nodetree", queryParams);

        return await httpClient.GetFromJsonAsync<Paged<NodeTreePreviewDto>>(
                uri,
                cancellationToken
                ).ConfigureAwait(false) ?? throw new Exception("API response could not be parsed");
    }
}
