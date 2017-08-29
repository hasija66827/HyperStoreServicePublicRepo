namespace HyperStoreServiceAPP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingcustomerordertransactionEntity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CustomerOrderTransactions",
                c => new
                    {
                        CustomerOrderTransactionId = c.Guid(nullable: false),
                        TransactionId = c.Guid(nullable: false),
                        CustomerOrderId = c.Guid(nullable: false),
                        CustomerTransaction_CustomerTransactionId = c.Guid(),
                    })
                .PrimaryKey(t => t.CustomerOrderTransactionId)
                .ForeignKey("dbo.CustomerOrders", t => t.CustomerOrderId, cascadeDelete: true)
                .ForeignKey("dbo.CustomerTransactions", t => t.CustomerTransaction_CustomerTransactionId)
                .Index(t => t.CustomerOrderId)
                .Index(t => t.CustomerTransaction_CustomerTransactionId);
            
            AddColumn("dbo.CustomerTransactions", "IsCredit", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CustomerOrderTransactions", "CustomerTransaction_CustomerTransactionId", "dbo.CustomerTransactions");
            DropForeignKey("dbo.CustomerOrderTransactions", "CustomerOrderId", "dbo.CustomerOrders");
            DropIndex("dbo.CustomerOrderTransactions", new[] { "CustomerTransaction_CustomerTransactionId" });
            DropIndex("dbo.CustomerOrderTransactions", new[] { "CustomerOrderId" });
            DropColumn("dbo.CustomerTransactions", "IsCredit");
            DropTable("dbo.CustomerOrderTransactions");
        }
    }
}
