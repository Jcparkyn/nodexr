namespace Nodexr.Serialization;
using System.Text.Json.Serialization;

/// <summary>
/// A copy of <see cref="ReferenceHandler.Preserve"/> that allows for object references
/// to be reused across multiple calls to Serialize.
/// </summary>
internal class CachePreservingReferenceHandler : ReferenceHandler
{
    public CachePreservingReferenceHandler() => _rootedResolver = new PreserveReferenceResolver();
    private readonly ReferenceResolver _rootedResolver;
    public override ReferenceResolver CreateResolver() => _rootedResolver;
}
