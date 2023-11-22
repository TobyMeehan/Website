namespace Migrations.Tables;

public partial class CreateTables
{
    protected void UpTokens()
    {
        Create.Table("tokens")

            .WithColumn("Id")
            .AsString(Size.Id)
            .PrimaryKey()

            .WithColumn("AuthorizationId")
            .AsString(Size.Id)
            .ForeignKey("authorizations", "Id")

            .WithColumn("ReferenceId")
            .AsString(Size.Id)
            .Indexed()
            .Nullable()

            .WithColumn("Payload")
            .AsString(Size.Long)
            .Nullable()

            .WithColumn("Status")
            .AsString(Size.Short)
            .Nullable()

            .WithColumn("Type")
            .AsString(Size.Short)
            .Nullable()

            .WithColumn("Subject")
            .AsString(Size.Id)
            .Nullable()

            .WithColumn("RedeemedAt")
            .AsDateTime()
            .Nullable()

            .WithColumn("ExpiresAt")
            .AsDateTime()
            .Nullable()

            .WithColumn("CreatedAt")
            .AsDateTime()
            .Nullable();
    }

    protected void DownTokens()
    {
        Delete.Table("tokens");
    }
}