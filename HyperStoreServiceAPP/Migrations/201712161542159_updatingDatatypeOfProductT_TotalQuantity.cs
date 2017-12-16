namespace HyperStoreServiceAPP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatingDatatypeOfProductT_TotalQuantity : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Products", "TotalQuantity", c => c.Single());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Products", "TotalQuantity", c => c.Single(nullable: false));
        }
    }
}
