using Nodexr.ApiShared;
using Nodexr.ApiShared.Pagination;
using Nodexr.Shared.NodeTreeBrowser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Nodexr.Shared.Services
{
    public class NodeTreeBrowserService
    {
        private readonly HttpClient httpClient;
        private readonly INodeHandler nodeHandler;
        private NodeTreePreviewModel selectedNodeTree;

        public event EventHandler SelectedNodeTreeChanged;
        public NodeTreePreviewModel SelectedNodeTree
        {
            get => selectedNodeTree;
            set
            {
                selectedNodeTree = value;
                SelectedNodeTreeChanged(this, EventArgs.Empty);
            }
        }

        public NodeTreeBrowserService(HttpClient httpClient, INodeHandler nodeHandler)
        {
            this.httpClient = httpClient;
            this.nodeHandler = nodeHandler;
        }

        public void LoadSelectedNodeTree()
        {
            if (SelectedNodeTree is null)
            {
                return;
            }
            nodeHandler.TryCreateTreeFromRegex(SelectedNodeTree.Expression);
            //TODO: Load search/replace strings
        }

        public async Task<Paged<NodeTreePreviewModel>> GetAllNodeTrees(CancellationToken cancellationToken, int start = 0, int limit = 50)
        {
            var httpResponse = await httpClient.GetFromJsonAsync<Paged<NodeTreePreviewModel>>(
                    $"http://localhost:7071/api/nodetree?start={start}&limit={limit}",
                    cancellationToken
                    );

            return httpResponse;
        }
    }
}
