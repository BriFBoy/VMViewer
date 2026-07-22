

using FluentMigrator;

namespace VMViewer.Migrations;

[Migration(20260703002,description: "Adds a Captain to Team/player")]
public class CaptainPlayer: Migration
{
    public override void Up()
    {
        Alter.Table("players").AddColumn("iscaptain").AsBoolean().NotNullable().WithDefaultValue(false);
        Alter.Table("teams").AddColumn("captain").AsInt32().Nullable();
        
        Create.ForeignKey("FK_CAPTAIN").FromTable("teams").ForeignColumn("captain").ToTable("players").PrimaryColumn("playerid");
    }

    public override void Down()
    {
        Delete.Column("iscaptain").FromTable("players");
        Delete.Column("captain").FromTable("teams");
        
        Delete.ForeignKey("FK_CAPTAIN");
    }
}