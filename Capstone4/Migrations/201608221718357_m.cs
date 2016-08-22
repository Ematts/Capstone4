namespace Capstone4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ContractorReviews",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Review = c.String(),
                        Rating = c.Double(nullable: false),
                        CompletionDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        ServiceRequestID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.ServiceRequests", t => t.ServiceRequestID, cascadeDelete: true)
                .Index(t => t.ServiceRequestID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ContractorReviews", "ServiceRequestID", "dbo.ServiceRequests");
            DropIndex("dbo.ContractorReviews", new[] { "ServiceRequestID" });
            DropTable("dbo.ContractorReviews");
        }
    }
}
