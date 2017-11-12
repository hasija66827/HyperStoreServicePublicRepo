namespace HyperStoreServiceAPP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatingrpTopp : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RecommendedProducts", "PersonId", "dbo.People");
            DropForeignKey("dbo.RecommendedProducts", "ProductId", "dbo.Products");
            DropIndex("dbo.RecommendedProducts", new[] { "PersonId" });
            DropIndex("dbo.RecommendedProducts", new[] { "ProductId" });
            CreateTable(
                "dbo.PurchaseHistories",
                c => new
                    {
                        PurchaseHistoryId = c.Guid(nullable: false),
                        LatestPurchaseDate = c.DateTime(nullable: false),
                        ExpiryDays = c.Int(),
                        PersonId = c.Guid(nullable: false),
                        ProductId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.PurchaseHistoryId)
                .ForeignKey("dbo.People", t => t.PersonId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => new { t.PersonId, t.ProductId }, unique: true, name: "IX_PersonIdAndProductId");
            
            DropTable("dbo.RecommendedProducts");
        }
        
        public override void Down()
        {
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
                .PrimaryKey(t => t.RecommendedProductId);
            
            DropForeignKey("dbo.PurchaseHistories", "ProductId", "dbo.Products");
            DropForeignKey("dbo.PurchaseHistories", "PersonId", "dbo.People");
            DropIndex("dbo.PurchaseHistories", "IX_PersonIdAndProductId");
            DropTable("dbo.PurchaseHistories");
            CreateIndex("dbo.RecommendedProducts", "ProductId");
            CreateIndex("dbo.RecommendedProducts", "PersonId");
            AddForeignKey("dbo.RecommendedProducts", "ProductId", "dbo.Products", "ProductId", cascadeDelete: true);
            AddForeignKey("dbo.RecommendedProducts", "PersonId", "dbo.People", "PersonId", cascadeDelete: true);
        }
    }
}
