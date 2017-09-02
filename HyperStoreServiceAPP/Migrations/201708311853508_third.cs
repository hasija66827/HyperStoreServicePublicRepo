namespace HyperStoreServiceAPP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class third : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomerOrderProducts", "SellingPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.CustomerOrderProducts", "SellinPrice");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CustomerOrderProducts", "SellinPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.CustomerOrderProducts", "SellingPrice");
        }
    }
}
