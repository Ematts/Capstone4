namespace Capstone4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class g : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ContractorReviews", "ContractorID", "dbo.Contractors");
            DropIndex("dbo.ContractorReviews", new[] { "ContractorID" });
            AlterColumn("dbo.ContractorReviews", "ContractorID", c => c.Int());
            CreateIndex("dbo.ContractorReviews", "ContractorID");
            AddForeignKey("dbo.ContractorReviews", "ContractorID", "dbo.Contractors", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ContractorReviews", "ContractorID", "dbo.Contractors");
            DropIndex("dbo.ContractorReviews", new[] { "ContractorID" });
            AlterColumn("dbo.ContractorReviews", "ContractorID", c => c.Int(nullable: false));
            CreateIndex("dbo.ContractorReviews", "ContractorID");
            AddForeignKey("dbo.ContractorReviews", "ContractorID", "dbo.Contractors", "ID", cascadeDelete: true);
        }
    }
}
