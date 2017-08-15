namespace HyperStoreServiceAPP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Second : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Transactions", "TransactionAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Transactions", "SupplierId", c => c.Guid(nullable: false));
            AlterColumn("dbo.Suppliers", "WalletBalance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Transactions", "WalletSnapshot", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            CreateIndex("dbo.Transactions", "SupplierId");
            AddForeignKey("dbo.Transactions", "SupplierId", "dbo.Suppliers", "SupplierId", cascadeDelete: false);
            DropColumn("dbo.Transactions", "CreditAmount");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Transactions", "CreditAmount", c => c.Single(nullable: false));
            DropForeignKey("dbo.Transactions", "SupplierId", "dbo.Suppliers");
            DropIndex("dbo.Transactions", new[] { "SupplierId" });
            AlterColumn("dbo.Transactions", "WalletSnapshot", c => c.Single(nullable: false));
            AlterColumn("dbo.Suppliers", "WalletBalance", c => c.Single(nullable: false));
            DropColumn("dbo.Transactions", "SupplierId");
            DropColumn("dbo.Transactions", "TransactionAmount");
        }
    }
}
