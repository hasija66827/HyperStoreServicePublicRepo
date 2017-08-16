namespace HyperStoreServiceAPP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Fifth : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.SupplierOrders", "SupplierOrderNo", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SupplierOrders", "SupplierOrderNo", c => c.String());
        }
    }
}
