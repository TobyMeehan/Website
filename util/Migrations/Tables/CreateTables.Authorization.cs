namespace Migrations.Tables;

public partial class CreateTables
{
    protected void UpAuthorizations()
    {
        Create.Table("authorizations")

            .WithColumn("Id")
            .AsString(Size.Id)
            .PrimaryKey()

            .WithColumn("ApplicationId")
            .AsString(Size.Id)
            .ForeignKey("applications", "Id")
            .Indexed()

            .WithColumn("UserId")
            .AsString(Size.Id)
            .ForeignKey("users", "Id")
            .Indexed()

            .WithColumn("Status")
            .AsString(Size.Short)
            .Nullable()

            .WithColumn("Type")
            .AsString(Size.Short)
            .Nullable()

            .WithColumn("CreatedAt")
            .AsDateTime()

            .WithColumn("Scopes")
            .AsString(Size.Long);
    }

    protected void DownAuthorizations()
    {
        Delete.Table("authorizations");
    }
}