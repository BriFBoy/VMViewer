using FluentMigrator.Runner;
using Npgsql;
using VMViewer.Migrations;
using VMViewer.Repository;
using VMViewer.Service;

namespace VMViewer;

public abstract class Program
{
  private static readonly string ConnectionString;

  static Program()
  {
    DotNetEnv.Env.Load();
    var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__postgres");
    if (connectionString == null)
    {
      Console.WriteLine("Connection String null");
      Environment.Exit(1);
    }
    Console.WriteLine(connectionString);
    ConnectionString = connectionString;

  }


  private static void Main(string[] args)
  {

    var builder = WebApplication.CreateBuilder(args);
    builder.AddServiceDefaults();
    builder.Services.AddOpenTelemetry();
    
    builder.Services.AddControllers();
    builder.Services.AddTransient(_ => new NpgsqlConnection(ConnectionString));
    builder.Services.AddSingleton<ISquadService, SquadService>();
    builder.Services.AddSingleton<IPlayerService, PlayerService>();
    builder.Services.AddSingleton<ITeamService, TeamService>();
    builder.Services.AddSingleton<ITeamRepository, TeamRepository>();
    builder.Services.AddSingleton<IPlayerRepository, PlayerRepository>();

    builder.Services.AddFluentMigratorCore().ConfigureRunner(rb => rb
      .AddPostgres()
      .WithGlobalConnectionString(ConnectionString)
      .ScanIn(typeof(CreateTables).Assembly)
      .For.All()).AddLogging(lb => lb.AddFluentMigratorConsole());


    var app = builder.Build();
    app.MapDefaultEndpoints();
    app.MapControllers();

    using (var runner = app.Services.CreateScope())
    {
      runner.ServiceProvider.GetRequiredService<IMigrationRunner>().MigrateUp();
    }


    app.Run();

  }
}