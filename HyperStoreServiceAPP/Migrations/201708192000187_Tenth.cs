namespace HyperStoreServiceAPP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Tenth : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomerOrderProducts", "NetValue", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Products", "DisplayPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Products", "DisplayPrice", c => c.Single(nullable: false));
            DropColumn("dbo.CustomerOrderProducts", "NetValue");
        }
    }
}
