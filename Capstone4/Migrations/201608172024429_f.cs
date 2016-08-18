namespace Capstone4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class f : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ServiceRequestFilePaths",
                c => new
                    {
                        ServiceRequestFilePathId = c.Int(nullable: false, identity: true),
                        FileName = c.String(maxLength: 255),
                        FileType = c.Int(nullable: false),
                        ServiceRequestID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ServiceRequestFilePathId)
                .ForeignKey("dbo.ServiceRequests", t => t.ServiceRequestID, cascadeDelete: true)
                .Index(t => t.ServiceRequestID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ServiceRequestFilePaths", "ServiceRequestID", "dbo.ServiceRequests");
            DropIndex("dbo.ServiceRequestFilePaths", new[] { "ServiceRequestID" });
            DropTable("dbo.ServiceRequestFilePaths");
        }
    }
}
