namespace Capstone4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class d12 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ReviewResponses",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Review = c.String(),
                        ResponseDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        ContractorID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Contractors", t => t.ContractorID)
                .Index(t => t.ContractorID);
            
            AddColumn("dbo.ContractorReviews", "ReviewResponseID", c => c.Int());
            CreateIndex("dbo.ContractorReviews", "ReviewResponseID");
            AddForeignKey("dbo.ContractorReviews", "ReviewResponseID", "dbo.ReviewResponses", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ContractorReviews", "ReviewResponseID", "dbo.ReviewResponses");
            DropForeignKey("dbo.ReviewResponses", "ContractorID", "dbo.Contractors");
            DropIndex("dbo.ReviewResponses", new[] { "ContractorID" });
            DropIndex("dbo.ContractorReviews", new[] { "ReviewResponseID" });
            DropColumn("dbo.ContractorReviews", "ReviewResponseID");
            DropTable("dbo.ReviewResponses");
        }
    }
}
