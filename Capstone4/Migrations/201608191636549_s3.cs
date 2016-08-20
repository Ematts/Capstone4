namespace Capstone4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class s3 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ContractorAcceptances",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ServiceRequestID = c.Int(nullable: false),
                        AcceptanceDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.ServiceRequests", t => t.ServiceRequestID, cascadeDelete: true)
                .Index(t => t.ServiceRequestID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ContractorAcceptances", "ServiceRequestID", "dbo.ServiceRequests");
            DropIndex("dbo.ContractorAcceptances", new[] { "ServiceRequestID" });
            DropTable("dbo.ContractorAcceptances");
        }
    }
}
