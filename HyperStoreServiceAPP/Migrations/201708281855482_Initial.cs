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
                        DisplayCostSnapShot = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DiscountPerSnapShot = c.Single(nullable: false),
                        CGSTPerSnapShot = c.Single(nullable: false),
                        SGSTPerSnapshot = c.Single(nullable: false),
                        QuantityConsumed = c.Single(nullable: false),
                        NetValue = c.Decimal(nullable: false, precision: 18, scale: 2),
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
                        CustomerOrderNo = c.String(nullable: false),
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
                        MobileNo = c.String(nullable: false, maxLength: 10),
                        Name = c.String(nullable: false, maxLength: 24),
                        WalletBalance = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.CustomerId)
                .Index(t => t.MobileNo, unique: true)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        ProductId = c.Guid(nullable: false),
                        CGSTPer = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Code = c.String(nullable: false, maxLength: 15),
                        DisplayPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DiscountPer = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Name = c.String(nullable: false, maxLength: 24),
                        RefillTime = c.Int(nullable: false),
                        SGSTPer = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Threshold = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProductId)
                .Index(t => t.Code, unique: true)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.CustomerTransactions",
                c => new
                    {
                        CustomerTransactionId = c.Guid(nullable: false),
                        TransactionAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TransactionDate = c.DateTime(nullable: false),
                        TransactionNo = c.String(nullable: false),
                        WalletSnapshot = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CustomerId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.CustomerTransactionId)
                .ForeignKey("dbo.Customers", t => t.CustomerId, cascadeDelete: true)
                .Index(t => t.CustomerId);
            
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
                        TagName = c.String(nullable: false, maxLength: 24),
                    })
                .PrimaryKey(t => t.TagId)
                .Index(t => t.TagName, unique: true);
            
            CreateTable(
                "dbo.SupplierOrderProducts",
                c => new
                    {
                        SupplierOrderProductId = c.Guid(nullable: false),
                        PurchasePrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        QuantityPurchased = c.Single(nullable: false),
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
                        BillAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DueDate = c.DateTime(nullable: false),
                        OrderDate = c.DateTime(nullable: false),
                        PaidAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SupplierOrderNo = c.String(nullable: false),
                        SupplierId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.SupplierOrderId)
                .ForeignKey("dbo.Suppliers", t => t.SupplierId, cascadeDelete: true)
                .Index(t => t.SupplierId);
            
            CreateTable(
                "dbo.Suppliers",
                c => new
                    {
                        SupplierId = c.Guid(nullable: false),
                        Address = c.String(),
                        GSTIN = c.String(),
                        MobileNo = c.String(nullable: false, maxLength: 10),
                        Name = c.String(nullable: false, maxLength: 24),
                        WalletBalance = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.SupplierId)
                .Index(t => t.MobileNo, unique: true);
            
            CreateTable(
                "dbo.SupplierOrderTransactions",
                c => new
                    {
                        SupplierOrderTransactionId = c.Guid(nullable: false),
                        PaidAmount = c.Decimal(precision: 18, scale: 2),
                        IsPaymentComplete = c.Boolean(nullable: false),
                        TransactionId = c.Guid(nullable: false),
                        SupplierOrderId = c.Guid(nullable: false),
                        SupplierTransaction_SupplierTransactionId = c.Guid(),
                    })
                .PrimaryKey(t => t.SupplierOrderTransactionId)
                .ForeignKey("dbo.SupplierOrders", t => t.SupplierOrderId, cascadeDelete: true)
                .ForeignKey("dbo.SupplierTransactions", t => t.SupplierTransaction_SupplierTransactionId)
                .Index(t => t.SupplierOrderId)
                .Index(t => t.SupplierTransaction_SupplierTransactionId);
            
            CreateTable(
                "dbo.SupplierTransactions",
                c => new
                    {
                        SupplierTransactionId = c.Guid(nullable: false),
                        IsCredit = c.Boolean(nullable: false),
                        TransactionNo = c.String(nullable: false),
                        TransactionDate = c.DateTime(nullable: false),
                        TransactionAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalletSnapshot = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SupplierId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.SupplierTransactionId)
                .ForeignKey("dbo.Suppliers", t => t.SupplierId, cascadeDelete: false)
                .Index(t => t.SupplierId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SupplierOrderTransactions", "SupplierTransaction_SupplierTransactionId", "dbo.SupplierTransactions");
            DropForeignKey("dbo.SupplierTransactions", "SupplierId", "dbo.Suppliers");
            DropForeignKey("dbo.SupplierOrderTransactions", "SupplierOrderId", "dbo.SupplierOrders");
            DropForeignKey("dbo.SupplierOrderProducts", "SupplierOrderId", "dbo.SupplierOrders");
            DropForeignKey("dbo.SupplierOrders", "SupplierId", "dbo.Suppliers");
            DropForeignKey("dbo.SupplierOrderProducts", "ProductId", "dbo.Products");
            DropForeignKey("dbo.ProductTags", "TagId", "dbo.Tags");
            DropForeignKey("dbo.ProductTags", "ProductId", "dbo.Products");
            DropForeignKey("dbo.CustomerTransactions", "CustomerId", "dbo.Customers");
            DropForeignKey("dbo.CustomerOrderProducts", "ProductId", "dbo.Products");
            DropForeignKey("dbo.CustomerOrderProducts", "CustomerOrderId", "dbo.CustomerOrders");
            DropForeignKey("dbo.CustomerOrders", "CustomerId", "dbo.Customers");
            DropIndex("dbo.SupplierTransactions", new[] { "SupplierId" });
            DropIndex("dbo.SupplierOrderTransactions", new[] { "SupplierTransaction_SupplierTransactionId" });
            DropIndex("dbo.SupplierOrderTransactions", new[] { "SupplierOrderId" });
            DropIndex("dbo.Suppliers", new[] { "MobileNo" });
            DropIndex("dbo.SupplierOrders", new[] { "SupplierId" });
            DropIndex("dbo.SupplierOrderProducts", new[] { "ProductId" });
            DropIndex("dbo.SupplierOrderProducts", new[] { "SupplierOrderId" });
            DropIndex("dbo.Tags", new[] { "TagName" });
            DropIndex("dbo.ProductTags", new[] { "TagId" });
            DropIndex("dbo.ProductTags", new[] { "ProductId" });
            DropIndex("dbo.CustomerTransactions", new[] { "CustomerId" });
            DropIndex("dbo.Products", new[] { "Name" });
            DropIndex("dbo.Products", new[] { "Code" });
            DropIndex("dbo.Customers", new[] { "Name" });
            DropIndex("dbo.Customers", new[] { "MobileNo" });
            DropIndex("dbo.CustomerOrders", new[] { "CustomerId" });
            DropIndex("dbo.CustomerOrderProducts", new[] { "ProductId" });
            DropIndex("dbo.CustomerOrderProducts", new[] { "CustomerOrderId" });
            DropTable("dbo.SupplierTransactions");
            DropTable("dbo.SupplierOrderTransactions");
            DropTable("dbo.Suppliers");
            DropTable("dbo.SupplierOrders");
            DropTable("dbo.SupplierOrderProducts");
            DropTable("dbo.Tags");
            DropTable("dbo.ProductTags");
            DropTable("dbo.CustomerTransactions");
            DropTable("dbo.Products");
            DropTable("dbo.Customers");
            DropTable("dbo.CustomerOrders");
            DropTable("dbo.CustomerOrderProducts");
        }
    }
}
