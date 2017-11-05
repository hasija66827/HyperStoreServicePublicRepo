namespace HyperStoreServiceAPP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cartManage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "LatestSupplierId", c => c.Guid());
            AddColumn("dbo.Products", "PotentielSupplierId", c => c.Guid());
            AddColumn("dbo.Products", "PotentielQuantityPurhcased", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Products", "PotentielPurchasePrice", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Products", "IsPurchased", c => c.Boolean());
            CreateIndex("dbo.Products", "LatestSupplierId");
            CreateIndex("dbo.Products", "PotentielSupplierId");
            AddForeignKey("dbo.Products", "LatestSupplierId", "dbo.People", "PersonId");
            AddForeignKey("dbo.Products", "PotentielSupplierId", "dbo.People", "PersonId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Products", "PotentielSupplierId", "dbo.People");
            DropForeignKey("dbo.Products", "LatestSupplierId", "dbo.People");
            DropIndex("dbo.Products", new[] { "PotentielSupplierId" });
            DropIndex("dbo.Products", new[] { "LatestSupplierId" });
            DropColumn("dbo.Products", "IsPurchased");
            DropColumn("dbo.Products", "PotentielPurchasePrice");
            DropColumn("dbo.Products", "PotentielQuantityPurhcased");
            DropColumn("dbo.Products", "PotentielSupplierId");
            DropColumn("dbo.Products", "LatestSupplierId");
        }
    }
}
