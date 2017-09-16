namespace HyperStoreServiceAPP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingUserEntity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Guid(nullable: false),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        EmailId = c.String(nullable: false),
                        DateOfBirth = c.DateTime(nullable: false),
                        MobileNo = c.String(nullable: false, maxLength: 10),
                        Password = c.String(nullable: false),
                        BusinessName = c.String(nullable: false),
                        BusinessType = c.String(nullable: false),
                        GSTIN = c.String(nullable: false),
                        AddressLine = c.String(nullable: false),
                        City = c.String(nullable: false),
                        PinCode = c.String(nullable: false),
                        State = c.String(nullable: false),
                        Latitude = c.String(nullable: false),
                        Longitude = c.String(nullable: false),
                        NumberOfSMS = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserId)
                .Index(t => t.MobileNo, unique: true);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Users", new[] { "MobileNo" });
            DropTable("dbo.Users");
        }
    }
}
