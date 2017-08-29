namespace HyperStoreServiceAPP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addCustomeOrderNoInTrans : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomerTransactions", "CustomerOrderNo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CustomerTransactions", "CustomerOrderNo");
        }
    }
}
