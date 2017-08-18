namespace HyperStoreServiceAPP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class eight : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.SupplierOrderTransactions", "PaidAmount", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SupplierOrderTransactions", "PaidAmount", c => c.Single(nullable: false));
        }
    }
}
