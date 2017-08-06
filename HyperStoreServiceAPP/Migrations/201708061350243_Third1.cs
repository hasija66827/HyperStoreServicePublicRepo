namespace HyperStoreServiceAPP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Third1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "Code", c => c.String(nullable: false));
            AlterColumn("dbo.Products", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.Suppliers", "MobileNo", c => c.String(nullable: false));
            AlterColumn("dbo.Suppliers", "Name", c => c.String(nullable: false));
            DropColumn("dbo.Products", "BarCode");
            DropColumn("dbo.Products", "UserDefinedCode");
            DropColumn("dbo.Products", "IsInventoryItem");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Products", "IsInventoryItem", c => c.Boolean(nullable: false));
            AddColumn("dbo.Products", "UserDefinedCode", c => c.String());
            AddColumn("dbo.Products", "BarCode", c => c.String());
            AlterColumn("dbo.Suppliers", "Name", c => c.String());
            AlterColumn("dbo.Suppliers", "MobileNo", c => c.String());
            AlterColumn("dbo.Products", "Name", c => c.String());
            DropColumn("dbo.Products", "Code");
        }
    }
}
