namespace Capstone4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class d1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ServiceRequests", "ContractorID", "dbo.Contractors");
            DropIndex("dbo.ServiceRequests", new[] { "ContractorID" });
            AlterColumn("dbo.ServiceRequests", "ContractorID", c => c.Int());
            CreateIndex("dbo.ServiceRequests", "ContractorID");
            AddForeignKey("dbo.ServiceRequests", "ContractorID", "dbo.Contractors", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ServiceRequests", "ContractorID", "dbo.Contractors");
            DropIndex("dbo.ServiceRequests", new[] { "ContractorID" });
            AlterColumn("dbo.ServiceRequests", "ContractorID", c => c.Int(nullable: false));
            CreateIndex("dbo.ServiceRequests", "ContractorID");
            AddForeignKey("dbo.ServiceRequests", "ContractorID", "dbo.Contractors", "ID", cascadeDelete: true);
        }
    }
}
