namespace HyperStoreServiceAPP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingEntityTypeInTransaction : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Transactions", "EntityType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Transactions", "EntityType");
        }
    }
}
