namespace Capstone4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class d15 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ServiceRequests", "ManualValidated", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ServiceRequests", "ManualValidated");
        }
    }
}
