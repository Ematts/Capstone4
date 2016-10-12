namespace Capstone4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class d13 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ReviewResponses", "Response", c => c.String());
            DropColumn("dbo.ReviewResponses", "Review");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ReviewResponses", "Review", c => c.String());
            DropColumn("dbo.ReviewResponses", "Response");
        }
    }
}
