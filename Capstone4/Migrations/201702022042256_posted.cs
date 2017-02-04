namespace Capstone4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class posted : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ServiceRequests", "PostedAmbigTime", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ServiceRequests", "PostedAmbigTime");
        }
    }
}
