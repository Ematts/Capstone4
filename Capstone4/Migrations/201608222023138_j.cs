namespace Capstone4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class j : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Addresses",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Street = c.String(nullable: false, maxLength: 40),
                        City = c.String(nullable: false, maxLength: 40),
                        State = c.String(nullable: false, maxLength: 2),
                        Zip = c.String(nullable: false, maxLength: 5),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.CompletedServiceRequestFilePaths",
                c => new
                    {
                        CompletedServiceRequestFilePathId = c.Int(nullable: false, identity: true),
                        FileName = c.String(maxLength: 255),
                        FileType = c.Int(nullable: false),
                        ServiceRequestID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CompletedServiceRequestFilePathId)
                .ForeignKey("dbo.ServiceRequests", t => t.ServiceRequestID, cascadeDelete: true)
                .Index(t => t.ServiceRequestID);
            
            CreateTable(
                "dbo.ServiceRequests",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        AddressID = c.Int(),
                        ContractorID = c.Int(),
                        HomeownerID = c.Int(nullable: false),
                        ContractorReviewID = c.Int(),
                        PostedDate = c.DateTime(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CompletionDeadline = c.DateTime(nullable: false),
                        Description = c.String(nullable: false, maxLength: 100),
                        Service_Number = c.Int(nullable: false),
                        Expired = c.Boolean(nullable: false),
                        CompletionDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        AmountDue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ContractorPaid = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Addresses", t => t.AddressID)
                .ForeignKey("dbo.Contractors", t => t.ContractorID)
                .ForeignKey("dbo.ContractorReviews", t => t.ContractorReviewID)
                .ForeignKey("dbo.Homeowners", t => t.HomeownerID, cascadeDelete: true)
                .Index(t => t.AddressID)
                .Index(t => t.ContractorID)
                .Index(t => t.HomeownerID)
                .Index(t => t.ContractorReviewID);
            
            CreateTable(
                "dbo.Contractors",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        AddressID = c.Int(),
                        Username = c.String(nullable: false, maxLength: 15),
                        FirstName = c.String(nullable: false, maxLength: 20),
                        LastName = c.String(nullable: false, maxLength: 25),
                        Rating = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Addresses", t => t.AddressID)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.AddressID);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.ContractorReviews",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Review = c.String(),
                        Rating = c.Double(nullable: false),
                        ReviewDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        ContractorID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Contractors", t => t.ContractorID, cascadeDelete: true)
                .Index(t => t.ContractorID);
            
            CreateTable(
                "dbo.ContractorAcceptances",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ContractorID = c.Int(),
                        ServiceRequestID = c.Int(nullable: false),
                        AcceptanceDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Contractors", t => t.ContractorID)
                .ForeignKey("dbo.ServiceRequests", t => t.ServiceRequestID, cascadeDelete: true)
                .Index(t => t.ContractorID)
                .Index(t => t.ServiceRequestID);
            
            CreateTable(
                "dbo.Homeowners",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        AddressID = c.Int(),
                        Username = c.String(nullable: false, maxLength: 15),
                        FirstName = c.String(nullable: false, maxLength: 20),
                        LastName = c.String(nullable: false, maxLength: 25),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Addresses", t => t.AddressID)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.AddressID);
            
            CreateTable(
                "dbo.ServiceRequestFilePaths",
                c => new
                    {
                        ServiceRequestFilePathId = c.Int(nullable: false, identity: true),
                        FileName = c.String(maxLength: 255),
                        FileType = c.Int(nullable: false),
                        ServiceRequestID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ServiceRequestFilePathId)
                .ForeignKey("dbo.ServiceRequests", t => t.ServiceRequestID, cascadeDelete: true)
                .Index(t => t.ServiceRequestID);
            
            CreateTable(
                "dbo.CompletedServiceRequests",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ServiceRequestID = c.Int(nullable: false),
                        CompletionDate = c.DateTime(nullable: false),
                        AmountDue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ContractorPaid = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.ServiceRequests", t => t.ServiceRequestID, cascadeDelete: true)
                .Index(t => t.ServiceRequestID);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.CompletedServiceRequests", "ServiceRequestID", "dbo.ServiceRequests");
            DropForeignKey("dbo.ServiceRequestFilePaths", "ServiceRequestID", "dbo.ServiceRequests");
            DropForeignKey("dbo.ServiceRequests", "HomeownerID", "dbo.Homeowners");
            DropForeignKey("dbo.Homeowners", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Homeowners", "AddressID", "dbo.Addresses");
            DropForeignKey("dbo.ServiceRequests", "ContractorReviewID", "dbo.ContractorReviews");
            DropForeignKey("dbo.ContractorAcceptances", "ServiceRequestID", "dbo.ServiceRequests");
            DropForeignKey("dbo.ContractorAcceptances", "ContractorID", "dbo.Contractors");
            DropForeignKey("dbo.ServiceRequests", "ContractorID", "dbo.Contractors");
            DropForeignKey("dbo.ContractorReviews", "ContractorID", "dbo.Contractors");
            DropForeignKey("dbo.Contractors", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Contractors", "AddressID", "dbo.Addresses");
            DropForeignKey("dbo.CompletedServiceRequestFilePaths", "ServiceRequestID", "dbo.ServiceRequests");
            DropForeignKey("dbo.ServiceRequests", "AddressID", "dbo.Addresses");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.CompletedServiceRequests", new[] { "ServiceRequestID" });
            DropIndex("dbo.ServiceRequestFilePaths", new[] { "ServiceRequestID" });
            DropIndex("dbo.Homeowners", new[] { "AddressID" });
            DropIndex("dbo.Homeowners", new[] { "UserId" });
            DropIndex("dbo.ContractorAcceptances", new[] { "ServiceRequestID" });
            DropIndex("dbo.ContractorAcceptances", new[] { "ContractorID" });
            DropIndex("dbo.ContractorReviews", new[] { "ContractorID" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Contractors", new[] { "AddressID" });
            DropIndex("dbo.Contractors", new[] { "UserId" });
            DropIndex("dbo.ServiceRequests", new[] { "ContractorReviewID" });
            DropIndex("dbo.ServiceRequests", new[] { "HomeownerID" });
            DropIndex("dbo.ServiceRequests", new[] { "ContractorID" });
            DropIndex("dbo.ServiceRequests", new[] { "AddressID" });
            DropIndex("dbo.CompletedServiceRequestFilePaths", new[] { "ServiceRequestID" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.CompletedServiceRequests");
            DropTable("dbo.ServiceRequestFilePaths");
            DropTable("dbo.Homeowners");
            DropTable("dbo.ContractorAcceptances");
            DropTable("dbo.ContractorReviews");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Contractors");
            DropTable("dbo.ServiceRequests");
            DropTable("dbo.CompletedServiceRequestFilePaths");
            DropTable("dbo.Addresses");
        }
    }
}
