namespace Capstone4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class bb : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ServiceRequests", "PaymentError", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ServiceRequests", "PaymentError");
        }
    }
}
