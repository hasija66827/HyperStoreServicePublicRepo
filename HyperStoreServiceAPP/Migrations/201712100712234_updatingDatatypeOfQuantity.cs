namespace HyperStoreServiceAPP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatingDatatypeOfQuantity : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Products", "TotalQuantity", c => c.Single(nullable: false));
            AlterColumn("dbo.DeficientStockHits", "QuantitySnapshot", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DeficientStockHits", "QuantitySnapshot", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Products", "TotalQuantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
