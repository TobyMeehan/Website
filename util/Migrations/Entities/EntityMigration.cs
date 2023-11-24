using FluentMigrator;

namespace Migrations.Entities;

public class EntityMigration<T> : Migration
{
    public string Table { get; }
    public T Model { get; }

    public EntityMigration(string table, T model)
    {
        Table = table;
        Model = model;
    }
    
    public override void Up()
    {
        Insert.IntoTable(Table).Row(Model);
    }

    public override void Down()
    {
        Delete.FromTable(Table).Row(Model);
    }
}