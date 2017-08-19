namespace HyperStoreServiceAPP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Ninth : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomerOrderProducts", "CGSTPerSnapShot", c => c.Single(nullable: false));
            AddColumn("dbo.CustomerOrderProducts", "SGSTPerSnapshot", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CustomerOrderProducts", "SGSTPerSnapshot");
            DropColumn("dbo.CustomerOrderProducts", "CGSTPerSnapShot");
        }
    }
}
