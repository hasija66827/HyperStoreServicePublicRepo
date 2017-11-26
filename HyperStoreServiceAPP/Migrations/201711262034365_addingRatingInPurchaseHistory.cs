namespace HyperStoreServiceAPP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingRatingInPurchaseHistory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PurchaseHistories", "Rating", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PurchaseHistories", "Rating");
        }
    }
}
