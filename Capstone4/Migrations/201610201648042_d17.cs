namespace Capstone4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class d17 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ServiceRequests", "Posted", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ServiceRequests", "Posted");
        }
    }
}
