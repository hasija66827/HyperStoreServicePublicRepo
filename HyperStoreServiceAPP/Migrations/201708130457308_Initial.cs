namespace HyperStoreServiceAPP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CustomerOrderProducts",
                c => new
                    {
                        CustomerOrderProductId = c.Guid(nullable: false),
                        DiscountPerSnapShot = c.Single(nullable: false),
                        DisplayCostSnapShot = c.Single(nullable: false),
                        QuantityPurchased = c.Single(nullable: false),
                        CustomerOrderId = c.Guid(nullable: false),
                        ProductId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.CustomerOrderProductId)
                .ForeignKey("dbo.CustomerOrders", t => t.CustomerOrderId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.CustomerOrderId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.CustomerOrders",
                c => new
                    {
                        CustomerOrderId = c.Guid(nullable: false),
                        CustomerOrderNo = c.String(),
                        OrderDate = c.DateTime(nullable: false),
                        BillAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DiscountedAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsPayingNow = c.Boolean(nullable: false),
                        IsUsingWallet = c.Boolean(nullable: false),
                        PayingAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UsingWalletAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CustomerId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.CustomerOrderId)
                .ForeignKey("dbo.Customers", t => t.CustomerId, cascadeDelete: true)
                .Index(t => t.CustomerId);
            
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        CustomerId = c.Guid(nullable: false),
                        Address = c.String(),
                        GSTIN = c.String(),
                        MobileNo = c.String(nullable: false),
                        Name = c.String(nullable: false),
                        WalletBalance = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.CustomerId);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        ProductId = c.Guid(nullable: false),
                        CGSTPer = c.Single(),
                        Code = c.String(nullable: false),
                        DisplayPrice = c.Single(nullable: false),
                        DiscountPer = c.Single(nullable: false),
                        Name = c.String(nullable: false),
                        RefillTime = c.Int(nullable: false),
                        SGSTPer = c.Single(),
                        Threshold = c.Int(nullable: false),
                        TotalQuantity = c.Single(nullable: false),
                        SupplierId = c.Guid(),
                    })
                .PrimaryKey(t => t.ProductId)
                .ForeignKey("dbo.Suppliers", t => t.SupplierId)
                .Index(t => t.SupplierId);
            
            CreateTable(
                "dbo.Suppliers",
                c => new
                    {
                        SupplierId = c.Guid(nullable: false),
                        Address = c.String(),
                        GSTIN = c.String(),
                        MobileNo = c.String(nullable: false),
                        Name = c.String(nullable: false),
                        WalletBalance = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.SupplierId);
            
            CreateTable(
                "dbo.ProductTags",
                c => new
                    {
                        ProductTagId = c.Guid(nullable: false),
                        ProductId = c.Guid(nullable: false),
                        TagId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.ProductTagId)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .ForeignKey("dbo.Tags", t => t.TagId, cascadeDelete: true)
                .Index(t => t.ProductId)
                .Index(t => t.TagId);
            
            CreateTable(
                "dbo.Tags",
                c => new
                    {
                        TagId = c.Guid(nullable: false),
                        TagName = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.TagId);
            
            CreateTable(
                "dbo.SupplierOrderProducts",
                c => new
                    {
                        SupplierOrderProductId = c.Guid(nullable: false),
                        QuantityPurchased = c.Int(nullable: false),
                        PurchasePrice = c.Single(nullable: false),
                        SupplierOrderId = c.Guid(nullable: false),
                        ProductId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.SupplierOrderProductId)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .ForeignKey("dbo.SupplierOrders", t => t.SupplierOrderId, cascadeDelete: true)
                .Index(t => t.SupplierOrderId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.SupplierOrders",
                c => new
                    {
                        SupplierOrderId = c.Guid(nullable: false),
                        SupplierOrderNo = c.String(nullable: false),
                        OrderDate = c.DateTime(nullable: false),
                        DueDate = c.DateTime(nullable: false),
                        BillAmount = c.Single(nullable: false),
                        PaidAmount = c.Single(nullable: false),
                        SupplierId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.SupplierOrderId)
                .ForeignKey("dbo.Suppliers", t => t.SupplierId, cascadeDelete: true)
                .Index(t => t.SupplierId);
            
            CreateTable(
                "dbo.SupplierOrderTransactions",
                c => new
                    {
                        SupplierOrderTransactionId = c.Guid(nullable: false),
                        PaidAmount = c.Single(nullable: false),
                        IsPaymentComplete = c.Boolean(nullable: false),
                        TransactionId = c.Guid(nullable: false),
                        SupplierOrderId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.SupplierOrderTransactionId)
                .ForeignKey("dbo.SupplierOrders", t => t.SupplierOrderId, cascadeDelete: true)
                .ForeignKey("dbo.Transactions", t => t.TransactionId, cascadeDelete: true)
                .Index(t => t.TransactionId)
                .Index(t => t.SupplierOrderId);
            
            CreateTable(
                "dbo.Transactions",
                c => new
                    {
                        TransactionId = c.Guid(nullable: false),
                        TransactionNo = c.String(nullable: false),
                        CreditAmount = c.Single(nullable: false),
                        TransactionDate = c.DateTime(nullable: false),
                        WalletSnapshot = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.TransactionId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SupplierOrderTransactions", "TransactionId", "dbo.Transactions");
            DropForeignKey("dbo.SupplierOrderTransactions", "SupplierOrderId", "dbo.SupplierOrders");
            DropForeignKey("dbo.SupplierOrderProducts", "SupplierOrderId", "dbo.SupplierOrders");
            DropForeignKey("dbo.SupplierOrders", "SupplierId", "dbo.Suppliers");
            DropForeignKey("dbo.SupplierOrderProducts", "ProductId", "dbo.Products");
            DropForeignKey("dbo.ProductTags", "TagId", "dbo.Tags");
            DropForeignKey("dbo.ProductTags", "ProductId", "dbo.Products");
            DropForeignKey("dbo.CustomerOrderProducts", "ProductId", "dbo.Products");
            DropForeignKey("dbo.Products", "SupplierId", "dbo.Suppliers");
            DropForeignKey("dbo.CustomerOrderProducts", "CustomerOrderId", "dbo.CustomerOrders");
            DropForeignKey("dbo.CustomerOrders", "CustomerId", "dbo.Customers");
            DropIndex("dbo.SupplierOrderTransactions", new[] { "SupplierOrderId" });
            DropIndex("dbo.SupplierOrderTransactions", new[] { "TransactionId" });
            DropIndex("dbo.SupplierOrders", new[] { "SupplierId" });
            DropIndex("dbo.SupplierOrderProducts", new[] { "ProductId" });
            DropIndex("dbo.SupplierOrderProducts", new[] { "SupplierOrderId" });
            DropIndex("dbo.ProductTags", new[] { "TagId" });
            DropIndex("dbo.ProductTags", new[] { "ProductId" });
            DropIndex("dbo.Products", new[] { "SupplierId" });
            DropIndex("dbo.CustomerOrders", new[] { "CustomerId" });
            DropIndex("dbo.CustomerOrderProducts", new[] { "ProductId" });
            DropIndex("dbo.CustomerOrderProducts", new[] { "CustomerOrderId" });
            DropTable("dbo.Transactions");
            DropTable("dbo.SupplierOrderTransactions");
            DropTable("dbo.SupplierOrders");
            DropTable("dbo.SupplierOrderProducts");
            DropTable("dbo.Tags");
            DropTable("dbo.ProductTags");
            DropTable("dbo.Suppliers");
            DropTable("dbo.Products");
            DropTable("dbo.Customers");
            DropTable("dbo.CustomerOrders");
            DropTable("dbo.CustomerOrderProducts");
        }
    }
}
