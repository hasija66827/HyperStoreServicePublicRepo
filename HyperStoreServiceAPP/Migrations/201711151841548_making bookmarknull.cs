namespace HyperStoreServiceAPP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class makingbookmarknull : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Carts", "IsBookmarked", c => c.Boolean());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Carts", "IsBookmarked", c => c.Boolean(nullable: false));
        }
    }
}
