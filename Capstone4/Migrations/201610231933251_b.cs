namespace Capstone4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class b : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ServiceRequests", "PostedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ServiceRequests", "PostedDate", c => c.DateTime(nullable: false));
        }
    }
}
