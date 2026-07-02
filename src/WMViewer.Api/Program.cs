
using FluentMigrator.Runner;
using Npgsql;
using VMViewer.Migrations;
using VMViewer.Repository;
using VMViewer.Service;

public class Program
{
  private static readonly string ConnectionString;

  static Program()
  {
    DotNetEnv.Env.Load();
    var connection_string = Environment.GetEnvironmentVariable("CONNECTION_STRING");
    if (connection_string == null)
    {
      Console.WriteLine("Connection String null");
      Environment.Exit(1);
    }
    ConnectionString = connection_string;

  }



  static void Main(string[] args)
  {

    var builder = WebApplication.CreateBuilder(args);
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
    app.MapControllers();

    using (var runner = app.Services.CreateScope())
    {
      runner.ServiceProvider.GetRequiredService<IMigrationRunner>().MigrateUp();
    }


    app.Run();

  }
}
