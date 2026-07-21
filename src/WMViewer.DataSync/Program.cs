using System.Data;
using System.Globalization;
using System.Text.Json;
using CsvHelper.Configuration;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using Shared.Model;
using WMViewer.DataSync.Migrations;
using WMViewer.DataSync.Reader;
using WMViewer.DataSync.Repository;
using WMViewer.DataSync.Validation;
using CsvReader = CsvHelper.CsvReader;

namespace WMViewer.DataSync;

internal abstract class Program
{
    private static void Main()
    {
        var builder = Host.CreateApplicationBuilder();
        var connection = new NpgsqlConnection(builder.Configuration["connection__string"]);
        
        
        builder.Services.AddSingleton<TeamRepository>();
        builder.Services.AddSingleton<IDbConnection>(connection);
        builder.Services.AddSingleton<PlayerRepository>();
        builder.Services.AddSingleton<Validator>();
        builder.Services.AddFluentMigratorCore().ConfigureRunner(r =>
        {
            r.AddPostgres();
            r.WithGlobalConnectionString(builder.Configuration["connection__string"]);
            r.ScanIn(typeof(Table_20260721).Assembly).For.All();
        }).AddLogging(l => l.AddFluentMigratorConsole());
        
        var csvPath = builder.Configuration["DataSync:CsvPath"];
        if (csvPath.IsNullOrEmpty())
        {
            throw new ArgumentNullException();
        }
        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            scope.ServiceProvider.GetRequiredService<IMigrationRunner>().MigrateUp();
        }
        var csvHelperConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            PrepareHeaderForMatch = args => args.Header.ToLower()
        };
        var validator = app.Services.GetService<Validator>();
        var playerRepository = app.Services.GetService<PlayerRepository>();
        if (validator == null || playerRepository == null)
        {
            throw new NullReferenceException("Unable to get classes class");

        }
        
        RunSync(csvPath, csvHelperConfig, validator, playerRepository);

    }

    private static void RunSync(string? csvPath, CsvConfiguration csvHelperConfig, Validator validator, PlayerRepository playerRepository)
    {
        using var reader = new StreamReader(csvPath!);
        using var csv = new CsvReader(reader, csvHelperConfig);
        
        using var records = csv.GetRecords<PlayerMap>().GetEnumerator();
        while (records.MoveNext())
        {
            var player = validator.CreatePlayer(records.Current);
            if (player == null)
            {
                break;
            }
            playerRepository.AddPlayer(player);
        }
    }
}