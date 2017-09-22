namespace HyperStoreServiceAPP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatingproductentity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "MRP", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Products", "HSN", c => c.Int(nullable: false));
            DropColumn("dbo.Products", "DisplayPrice");
            DropColumn("dbo.Products", "RefillTime");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Products", "RefillTime", c => c.Int(nullable: false));
            AddColumn("dbo.Products", "DisplayPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.Products", "HSN");
            DropColumn("dbo.Products", "MRP");
        }
    }
}
