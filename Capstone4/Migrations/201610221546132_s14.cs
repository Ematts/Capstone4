namespace Capstone4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class s14 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Contractors", "NeedsManualValidation", c => c.Boolean());
            AddColumn("dbo.Homeowners", "NeedsManualValidation", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Homeowners", "NeedsManualValidation");
            DropColumn("dbo.Contractors", "NeedsManualValidation");
        }
    }
}
