namespace HyperStoreServiceAPP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingfirstvisitedInPerson : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.People", "FirstVisited", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.People", "FirstVisited");
        }
    }
}
