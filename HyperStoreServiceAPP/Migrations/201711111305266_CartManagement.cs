namespace HyperStoreServiceAPP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CartManagement : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Products", "PotentielSupplierId", "dbo.People");
            DropIndex("dbo.Products", new[] { "PotentielSupplierId" });
            CreateTable(
                "dbo.CartProducts",
                c => new
                    {
                        CartProductId = c.Guid(nullable: false),
                        PotentielQuantityPurhcased = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PotentielPurchasePrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CartId = c.Guid(nullable: false),
                        ProductId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.CartProductId)
                .ForeignKey("dbo.Carts", t => t.CartId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: false)
                .Index(t => t.CartId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.Carts",
                c => new
                    {
                        CartId = c.Guid(nullable: false),
                        CartStatus = c.Int(nullable: false),
                        OrderCompletionDate = c.DateTime(),
                        PreferedDeliveryTime = c.DateTime(),
                        PersonId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.CartId)
                .ForeignKey("dbo.People", t => t.PersonId, cascadeDelete: false)
                .Index(t => t.PersonId);
            
            CreateTable(
                "dbo.RecommendedProducts",
                c => new
                    {
                        RecommendedProductId = c.Guid(nullable: false),
                        LatestPurchaseDate = c.DateTime(nullable: false),
                        ExpiryDays = c.Int(),
                        PersonId = c.Guid(nullable: false),
                        ProductId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.RecommendedProductId)
                .ForeignKey("dbo.People", t => t.PersonId, cascadeDelete: false)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: false)
                .Index(t => t.PersonId)
                .Index(t => t.ProductId);
            
            AddColumn("dbo.People", "PreferedTimeToContact", c => c.DateTime(nullable: false));
            DropColumn("dbo.Products", "PotentielSupplierId");
            DropColumn("dbo.Products", "PotentielQuantityPurhcased");
            DropColumn("dbo.Products", "PotentielPurchasePrice");
            DropColumn("dbo.Products", "IsPurchased");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Products", "IsPurchased", c => c.Boolean());
            AddColumn("dbo.Products", "PotentielPurchasePrice", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Products", "PotentielQuantityPurhcased", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Products", "PotentielSupplierId", c => c.Guid());
            DropForeignKey("dbo.RecommendedProducts", "ProductId", "dbo.Products");
            DropForeignKey("dbo.RecommendedProducts", "PersonId", "dbo.People");
            DropForeignKey("dbo.CartProducts", "ProductId", "dbo.Products");
            DropForeignKey("dbo.CartProducts", "CartId", "dbo.Carts");
            DropForeignKey("dbo.Carts", "PersonId", "dbo.People");
            DropIndex("dbo.RecommendedProducts", new[] { "ProductId" });
            DropIndex("dbo.RecommendedProducts", new[] { "PersonId" });
            DropIndex("dbo.Carts", new[] { "PersonId" });
            DropIndex("dbo.CartProducts", new[] { "ProductId" });
            DropIndex("dbo.CartProducts", new[] { "CartId" });
            DropColumn("dbo.People", "PreferedTimeToContact");
            DropTable("dbo.RecommendedProducts");
            DropTable("dbo.Carts");
            DropTable("dbo.CartProducts");
            CreateIndex("dbo.Products", "PotentielSupplierId");
            AddForeignKey("dbo.Products", "PotentielSupplierId", "dbo.People", "PersonId");
        }
    }
}
