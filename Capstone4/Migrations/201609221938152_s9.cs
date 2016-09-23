namespace Capstone4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class s9 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Addresses", "vacant", c => c.Boolean(nullable: false));
            AddColumn("dbo.Addresses", "validated", c => c.Boolean(nullable: false));
            AddColumn("dbo.ServiceRequests", "Inactive", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ServiceRequests", "Inactive");
            DropColumn("dbo.Addresses", "validated");
            DropColumn("dbo.Addresses", "vacant");
        }
    }
}
