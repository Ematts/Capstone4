namespace Capstone4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class s4 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CompletedServiceRequestFilePaths", "CompletedServiceRequestID", "dbo.CompletedServiceRequests");
            DropIndex("dbo.CompletedServiceRequestFilePaths", new[] { "CompletedServiceRequestID" });
            AddColumn("dbo.CompletedServiceRequestFilePaths", "ServiceRequestID", c => c.Int(nullable: false));
            CreateIndex("dbo.CompletedServiceRequestFilePaths", "ServiceRequestID");
            AddForeignKey("dbo.CompletedServiceRequestFilePaths", "ServiceRequestID", "dbo.ServiceRequests", "ID", cascadeDelete: true);
            DropColumn("dbo.CompletedServiceRequestFilePaths", "CompletedServiceRequestID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CompletedServiceRequestFilePaths", "CompletedServiceRequestID", c => c.Int(nullable: false));
            DropForeignKey("dbo.CompletedServiceRequestFilePaths", "ServiceRequestID", "dbo.ServiceRequests");
            DropIndex("dbo.CompletedServiceRequestFilePaths", new[] { "ServiceRequestID" });
            DropColumn("dbo.CompletedServiceRequestFilePaths", "ServiceRequestID");
            CreateIndex("dbo.CompletedServiceRequestFilePaths", "CompletedServiceRequestID");
            AddForeignKey("dbo.CompletedServiceRequestFilePaths", "CompletedServiceRequestID", "dbo.CompletedServiceRequests", "ID", cascadeDelete: true);
        }
    }
}
