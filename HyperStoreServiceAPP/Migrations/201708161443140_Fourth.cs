namespace HyperStoreServiceAPP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Fourth : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomerOrderProducts", "QuantityConsumed", c => c.Single(nullable: false));
            AlterColumn("dbo.CustomerOrderProducts", "DisplayCostSnapShot", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.CustomerOrders", "CustomerOrderNo", c => c.String(nullable: false));
            AlterColumn("dbo.SupplierOrderProducts", "QuantityPurchased", c => c.Single(nullable: false));
            AlterColumn("dbo.SupplierOrderProducts", "PurchasePrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.SupplierOrders", "SupplierOrderNo", c => c.String());
            AlterColumn("dbo.SupplierOrders", "BillAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.SupplierOrders", "PaidAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.CustomerOrderProducts", "QuantityPurchased");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CustomerOrderProducts", "QuantityPurchased", c => c.Single(nullable: false));
            AlterColumn("dbo.SupplierOrders", "PaidAmount", c => c.Single(nullable: false));
            AlterColumn("dbo.SupplierOrders", "BillAmount", c => c.Single(nullable: false));
            AlterColumn("dbo.SupplierOrders", "SupplierOrderNo", c => c.String(nullable: false));
            AlterColumn("dbo.SupplierOrderProducts", "PurchasePrice", c => c.Single(nullable: false));
            AlterColumn("dbo.SupplierOrderProducts", "QuantityPurchased", c => c.Int(nullable: false));
            AlterColumn("dbo.CustomerOrders", "CustomerOrderNo", c => c.String());
            AlterColumn("dbo.CustomerOrderProducts", "DisplayCostSnapShot", c => c.Single(nullable: false));
            DropColumn("dbo.CustomerOrderProducts", "QuantityConsumed");
        }
    }
}
