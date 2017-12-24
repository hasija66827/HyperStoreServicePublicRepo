namespace HyperStoreServiceAPP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingpaymentoption : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PaymentOptions",
                c => new
                    {
                        PaymentOptionId = c.Guid(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.PaymentOptionId);
            
            AddColumn("dbo.Orders", "PaymentOptionId", c => c.Guid());
            AddColumn("dbo.Transactions", "PaymentOptionId", c => c.Guid());
            CreateIndex("dbo.Orders", "PaymentOptionId");
            CreateIndex("dbo.Transactions", "PaymentOptionId");
            AddForeignKey("dbo.Orders", "PaymentOptionId", "dbo.PaymentOptions", "PaymentOptionId");
            AddForeignKey("dbo.Transactions", "PaymentOptionId", "dbo.PaymentOptions", "PaymentOptionId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Transactions", "PaymentOptionId", "dbo.PaymentOptions");
            DropForeignKey("dbo.Orders", "PaymentOptionId", "dbo.PaymentOptions");
            DropIndex("dbo.Transactions", new[] { "PaymentOptionId" });
            DropIndex("dbo.Orders", new[] { "PaymentOptionId" });
            DropColumn("dbo.Transactions", "PaymentOptionId");
            DropColumn("dbo.Orders", "PaymentOptionId");
            DropTable("dbo.PaymentOptions");
        }
    }
}
