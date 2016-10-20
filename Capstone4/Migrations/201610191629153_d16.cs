namespace Capstone4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class d16 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Addresses", "ManualValidated", c => c.Boolean());
            DropColumn("dbo.ServiceRequests", "ManualValidated");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ServiceRequests", "ManualValidated", c => c.Boolean());
            DropColumn("dbo.Addresses", "ManualValidated");
        }
    }
}
