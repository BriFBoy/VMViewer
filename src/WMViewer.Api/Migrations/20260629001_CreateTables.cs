using FluentMigrator;

namespace VMViewer.Migrations;

[Migration(20260629001, description: "Create Players and Teams tables")]
public class CreateTables : Migration
{
  public override void Up()
  {
    Create.Table("players")
      .WithColumn("playerid").AsInt32().NotNullable().PrimaryKey().Identity()
      .WithColumn("name").AsString(100)
      .WithColumn("age").AsInt32()
      .WithColumn("teamid").AsInt32();


    Create.Table("teams")
      .WithColumn("teamid").AsInt32().NotNullable().PrimaryKey().Identity()
      .WithColumn("name").AsString(100);

    Create.ForeignKey("FK_Team").FromTable("players").ForeignColumn("teamid").ToTable("teams").PrimaryColumn("teamid");

  }

  public override void Down()
  {
    Delete.Table("Players");
    Delete.Table("Teams");
  }
}
