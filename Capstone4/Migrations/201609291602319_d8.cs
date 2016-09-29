namespace Capstone4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class d8 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Contractors", "Inactive", c => c.Boolean(nullable: false));
            AddColumn("dbo.Homeowners", "Inactive", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Homeowners", "Inactive");
            DropColumn("dbo.Contractors", "Inactive");
        }
    }
}
