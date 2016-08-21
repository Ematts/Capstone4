namespace Capstone4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class b1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ServiceRequests", "CompletionDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AddColumn("dbo.ServiceRequests", "AmountDue", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.ServiceRequests", "ContractorPaid", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ServiceRequests", "ContractorPaid");
            DropColumn("dbo.ServiceRequests", "AmountDue");
            DropColumn("dbo.ServiceRequests", "CompletionDate");
        }
    }
}
