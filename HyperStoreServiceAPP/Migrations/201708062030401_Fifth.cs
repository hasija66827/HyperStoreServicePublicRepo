namespace HyperStoreServiceAPP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Fifth : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomerOrders", "IsPayingNow", c => c.Boolean(nullable: false));
            AddColumn("dbo.CustomerOrders", "IsUsingWallet", c => c.Boolean(nullable: false));
            AddColumn("dbo.CustomerOrders", "PayingAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.CustomerOrderProducts", "QuantityPurchased", c => c.Single(nullable: false));
            AlterColumn("dbo.CustomerOrders", "CustomerOrderNo", c => c.String(nullable: false));
            AlterColumn("dbo.CustomerOrders", "BillAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.CustomerOrders", "DiscountedAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.CustomerOrders", "UsingWalletAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Customers", "WalletBalance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.CustomerOrders", "IsPaidNow");
            DropColumn("dbo.CustomerOrders", "PayingNow");
            DropColumn("dbo.CustomerOrders", "AddingMoneyToWallet");
            DropColumn("dbo.CustomerOrders", "IsUseWallet");
            DropColumn("dbo.CustomerOrders", "PartiallyPaid");
            DropColumn("dbo.CustomerOrders", "PayingLater");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CustomerOrders", "PayingLater", c => c.Single(nullable: false));
            AddColumn("dbo.CustomerOrders", "PartiallyPaid", c => c.Single(nullable: false));
            AddColumn("dbo.CustomerOrders", "IsUseWallet", c => c.Boolean(nullable: false));
            AddColumn("dbo.CustomerOrders", "AddingMoneyToWallet", c => c.Single(nullable: false));
            AddColumn("dbo.CustomerOrders", "PayingNow", c => c.Single(nullable: false));
            AddColumn("dbo.CustomerOrders", "IsPaidNow", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Customers", "WalletBalance", c => c.Single(nullable: false));
            AlterColumn("dbo.CustomerOrders", "UsingWalletAmount", c => c.Single(nullable: false));
            AlterColumn("dbo.CustomerOrders", "DiscountedAmount", c => c.Single(nullable: false));
            AlterColumn("dbo.CustomerOrders", "BillAmount", c => c.Single(nullable: false));
            AlterColumn("dbo.CustomerOrders", "CustomerOrderNo", c => c.String());
            AlterColumn("dbo.CustomerOrderProducts", "QuantityPurchased", c => c.Int(nullable: false));
            DropColumn("dbo.CustomerOrders", "PayingAmount");
            DropColumn("dbo.CustomerOrders", "IsUsingWallet");
            DropColumn("dbo.CustomerOrders", "IsPayingNow");
        }
    }
}
