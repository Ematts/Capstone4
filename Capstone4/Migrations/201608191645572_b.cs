namespace Capstone4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class b : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ContractorAcceptances", "ContractorID", c => c.Int());
            CreateIndex("dbo.ContractorAcceptances", "ContractorID");
            AddForeignKey("dbo.ContractorAcceptances", "ContractorID", "dbo.Contractors", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ContractorAcceptances", "ContractorID", "dbo.Contractors");
            DropIndex("dbo.ContractorAcceptances", new[] { "ContractorID" });
            DropColumn("dbo.ContractorAcceptances", "ContractorID");
        }
    }
}
