namespace Capstone4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class f4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ServiceRequests", "CompletionAmbigTime", c => c.String());
            AddColumn("dbo.ContractorAcceptances", "AcceptanceAmbigTime", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ContractorAcceptances", "AcceptanceAmbigTime");
            DropColumn("dbo.ServiceRequests", "CompletionAmbigTime");
        }
    }
}
