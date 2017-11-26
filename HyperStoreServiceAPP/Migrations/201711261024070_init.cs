namespace HyperStoreServiceAPP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CartProducts",
                c => new
                    {
                        CartProductId = c.Guid(nullable: false),
                        PotentielQuantityPurhcased = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PotentielPurchasePrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CartId = c.Guid(nullable: false),
                        ProductId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.CartProductId)
                .ForeignKey("dbo.Carts", t => t.CartId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.CartId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.Carts",
                c => new
                    {
                        CartId = c.Guid(nullable: false),
                        CartStatus = c.Int(nullable: false),
                        IsBookmarked = c.Boolean(),
                        OrderCompletionDate = c.DateTime(),
                        PreferedDeliveryTime = c.DateTime(),
                        PersonId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.CartId)
                .ForeignKey("dbo.People", t => t.PersonId, cascadeDelete: true)
                .Index(t => t.PersonId);
            
            CreateTable(
                "dbo.People",
                c => new
                    {
                        PersonId = c.Guid(nullable: false),
                        EntityType = c.Int(nullable: false),
                        Address = c.String(),
                        GSTIN = c.String(),
                        MobileNo = c.String(nullable: false, maxLength: 10),
                        Name = c.String(nullable: false, maxLength: 24),
                        WalletBalance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NetWorth = c.Decimal(precision: 18, scale: 2),
                        FirstVisited = c.DateTime(nullable: false),
                        LastVisited = c.DateTime(nullable: false),
                        LastCalled = c.DateTime(),
                        Rating = c.Int(),
                        PreferedTimeToContact = c.DateTime(),
                    })
                .PrimaryKey(t => t.PersonId)
                .Index(t => t.MobileNo, unique: true);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        ProductId = c.Guid(nullable: false),
                        CGSTPer = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Code = c.String(nullable: false, maxLength: 15),
                        MRP = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DiscountPer = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Name = c.String(nullable: false, maxLength: 24),
                        HSN = c.Int(),
                        SGSTPer = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Threshold = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LatestSupplierId = c.Guid(),
                    })
                .PrimaryKey(t => t.ProductId)
                .ForeignKey("dbo.People", t => t.LatestSupplierId)
                .Index(t => t.Code, unique: true)
                .Index(t => t.Name, unique: true)
                .Index(t => t.LatestSupplierId);
            
            CreateTable(
                "dbo.DeficientStockHits",
                c => new
                    {
                        DeficientStockHitId = c.Guid(nullable: false),
                        QuantitySnapshot = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProductId = c.Guid(nullable: false),
                        TimeStamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.DeficientStockHitId)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => new { t.ProductId, t.TimeStamp }, unique: true, name: "IX_ProductIdAndTimeStamp");
            
            CreateTable(
                "dbo.OrderProducts",
                c => new
                    {
                        OrderProductId = c.Guid(nullable: false),
                        PurchasePrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        QuantityPurchased = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrderId = c.Guid(nullable: false),
                        ProductId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.OrderProductId)
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.OrderId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        OrderId = c.Guid(nullable: false),
                        EntityType = c.Int(nullable: false),
                        BillAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DueDate = c.DateTime(nullable: false),
                        OrderDate = c.DateTime(nullable: false),
                        PayedAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SettledPayedAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        InterestRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalItems = c.Int(nullable: false),
                        TotalQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrderNo = c.String(nullable: false),
                        PersonId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.OrderId)
                .ForeignKey("dbo.People", t => t.PersonId, cascadeDelete: true)
                .Index(t => t.PersonId);
            
            CreateTable(
                "dbo.OrderTransactions",
                c => new
                    {
                        OrderTransactionId = c.Guid(nullable: false),
                        PaidAmount = c.Decimal(precision: 18, scale: 2),
                        IsPaymentComplete = c.Boolean(nullable: false),
                        TransactionId = c.Guid(nullable: false),
                        OrderId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.OrderTransactionId)
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .ForeignKey("dbo.Transactions", t => t.TransactionId, cascadeDelete: true)
                .Index(t => t.TransactionId)
                .Index(t => t.OrderId);
            
            CreateTable(
                "dbo.Transactions",
                c => new
                    {
                        TransactionId = c.Guid(nullable: false),
                        EntityType = c.Int(nullable: false),
                        IsCredit = c.Boolean(nullable: false),
                        TransactionNo = c.String(nullable: false),
                        OrderNo = c.String(),
                        TransactionDate = c.DateTime(nullable: false),
                        TransactionAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalletSnapshot = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PersonId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.TransactionId)
                .ForeignKey("dbo.People", t => t.PersonId, cascadeDelete: false)
                .Index(t => t.PersonId);
            
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
                "dbo.PurchaseHistories",
                c => new
                    {
                        PurchaseHistoryId = c.Guid(nullable: false),
                        LatestPurchaseDate = c.DateTime(nullable: false),
                        ExpiryDays = c.Int(),
                        PersonId = c.Guid(nullable: false),
                        ProductId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.PurchaseHistoryId)
                .ForeignKey("dbo.People", t => t.PersonId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => new { t.PersonId, t.ProductId }, unique: true, name: "IX_PersonIdAndProductId");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PurchaseHistories", "ProductId", "dbo.Products");
            DropForeignKey("dbo.PurchaseHistories", "PersonId", "dbo.People");
            DropForeignKey("dbo.ProductTags", "TagId", "dbo.Tags");
            DropForeignKey("dbo.ProductTags", "ProductId", "dbo.Products");
            DropForeignKey("dbo.OrderTransactions", "TransactionId", "dbo.Transactions");
            DropForeignKey("dbo.Transactions", "PersonId", "dbo.People");
            DropForeignKey("dbo.OrderTransactions", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.OrderProducts", "ProductId", "dbo.Products");
            DropForeignKey("dbo.OrderProducts", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.Orders", "PersonId", "dbo.People");
            DropForeignKey("dbo.DeficientStockHits", "ProductId", "dbo.Products");
            DropForeignKey("dbo.CartProducts", "ProductId", "dbo.Products");
            DropForeignKey("dbo.Products", "LatestSupplierId", "dbo.People");
            DropForeignKey("dbo.CartProducts", "CartId", "dbo.Carts");
            DropForeignKey("dbo.Carts", "PersonId", "dbo.People");
            DropIndex("dbo.PurchaseHistories", "IX_PersonIdAndProductId");
            DropIndex("dbo.Tags", new[] { "TagName" });
            DropIndex("dbo.ProductTags", new[] { "TagId" });
            DropIndex("dbo.ProductTags", new[] { "ProductId" });
            DropIndex("dbo.Transactions", new[] { "PersonId" });
            DropIndex("dbo.OrderTransactions", new[] { "OrderId" });
            DropIndex("dbo.OrderTransactions", new[] { "TransactionId" });
            DropIndex("dbo.Orders", new[] { "PersonId" });
            DropIndex("dbo.OrderProducts", new[] { "ProductId" });
            DropIndex("dbo.OrderProducts", new[] { "OrderId" });
            DropIndex("dbo.DeficientStockHits", "IX_ProductIdAndTimeStamp");
            DropIndex("dbo.Products", new[] { "LatestSupplierId" });
            DropIndex("dbo.Products", new[] { "Name" });
            DropIndex("dbo.Products", new[] { "Code" });
            DropIndex("dbo.People", new[] { "MobileNo" });
            DropIndex("dbo.Carts", new[] { "PersonId" });
            DropIndex("dbo.CartProducts", new[] { "ProductId" });
            DropIndex("dbo.CartProducts", new[] { "CartId" });
            DropTable("dbo.PurchaseHistories");
            DropTable("dbo.Tags");
            DropTable("dbo.ProductTags");
            DropTable("dbo.Transactions");
            DropTable("dbo.OrderTransactions");
            DropTable("dbo.Orders");
            DropTable("dbo.OrderProducts");
            DropTable("dbo.DeficientStockHits");
            DropTable("dbo.Products");
            DropTable("dbo.People");
            DropTable("dbo.Carts");
            DropTable("dbo.CartProducts");
        }
    }
}
