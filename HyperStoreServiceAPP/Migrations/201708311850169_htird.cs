namespace HyperStoreServiceAPP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class htird : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomerOrderProducts", "SellinPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.CustomerOrderProducts", "DiscountPerSnapShot", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.CustomerOrderProducts", "CGSTPerSnapShot", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.CustomerOrderProducts", "SGSTPerSnapshot", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.CustomerOrderProducts", "QuantityConsumed", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.CustomerOrderProducts", "QuantityConsumed", c => c.Single(nullable: false));
            AlterColumn("dbo.CustomerOrderProducts", "SGSTPerSnapshot", c => c.Single(nullable: false));
            AlterColumn("dbo.CustomerOrderProducts", "CGSTPerSnapShot", c => c.Single(nullable: false));
            AlterColumn("dbo.CustomerOrderProducts", "DiscountPerSnapShot", c => c.Single(nullable: false));
            DropColumn("dbo.CustomerOrderProducts", "SellinPrice");
        }
    }
}
