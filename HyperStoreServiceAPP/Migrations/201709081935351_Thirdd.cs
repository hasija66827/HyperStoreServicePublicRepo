namespace HyperStoreServiceAPP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Thirdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SupplierOrders", "PayedAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.SupplierOrders", "PayedAmountIncTx", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.SupplierOrders", "PayingAmount");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SupplierOrders", "PayingAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.SupplierOrders", "PayedAmountIncTx");
            DropColumn("dbo.SupplierOrders", "PayedAmount");
        }
    }
}
