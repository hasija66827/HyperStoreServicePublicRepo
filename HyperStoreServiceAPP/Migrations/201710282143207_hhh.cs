namespace HyperStoreServiceAPP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hhh : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomerOrders", "BillAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.CustomerOrders", "DueDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.CustomerOrders", "SettledPayedAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.CustomerOrders", "PayedAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.CustomerOrders", "InterestRate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.CustomerOrders", "PayAmount");
            DropColumn("dbo.CustomerOrders", "IsPayingNow");
            DropColumn("dbo.CustomerOrders", "IsUsingWallet");
            DropColumn("dbo.CustomerOrders", "PayingAmount");
            DropColumn("dbo.CustomerOrders", "UsingWalletAmount");
            DropColumn("dbo.SupplierOrders", "PayedAmountByWallet");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SupplierOrders", "PayedAmountByWallet", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.CustomerOrders", "UsingWalletAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.CustomerOrders", "PayingAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.CustomerOrders", "IsUsingWallet", c => c.Boolean(nullable: false));
            AddColumn("dbo.CustomerOrders", "IsPayingNow", c => c.Boolean(nullable: false));
            AddColumn("dbo.CustomerOrders", "PayAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.CustomerOrders", "InterestRate");
            DropColumn("dbo.CustomerOrders", "PayedAmount");
            DropColumn("dbo.CustomerOrders", "SettledPayedAmount");
            DropColumn("dbo.CustomerOrders", "DueDate");
            DropColumn("dbo.CustomerOrders", "BillAmount");
        }
    }
}
