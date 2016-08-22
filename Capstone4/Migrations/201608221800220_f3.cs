namespace Capstone4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class f3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ContractorReviews", "ServiceRequestID", "dbo.ServiceRequests");
            DropIndex("dbo.ContractorReviews", new[] { "ServiceRequestID" });
            AddColumn("dbo.ServiceRequests", "ContractorReviewID", c => c.Int());
            CreateIndex("dbo.ServiceRequests", "ContractorReviewID");
            AddForeignKey("dbo.ServiceRequests", "ContractorReviewID", "dbo.ContractorReviews", "ID");
            DropColumn("dbo.ContractorReviews", "ServiceRequestID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ContractorReviews", "ServiceRequestID", c => c.Int(nullable: false));
            DropForeignKey("dbo.ServiceRequests", "ContractorReviewID", "dbo.ContractorReviews");
            DropIndex("dbo.ServiceRequests", new[] { "ContractorReviewID" });
            DropColumn("dbo.ServiceRequests", "ContractorReviewID");
            CreateIndex("dbo.ContractorReviews", "ServiceRequestID");
            AddForeignKey("dbo.ContractorReviews", "ServiceRequestID", "dbo.ServiceRequests", "ID", cascadeDelete: true);
        }
    }
}
