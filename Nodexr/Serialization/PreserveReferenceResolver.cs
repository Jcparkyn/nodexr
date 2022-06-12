namespace Nodexr.Serialization;

using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// Copy of PreserveReferenceResolver from .NET source
/// </summary>
/// <remarks>
/// <see href="https://source.dot.net/#System.Text.Json/System/Text/Json/Serialization/PreserveReferenceResolver.cs,8d6a81e04efc92c3"/>
/// </remarks>
internal sealed class PreserveReferenceResolver : ReferenceResolver
{
    private uint _referenceCount;
    private readonly Dictionary<string, object> _referenceIdToObjectMap;
    private readonly Dictionary<object, string> _objectToReferenceIdMap;

    public PreserveReferenceResolver()
    {
        _objectToReferenceIdMap = new Dictionary<object, string>(ReferenceEqualityComparer.Instance);
        _referenceIdToObjectMap = new Dictionary<string, object>();
    }

    public override void AddReference(string referenceId, object value)
    {
        if (!_referenceIdToObjectMap.TryAdd(referenceId, value))
        {
            throw new JsonException($"Duplicate ID {referenceId} found");
        }
    }

    public override string GetReference(object value, out bool alreadyExists)
    {
        if (_objectToReferenceIdMap.TryGetValue(value, out string? referenceId))
        {
            alreadyExists = true;
        }
        else
        {
            _referenceCount++;
            referenceId = _referenceCount.ToString(CultureInfo.InvariantCulture);
            _objectToReferenceIdMap.Add(value, referenceId);
            alreadyExists = false;
        }

        return referenceId;
    }

    public override object ResolveReference(string referenceId)
    {
        if (!_referenceIdToObjectMap.TryGetValue(referenceId, out object? value))
        {
            throw new JsonException($"Object with ID {referenceId} not found");
        }

        return value;
    }
}
