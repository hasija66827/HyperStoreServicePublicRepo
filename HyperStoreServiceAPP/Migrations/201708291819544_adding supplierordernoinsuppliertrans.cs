namespace HyperStoreServiceAPP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingsupplierordernoinsuppliertrans : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SupplierTransactions", "SupplierOrderNo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SupplierTransactions", "SupplierOrderNo");
        }
    }
}
