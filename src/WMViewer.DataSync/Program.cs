using Microsoft.Extensions.Hosting;

namespace WMViewer.DataSync;

internal abstract class Program
{
    private static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder();
        builder.AddServiceDefaults();

    }
}