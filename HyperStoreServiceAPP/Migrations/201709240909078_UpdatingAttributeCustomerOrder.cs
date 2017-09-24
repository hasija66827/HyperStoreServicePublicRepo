namespace HyperStoreServiceAPP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatingAttributeCustomerOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomerOrderProducts", "MRPSnapShot", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.CustomerOrderProducts", "ValueIncTaxSnapShot", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.CustomerOrderProducts", "DisplayCostSnapShot");
            DropColumn("dbo.CustomerOrderProducts", "SellingPrice");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CustomerOrderProducts", "SellingPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.CustomerOrderProducts", "DisplayCostSnapShot", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.CustomerOrderProducts", "ValueIncTaxSnapShot");
            DropColumn("dbo.CustomerOrderProducts", "MRPSnapShot");
        }
    }
}
