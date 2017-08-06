namespace HyperStoreServiceAPP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        ProductId = c.Guid(nullable: false),
                        Name = c.String(),
                        BarCode = c.String(),
                        UserDefinedCode = c.String(),
                        IsInventoryItem = c.Boolean(nullable: false),
                        Threshold = c.Int(nullable: false),
                        RefillTime = c.Int(nullable: false),
                        DisplayPrice = c.Single(nullable: false),
                        DiscountPer = c.Single(nullable: false),
                        TotalQuantity = c.Int(nullable: false),
                        SGSTPer = c.Single(nullable: false),
                        CGSTPer = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.ProductId);
            
            CreateTable(
                "dbo.CustomerOrderProducts",
                c => new
                    {
                        CustomerOrderProductId = c.Guid(nullable: false),
                        DiscountPerSnapShot = c.Single(nullable: false),
                        DisplayCostSnapShot = c.Single(nullable: false),
                        QuantityPurchased = c.Int(nullable: false),
                        Product_ProductId = c.Guid(),
                    })
                .PrimaryKey(t => t.CustomerOrderProductId)
                .ForeignKey("dbo.Products", t => t.Product_ProductId)
                .Index(t => t.Product_ProductId);
            
            CreateTable(
                "dbo.ProductTags",
                c => new
                    {
                        ProductTagId = c.Guid(nullable: false),
                        ProductId = c.Guid(nullable: false),
                        TagId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.ProductTagId)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.SupplierOrderProducts",
                c => new
                    {
                        SupplierOrderProductId = c.Guid(nullable: false),
                        QuantityPurchased = c.Int(nullable: false),
                        PurchasePrice = c.Single(nullable: false),
                        Product_ProductId = c.Guid(),
                    })
                .PrimaryKey(t => t.SupplierOrderProductId)
                .ForeignKey("dbo.Products", t => t.Product_ProductId)
                .Index(t => t.Product_ProductId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SupplierOrderProducts", "Product_ProductId", "dbo.Products");
            DropForeignKey("dbo.ProductTags", "ProductId", "dbo.Products");
            DropForeignKey("dbo.CustomerOrderProducts", "Product_ProductId", "dbo.Products");
            DropIndex("dbo.SupplierOrderProducts", new[] { "Product_ProductId" });
            DropIndex("dbo.ProductTags", new[] { "ProductId" });
            DropIndex("dbo.CustomerOrderProducts", new[] { "Product_ProductId" });
            DropTable("dbo.SupplierOrderProducts");
            DropTable("dbo.ProductTags");
            DropTable("dbo.CustomerOrderProducts");
            DropTable("dbo.Products");
        }
    }
}
