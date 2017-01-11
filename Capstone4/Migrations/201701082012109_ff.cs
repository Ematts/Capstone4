namespace Capstone4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ff : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ServiceRequests", "UTCDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ServiceRequests", "UTCDate");
        }
    }
}
