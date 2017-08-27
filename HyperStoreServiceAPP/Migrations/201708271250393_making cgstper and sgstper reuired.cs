namespace HyperStoreServiceAPP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class makingcgstperandsgstperreuired : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Products", "CGSTPer", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Products", "SGSTPer", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Products", "SGSTPer", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Products", "CGSTPer", c => c.Decimal(precision: 18, scale: 2));
        }
    }
}
