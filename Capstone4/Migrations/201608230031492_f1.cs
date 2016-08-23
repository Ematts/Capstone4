namespace Capstone4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class f1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ContractorReviews", "Rating", c => c.Double());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ContractorReviews", "Rating", c => c.Double(nullable: false));
        }
    }
}
