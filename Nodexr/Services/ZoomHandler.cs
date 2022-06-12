namespace Nodexr.Services;
using Microsoft.JSInterop;
using System.Globalization;

public static class ZoomHandler
{
    public static double Zoom { get; set; } = 1d;

    public static string ZoomCSS => Zoom.ToString(CultureInfo.InvariantCulture);

    public static void ZoomBy(double factor)
    {
        Zoom *= factor;
    }

    [JSInvokable]
    public static void SetZoom(double zoom)
    {
        Zoom = zoom;
    }
}
