namespace HyperStoreServiceAPP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatingdataytpeofhsn : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Products", "HSN", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Products", "HSN", c => c.Int(nullable: false));
        }
    }
}
