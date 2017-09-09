namespace HyperStoreServiceAPP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatingdatatypeofSOP : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.SupplierOrderProducts", "QuantityPurchased", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SupplierOrderProducts", "QuantityPurchased", c => c.Single(nullable: false));
        }
    }
}
