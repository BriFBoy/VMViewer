using FluentMigrator;

namespace VMViewer.Migrations;

[Migration(20260703001,description: "Add NumberOfPlayers to Table")]
public class AddTeamPlayerNumber: Migration {
    public override void Up()
    {
        Alter.Table("teams").AddColumn("numberofplayers").AsInt32().NotNullable().WithDefaultValue(0);
        Execute.Sql("UPDATE teams SET numberofplayers=(SELECT COUNT(*) FROM players WHERE players.teamid = teams.teamid)");
    }

    public override void Down()
    {
        Delete.Column("numberofplayers").FromTable("teams");
    }
}