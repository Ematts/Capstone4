namespace Capstone4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class h : DbMigration
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
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.ServiceRequests", "ContractorReviewID", c => c.Int());
            CreateIndex("dbo.ServiceRequests", "ContractorReviewID");
            AddForeignKey("dbo.ServiceRequests", "ContractorReviewID", "dbo.ContractorReviews", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ServiceRequests", "ContractorReviewID", "dbo.ContractorReviews");
            DropIndex("dbo.ServiceRequests", new[] { "ContractorReviewID" });
            DropColumn("dbo.ServiceRequests", "ContractorReviewID");
            DropTable("dbo.ContractorReviews");
        }
    }
}
