namespace Nodexr.Nodes;

using BlazorNodes.Core;
using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

public class RegexNodeJsonConverter : JsonConverter<INodeViewModel>
{
    private class SerializedNode
    {
        [JsonPropertyName("$ref")]
        public string? Ref { get; set; }
        [JsonPropertyName("$id")]
        public string? Id { get; set; }
        public string? NodeType { get; set; }
        public double PositionX { get; set; }
        public double PositionY { get; set; }
        public bool IsCollapsed { get; set; }
        public JsonObject? Inputs { get; set; }
    }

    //private static readonly Dictionary<string, Type> allowedNodeTypes = typeof(INodeViewModel).Assembly.;

    public override INodeViewModel? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var referenceResolver = options.ReferenceHandler?.CreateResolver()
            ?? throw new InvalidOperationException("A ReferenceHandler must be provided to deserialize this object");

        var props = JsonSerializer.Deserialize<SerializedNode>(ref reader, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        }) ?? throw new JsonException("Node should not be null");

        if (!string.IsNullOrEmpty(props.Ref))
        {
            return (INodeViewModel)referenceResolver.ResolveReference(props.Ref);
        }

        var newNodeType = Type.GetType(props.NodeType ?? throw new JsonException());
        if (newNodeType?.IsAssignableTo(typeof(INodeViewModel)) != true)
            return null;
        if (Activator.CreateInstance(newNodeType) is not INodeViewModel node)
            return null;

        node.Pos = new(props.PositionX, props.PositionY);
        node.IsCollapsed = props.IsCollapsed;

        var nodeType = node.GetType();
        if (props.Inputs is not null)
        {
            foreach (var (inputName, serializedInput) in props.Inputs)
            {
                var targetInput = nodeType.GetProperty(inputName)?.GetValue(node)
                    ?? throw new JsonException($"Input {inputName} doesn't exist");

                if (targetInput is IInputPort inputPort)
                {
                    var connectedNode = serializedInput.Deserialize<INodeViewModel>(options);
                    inputPort.TrySetConnectedNode(connectedNode);
                }
                else if (targetInput is INodeInput input)
                {
                    var valueProperty = input.GetType().GetProperty("Value");
                    if (valueProperty is null) continue;
                    var valueType = valueProperty.PropertyType;
                    input.TrySetValue(serializedInput?.Deserialize(valueType, options));
                }
            }
        }

        referenceResolver.AddReference(props.Id ?? throw new JsonException(), node);

        return node;
    }

    public override void Write(Utf8JsonWriter writer, INodeViewModel value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        var referenceResolver = options.ReferenceHandler?.CreateResolver()
            ?? throw new InvalidOperationException("A ReferenceHandler must be provided to deserialize this object");
        string nodeId = referenceResolver.GetReference(value, out bool alreadyExists);
        if (alreadyExists)
        {
            writer.WriteString("$ref", nodeId);
        }
        else
        {
            writer.WriteString("$id", nodeId);
            writer.WriteString("nodeType", value.GetType().AssemblyQualifiedName);
            writer.WriteNumber("positionX", value.Pos.x);
            writer.WriteNumber("positionY", value.Pos.y);
            writer.WriteBoolean("isCollapsed", value.IsCollapsed);
            WriteNodeInputs(writer, "inputs", value, options);
        }

        writer.WriteEndObject();
    }

    private static void WriteNodeInputs(Utf8JsonWriter writer, string propertyName, INodeViewModel node, JsonSerializerOptions options)
    {
        writer.WriteStartObject(propertyName);

        var properties = node.GetType().GetProperties();

        foreach (var input in node.NodeInputs.Append(node.PrimaryInput))
        {
            string inputId = properties.First(prop => ReferenceEquals(prop.GetValue(node), input)).Name;
            if (input is IInputPort port)
            {
                writer.WritePropertyName(inputId);
                JsonSerializer.Serialize(writer, port.ConnectedNodeUntyped as INodeViewModel, options);
            }
            else
            {
                var valueProperty = input.GetType()?.GetProperty("Value");

                var inputValue = valueProperty?.GetValue(input);
                if (inputValue is not null)
                {
                    writer.WritePropertyName(inputId);
                    JsonSerializer.Serialize(writer, inputValue, options);
                }
            }
        }
        writer.WriteEndObject();
    }
}
