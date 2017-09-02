namespace HyperStoreServiceAPP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Fouth : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomerOrders", "TotalQuantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.CustomerOrders", "TotalItems", c => c.Int(nullable: false));
            AddColumn("dbo.CustomerOrders", "CartAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.CustomerOrders", "DiscountAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.CustomerOrders", "Tax", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.CustomerOrders", "PayAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.CustomerOrders", "BillAmount");
            DropColumn("dbo.CustomerOrders", "DiscountedAmount");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CustomerOrders", "DiscountedAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.CustomerOrders", "BillAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.CustomerOrders", "PayAmount");
            DropColumn("dbo.CustomerOrders", "Tax");
            DropColumn("dbo.CustomerOrders", "DiscountAmount");
            DropColumn("dbo.CustomerOrders", "CartAmount");
            DropColumn("dbo.CustomerOrders", "TotalItems");
            DropColumn("dbo.CustomerOrders", "TotalQuantity");
        }
    }
}
