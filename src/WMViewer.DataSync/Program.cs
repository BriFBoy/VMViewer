using System.Data;
using System.Globalization;
using CsvHelper.Configuration;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using WMViewer.DataSync.Reader;
using WMViewer.DataSync.Repository;
using WMViewer.DataSync.Runner;
using WMViewer.DataSync.Validation;
using CsvReader = CsvHelper.CsvReader;

namespace WMViewer.DataSync;

internal abstract class Program
{
    private static void Main()
    {
        try
        {
            var builder = Host.CreateApplicationBuilder();
            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__wmviewer");
            if (connectionString.IsNullOrEmpty())
            {
                throw new ArgumentNullException();
            }

            var csvHelperConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                PrepareHeaderForMatch = args => args.Header.ToLower()
            };

            var connection = new NpgsqlConnection(connectionString);
            builder.ConfigureOpenTelemetry();


            builder.Services.AddSingleton<TeamRepository>();
            builder.Services.AddSingleton<IDbConnection>(connection);
            builder.Services.AddSingleton<PlayerRepository>();
            builder.Services.AddSingleton<Validator>();
            builder.Services.AddSingleton<Processor>();
            builder.Services.AddSingleton<SyncRunner>();
            builder.Services.AddSingleton<DatasyncRepository>();
            builder.Services.AddSingleton(csvHelperConfig);


            var csvPath = builder.Configuration["DataSync:CsvPath"];
            if (csvPath.IsNullOrEmpty())
            {
                throw new ArgumentNullException();
            }

            using var app = builder.Build();

            var syncRunner = app.Services.GetService<SyncRunner>() ?? throw new ArgumentNullException();
            syncRunner.RunSync(csvPath);
            
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e);
            throw;
        }
    }
}