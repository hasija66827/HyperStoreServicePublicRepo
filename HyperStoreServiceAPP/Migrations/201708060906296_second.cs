namespace HyperStoreServiceAPP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class second : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CustomerOrders",
                c => new
                    {
                        CustomerOrderId = c.Guid(nullable: false),
                        CustomerOrderNo = c.String(),
                        OrderDate = c.DateTime(nullable: false),
                        BillAmount = c.Single(nullable: false),
                        DiscountedAmount = c.Single(nullable: false),
                        IsPaidNow = c.Boolean(nullable: false),
                        PayingNow = c.Single(nullable: false),
                        AddingMoneyToWallet = c.Single(nullable: false),
                        IsUseWallet = c.Boolean(nullable: false),
                        UsingWalletAmount = c.Single(nullable: false),
                        PartiallyPaid = c.Single(nullable: false),
                        PayingLater = c.Single(nullable: false),
                        Customer_CustomerId = c.Guid(),
                    })
                .PrimaryKey(t => t.CustomerOrderId)
                .ForeignKey("dbo.Customers", t => t.Customer_CustomerId)
                .Index(t => t.Customer_CustomerId);
            
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        CustomerId = c.Guid(nullable: false),
                        Address = c.String(),
                        GSTIN = c.String(),
                        MobileNo = c.String(),
                        Name = c.String(),
                        WalletBalance = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.CustomerId);
            
            CreateTable(
                "dbo.SupplierOrders",
                c => new
                    {
                        SupplierOrderId = c.Guid(nullable: false),
                        SupplierOrderNo = c.String(),
                        OrderDate = c.DateTime(nullable: false),
                        DueDate = c.DateTime(nullable: false),
                        BillAmount = c.Single(nullable: false),
                        PaidAmount = c.Single(nullable: false),
                        Supplier_SupplierId = c.Guid(),
                    })
                .PrimaryKey(t => t.SupplierOrderId)
                .ForeignKey("dbo.Suppliers", t => t.Supplier_SupplierId)
                .Index(t => t.Supplier_SupplierId);
            
            CreateTable(
                "dbo.SupplierOrderTransactions",
                c => new
                    {
                        SupplierOrderTransactionId = c.Guid(nullable: false),
                        PaidAmount = c.Single(nullable: false),
                        IsPaymentComplete = c.Boolean(nullable: false),
                        SupplierOrder_SupplierOrderId = c.Guid(),
                        Transaction_TransactionId = c.Guid(),
                    })
                .PrimaryKey(t => t.SupplierOrderTransactionId)
                .ForeignKey("dbo.SupplierOrders", t => t.SupplierOrder_SupplierOrderId)
                .ForeignKey("dbo.Transactions", t => t.Transaction_TransactionId)
                .Index(t => t.SupplierOrder_SupplierOrderId)
                .Index(t => t.Transaction_TransactionId);
            
            CreateTable(
                "dbo.Suppliers",
                c => new
                    {
                        SupplierId = c.Guid(nullable: false),
                        Address = c.String(),
                        GSTIN = c.String(),
                        MobileNo = c.String(),
                        Name = c.String(),
                        WalletBalance = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.SupplierId);
            
            CreateTable(
                "dbo.Transactions",
                c => new
                    {
                        TransactionId = c.Guid(nullable: false),
                        TransactionNo = c.String(),
                        CreditAmount = c.Single(nullable: false),
                        TransactionDate = c.DateTime(nullable: false),
                        WalletSnapshot = c.Single(nullable: false),
                        Supplier_SupplierId = c.Guid(),
                    })
                .PrimaryKey(t => t.TransactionId)
                .ForeignKey("dbo.Suppliers", t => t.Supplier_SupplierId)
                .Index(t => t.Supplier_SupplierId);
            
            CreateTable(
                "dbo.Tags",
                c => new
                    {
                        TagId = c.Guid(nullable: false),
                        TagName = c.String(),
                    })
                .PrimaryKey(t => t.TagId);
            
            AddColumn("dbo.Products", "Supplier_SupplierId", c => c.Guid());
            AddColumn("dbo.CustomerOrderProducts", "CustomerOrder_CustomerOrderId", c => c.Guid());
            AddColumn("dbo.SupplierOrderProducts", "SupplierOrder_SupplierOrderId", c => c.Guid());
            CreateIndex("dbo.CustomerOrderProducts", "CustomerOrder_CustomerOrderId");
            CreateIndex("dbo.Products", "Supplier_SupplierId");
            CreateIndex("dbo.ProductTags", "TagId");
            CreateIndex("dbo.SupplierOrderProducts", "SupplierOrder_SupplierOrderId");
            AddForeignKey("dbo.CustomerOrderProducts", "CustomerOrder_CustomerOrderId", "dbo.CustomerOrders", "CustomerOrderId");
            AddForeignKey("dbo.SupplierOrderProducts", "SupplierOrder_SupplierOrderId", "dbo.SupplierOrders", "SupplierOrderId");
            AddForeignKey("dbo.Products", "Supplier_SupplierId", "dbo.Suppliers", "SupplierId");
            AddForeignKey("dbo.ProductTags", "TagId", "dbo.Tags", "TagId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductTags", "TagId", "dbo.Tags");
            DropForeignKey("dbo.Transactions", "Supplier_SupplierId", "dbo.Suppliers");
            DropForeignKey("dbo.SupplierOrderTransactions", "Transaction_TransactionId", "dbo.Transactions");
            DropForeignKey("dbo.SupplierOrders", "Supplier_SupplierId", "dbo.Suppliers");
            DropForeignKey("dbo.Products", "Supplier_SupplierId", "dbo.Suppliers");
            DropForeignKey("dbo.SupplierOrderTransactions", "SupplierOrder_SupplierOrderId", "dbo.SupplierOrders");
            DropForeignKey("dbo.SupplierOrderProducts", "SupplierOrder_SupplierOrderId", "dbo.SupplierOrders");
            DropForeignKey("dbo.CustomerOrders", "Customer_CustomerId", "dbo.Customers");
            DropForeignKey("dbo.CustomerOrderProducts", "CustomerOrder_CustomerOrderId", "dbo.CustomerOrders");
            DropIndex("dbo.Transactions", new[] { "Supplier_SupplierId" });
            DropIndex("dbo.SupplierOrderTransactions", new[] { "Transaction_TransactionId" });
            DropIndex("dbo.SupplierOrderTransactions", new[] { "SupplierOrder_SupplierOrderId" });
            DropIndex("dbo.SupplierOrders", new[] { "Supplier_SupplierId" });
            DropIndex("dbo.SupplierOrderProducts", new[] { "SupplierOrder_SupplierOrderId" });
            DropIndex("dbo.ProductTags", new[] { "TagId" });
            DropIndex("dbo.Products", new[] { "Supplier_SupplierId" });
            DropIndex("dbo.CustomerOrders", new[] { "Customer_CustomerId" });
            DropIndex("dbo.CustomerOrderProducts", new[] { "CustomerOrder_CustomerOrderId" });
            DropColumn("dbo.SupplierOrderProducts", "SupplierOrder_SupplierOrderId");
            DropColumn("dbo.CustomerOrderProducts", "CustomerOrder_CustomerOrderId");
            DropColumn("dbo.Products", "Supplier_SupplierId");
            DropTable("dbo.Tags");
            DropTable("dbo.Transactions");
            DropTable("dbo.Suppliers");
            DropTable("dbo.SupplierOrderTransactions");
            DropTable("dbo.SupplierOrders");
            DropTable("dbo.Customers");
            DropTable("dbo.CustomerOrders");
        }
    }
}
