namespace Capstone4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class f5 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ContractorReviews", "ReviewDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
            DropColumn("dbo.ContractorReviews", "CompletionDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ContractorReviews", "CompletionDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
            DropColumn("dbo.ContractorReviews", "ReviewDate");
        }
    }
}
