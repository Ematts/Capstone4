namespace Capstone4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class s13 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ServiceRequests", "NeedsManualValidation", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ServiceRequests", "NeedsManualValidation");
        }
    }
}
