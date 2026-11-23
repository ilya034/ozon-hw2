using FluentMigrator;
using ProductService.Domain.Products;

namespace ProductService.Infrastructure.Migrations;

[Migration(20261123173000, "Create product table")]
public class CreateProductTableMigration : Migration
{
    public override void Up()
    {
        Create.Table("Product")
            .WithColumn(nameof(Product.Id)).AsGuid().PrimaryKey()
            .WithColumn(nameof(Product.Name)).AsString(200).NotNullable()
            .WithColumn(nameof(Product.Price)).AsCustom("numeric").NotNullable()
            .WithColumn(nameof(Product.Weight)).AsDouble().NotNullable()
            .WithColumn(nameof(Product.Type)).AsInt32().NotNullable()
            .WithColumn(nameof(Product.CreatedAt)).AsDateTimeOffset().NotNullable()
            .WithColumn(nameof(Product.WarehouseId)).AsInt64().NotNullable();

        Create.Index("IX_Product_WarehouseId").OnTable("Product").OnColumn(nameof(Product.WarehouseId));
        Create.Index("IX_Product_Type").OnTable("Product").OnColumn(nameof(Product.Type));
        Create.Index("IX_Product_CreatedAt").OnTable("Product").OnColumn(nameof(Product.CreatedAt));
    }

    public override void Down()
    {
        if (Schema.Table("Product").Exists())
        {
            Delete.Table("Product");
        }
    }
}
