namespace Capstone4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class vv : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ServiceRequests", "AmbigTime", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ServiceRequests", "AmbigTime");
        }
    }
}
