using FluentMigrator;

namespace WMViewer.DataSync.Migrations;
[Migration(20260721001, description:"Creating table for sync checkmarks")]
public class Table: Migration
{
    public override void Up()
    {
        Create.Table("datasync")
            .WithColumn("datasync_id").AsInt32().PrimaryKey().Identity()
            .WithColumn("csv_name").AsString(100)
            .WithColumn("date_of_sync").AsDateTimeOffset().NotNullable().WithDefaultValue(SystemMethods.CurrentDateTime)
            .WithColumn("last_update").AsDateTimeOffset().NotNullable()
            .WithColumn("status").AsString(20)
            .WithColumn("error_message").AsString(400).Nullable();
    }

    public override void Down()
    {
        Delete.Table("datasync");
    }
}