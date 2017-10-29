namespace HyperStoreServiceAPP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingentitytype : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SupplierOrders", "EntityType", c => c.Int(nullable: false));
            AddColumn("dbo.Suppliers", "EntityType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Suppliers", "EntityType");
            DropColumn("dbo.SupplierOrders", "EntityType");
        }
    }
}
