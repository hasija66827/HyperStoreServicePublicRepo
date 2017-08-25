namespace HyperStoreServiceAPP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Thirteen : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Products", "CGSTPer", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Products", "DiscountPer", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Products", "SGSTPer", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Products", "Threshold", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Products", "TotalQuantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Products", "TotalQuantity", c => c.Single(nullable: false));
            AlterColumn("dbo.Products", "Threshold", c => c.Int(nullable: false));
            AlterColumn("dbo.Products", "SGSTPer", c => c.Single());
            AlterColumn("dbo.Products", "DiscountPer", c => c.Single(nullable: false));
            AlterColumn("dbo.Products", "CGSTPer", c => c.Single());
        }
    }
}
