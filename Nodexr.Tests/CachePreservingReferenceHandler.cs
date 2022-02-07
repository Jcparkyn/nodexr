namespace Nodexr.Tests;
using System.Text.Json.Serialization;

internal class CachePreservingReferenceHandler : ReferenceHandler
{
    public CachePreservingReferenceHandler() => Reset();
    private ReferenceResolver _rootedResolver;
    public override ReferenceResolver CreateResolver() => _rootedResolver;
    public void Reset() => _rootedResolver = new PreserveReferenceResolver();
}
