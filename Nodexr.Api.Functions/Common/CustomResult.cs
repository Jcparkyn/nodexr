namespace Nodexr.Api.Functions.Common;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

public class CustomResult
{
    /// <summary>
    /// Serializes the response object using <see cref="System.Text.Json"/>
    /// </summary>
    public static ActionResult Json<T>(T source, JsonSerializerOptions? options = null)
    {
        var result = new ContentResult
        {
            Content = JsonSerializer.Serialize(source, options),
            ContentType = "application/json; charset=utf-8",
            StatusCode = 200
        };
        return result;
    }
}
