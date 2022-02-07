namespace Nodexr.Tests;
using System.Text.Json.Serialization;

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
        //Debug.Assert(_referenceIdToObjectMap != null);

        if (!_referenceIdToObjectMap.TryAdd(referenceId, value))
        {
            //ThrowHelper.ThrowJsonException_MetadataDuplicateIdFound(referenceId);
            throw new Exception();
        }
    }

    public override string GetReference(object value, out bool alreadyExists)
    {
        if (_objectToReferenceIdMap.TryGetValue(value, out string referenceId))
        {
            alreadyExists = true;
        }
        else
        {
            _referenceCount++;
            referenceId = _referenceCount.ToString();
            _objectToReferenceIdMap.Add(value, referenceId);
            alreadyExists = false;
        }

        return referenceId;
    }

    public override object ResolveReference(string referenceId)
    {
        if (!_referenceIdToObjectMap.TryGetValue(referenceId, out object value))
        {
            throw new Exception($"Object with ID {referenceId} not found");
        }

        return value;
    }
}

//public class NodeReferenceResolver : ReferenceResolver
//{
//    private readonly IDictionary<string, INodeViewModel> _people = new();

//    public override object ResolveReference(string referenceId)
//    {
//        _people.TryGetValue(referenceId, out var node);

//        return node;
//    }

//    public override string GetReference(object value, out bool alreadyExists)
//    {
//        INodeViewModel p = (INodeViewModel)value;

//        if (!(alreadyExists = _people.ContainsKey(p.Id)))
//        {
//            _people[p.Id] = p;
//        }

//        return p.Id.ToString();
//    }

//    public override void AddReference(string reference, object value)
//    {
//        INodeViewModel node = (INodeViewModel)value;
//        node.Id = id;
//        _people[id] = node;
//    }

//    private static string GetId(INodeViewModel node) => $"{node.Pos.x},{node.Pos.y},{node.Title}";
//}
