namespace Capstone4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ff : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ServiceRequests", "WarningSent", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ServiceRequests", "WarningSent");
        }
    }
}
