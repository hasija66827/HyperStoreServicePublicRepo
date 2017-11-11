namespace HyperStoreServiceAPP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatingcart : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Carts", "IsBookmarked", c => c.Boolean(nullable: false));
            AddColumn("dbo.People", "LastCalled", c => c.DateTime());
            AddColumn("dbo.People", "Rating", c => c.Int());
            AlterColumn("dbo.People", "PreferedTimeToContact", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.People", "PreferedTimeToContact", c => c.DateTime(nullable: false));
            DropColumn("dbo.People", "Rating");
            DropColumn("dbo.People", "LastCalled");
            DropColumn("dbo.Carts", "IsBookmarked");
        }
    }
}
