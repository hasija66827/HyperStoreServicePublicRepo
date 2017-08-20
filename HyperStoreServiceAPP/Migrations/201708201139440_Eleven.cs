namespace HyperStoreServiceAPP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Eleven : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Products", "SupplierId", "dbo.Suppliers");
            DropIndex("dbo.Products", new[] { "SupplierId" });
            DropColumn("dbo.Products", "SupplierId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Products", "SupplierId", c => c.Guid());
            CreateIndex("dbo.Products", "SupplierId");
            AddForeignKey("dbo.Products", "SupplierId", "dbo.Suppliers", "SupplierId");
        }
    }
}
