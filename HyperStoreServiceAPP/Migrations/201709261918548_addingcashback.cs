namespace HyperStoreServiceAPP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingcashback : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomerTransactions", "IsCashbackTransaction", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CustomerTransactions", "IsCashbackTransaction");
        }
    }
}
