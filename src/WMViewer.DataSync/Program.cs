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
                                  
            var connection = new NpgsqlConnection(connectionString);
            builder.ConfigureOpenTelemetry();


            builder.Services.AddSingleton<TeamRepository>();
            builder.Services.AddSingleton<IDbConnection>(connection);
            builder.Services.AddSingleton<PlayerRepository>();
            builder.Services.AddSingleton<Validator>();


            var csvPath = builder.Configuration["DataSync:CsvPath"];
            if (csvPath.IsNullOrEmpty())
            {
                throw new ArgumentNullException();
            }

            using var app = builder.Build();

            var csvHelperConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                PrepareHeaderForMatch = args => args.Header.ToLower()
            };
            var validator = app.Services.GetService<Validator>();
            var playerRepository = app.Services.GetService<PlayerRepository>();
            var teamRepository = app.Services.GetService<TeamRepository>();
            var syncConnection = app.Services.GetService<IDbConnection>();
            if (validator == null || playerRepository == null || teamRepository == null || syncConnection == null)
            {
                throw new NullReferenceException("Unable to get classes class");
            }

            RunSync(csvPath, csvHelperConfig, validator, playerRepository, teamRepository, syncConnection);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e);
            throw;
        }
    }

    private static void RunSync(string? csvPath, CsvConfiguration csvHelperConfig,
        Validator validator, PlayerRepository playerRepository,
        TeamRepository teamRepository, IDbConnection connection)
    {
        using var reader = new StreamReader(csvPath!);
        using var csv = new CsvReader(reader, csvHelperConfig);

        using var records = csv.GetRecords<PlayerMap>().GetEnumerator();
        Console.WriteLine("[datasync] opening connection");
        connection.Open();
        Console.WriteLine("[datasync] connection open, beginning transaction");
        using (var transaction = connection.BeginTransaction())
        {
            try
            {
                var rowCount = 0;
                while (records.MoveNext())
                {
                    Console.WriteLine($"[datasync] processing row {++rowCount}");
                    if (records.Current.TeamName.IsNullOrEmpty())
                    {
                        continue;
                    }

                    var (player, team) = validator.CreateValidPlayerAndTeam(records.Current);
                    if (player == null)
                    {
                        break;
                    }

                    if (team != null)
                    {
                        teamRepository.UpdateTeam(team);
                    }

                    playerRepository.AddPlayer(player);
                }

                Console.WriteLine("[datasync] committing transaction");
                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                Console.WriteLine(e);
                throw;
            }
        }

        connection.Close();
    }
}