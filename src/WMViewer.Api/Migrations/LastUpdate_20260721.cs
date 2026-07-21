using FluentMigrator;

namespace VMViewer.Migrations;

[Migration(20260721, description:"last update colum to know when to update player")]
public class LastUpdate_20260721: Migration
{
    public override void Up()
    {
        Alter.Table("player").AddColumn("last_update").AsDateTimeOffset().Nullable();
    }

    public override void Down()
    {
        Delete.Column("last_update").FromTable("player");
    }
}