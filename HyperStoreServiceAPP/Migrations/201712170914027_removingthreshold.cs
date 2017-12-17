namespace HyperStoreServiceAPP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removingthreshold : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Products", "Threshold");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Products", "Threshold", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
