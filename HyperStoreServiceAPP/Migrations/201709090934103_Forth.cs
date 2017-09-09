namespace HyperStoreServiceAPP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Forth : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SupplierOrders", "PayedAmountByWallet", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.SupplierOrders", "SettledPayedAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.SupplierOrders", "PayedAmountIncTx");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SupplierOrders", "PayedAmountIncTx", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.SupplierOrders", "SettledPayedAmount");
            DropColumn("dbo.SupplierOrders", "PayedAmountByWallet");
        }
    }
}
