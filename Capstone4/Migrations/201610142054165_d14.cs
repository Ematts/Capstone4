namespace Capstone4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class d14 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ReviewResponses", "Response", c => c.String(maxLength: 500));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ReviewResponses", "Response", c => c.String());
        }
    }
}
