namespace Capstone4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fff : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ServiceRequests",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        AddressID = c.Int(),
                        ContractorID = c.Int(nullable: false),
                        HomeownerID = c.Int(nullable: false),
                        PostedDate = c.DateTime(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CompletionDeadline = c.DateTime(nullable: false),
                        Description = c.String(nullable: false, maxLength: 100),
                        Service_Number = c.Int(nullable: false),
                        Expired = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Addresses", t => t.AddressID)
                .ForeignKey("dbo.Contractors", t => t.ContractorID, cascadeDelete: true)
                .ForeignKey("dbo.Homeowners", t => t.HomeownerID, cascadeDelete: true)
                .Index(t => t.AddressID)
                .Index(t => t.ContractorID)
                .Index(t => t.HomeownerID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ServiceRequests", "HomeownerID", "dbo.Homeowners");
            DropForeignKey("dbo.ServiceRequests", "ContractorID", "dbo.Contractors");
            DropForeignKey("dbo.ServiceRequests", "AddressID", "dbo.Addresses");
            DropIndex("dbo.ServiceRequests", new[] { "HomeownerID" });
            DropIndex("dbo.ServiceRequests", new[] { "ContractorID" });
            DropIndex("dbo.ServiceRequests", new[] { "AddressID" });
            DropTable("dbo.ServiceRequests");
        }
    }
}
