namespace Capstone4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class f2 : DbMigration
    {
        public override void Up()
        {
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
                "dbo.CompletedServiceRequestFilePaths",
                c => new
                    {
                        CompletedServiceRequestFilePathId = c.Int(nullable: false, identity: true),
                        FileName = c.String(maxLength: 255),
                        FileType = c.Int(nullable: false),
                        CompletedServiceRequestID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CompletedServiceRequestFilePathId)
                .ForeignKey("dbo.CompletedServiceRequests", t => t.CompletedServiceRequestID, cascadeDelete: true)
                .Index(t => t.CompletedServiceRequestID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CompletedServiceRequests", "ServiceRequestID", "dbo.ServiceRequests");
            DropForeignKey("dbo.CompletedServiceRequestFilePaths", "CompletedServiceRequestID", "dbo.CompletedServiceRequests");
            DropIndex("dbo.CompletedServiceRequestFilePaths", new[] { "CompletedServiceRequestID" });
            DropIndex("dbo.CompletedServiceRequests", new[] { "ServiceRequestID" });
            DropTable("dbo.CompletedServiceRequestFilePaths");
            DropTable("dbo.CompletedServiceRequests");
        }
    }
}
